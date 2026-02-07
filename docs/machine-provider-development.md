# Machine Provider Development Guide

This guide describes how to create a machine provider plugin for Overseer. Machine providers are responsible for managing the state and operations of machines in the system.

## Goals

- Keep machine provider code isolated in a class library.
- Your plugin should be in the Overseer namespace.
  - This is so your assemblies are prefixed with "Overseer", e.g., "Overseer.Your.Machine.Provider.dll"
- Implement `IMachineProvider<TMachine>` to provide machine-specific operations.
- Implement `IPluginConfiguration` to register your machine provider and any dependencies.

---
## Creating a Machine Provider

This example shows the minimal pieces you typically need to provide.

### 1) Define machine properties 

Create a derived machine class with properties that wrap the base `Properties` dictionary. Use the `MachinePropertyAttribute` to control how these properties are displayed in the UI:

```csharp
public class CustomMachine : Machine
{
  [MachineProperty(DisplayName = "API Key")]
  public string ApiKey 
  { 
    get => Properties.GetValueOrDefault("ApiKey");
    set => Properties["ApiKey"] = value;
  }
  
  [MachineProperty(DisplayName = "Hostname", DisplayType = MachinePropertyDisplayType.SetupOnly)]
  public string Hostname 
  { 
    get => Properties.GetValueOrDefault("Hostname", out var value);
    set => Properties["Hostname"] = value;
  }

  [MachineProperty(IsIgnored = true)] // not show on client
  public string PersistedProperty
  { 
    get => Properties.GetValueOrDefault("PersistedProperty", out var value);
    set => Properties["PersistedProperty"] = value;
  }
}
```

> **Important:** Custom properties should wrap the `Properties` dictionary from the base `Machine` class. This ensures that property values are properly persisted, retrieved, and displayed by the Overseer system. 



### 2) Implement the `IMachineProvider<TMachine>` interface

Create a class that implements `IMachineProvider<TMachine>` where `TMachine` is a strongly-typed machine class derived from or compatible with the `Machine` type:

```csharp
// CustomMachineProvider.cs
using Overseer.Server.Integration.Machines;

public class CustomMachineProvider : IMachineProvider<CustomMachine>
{ 
  public event EventHandler<MachineStatusEventArgs> StatusUpdated;

  public string MachineType => "Custom";
  
  public Task PauseJob()
  {
    // Implement pause logic for Custom machines
    return Task.CompletedTask;
  }
  
  public Task ResumeJob()
  {
    // Implement resume logic for Custom machines
    return Task.CompletedTask;
  }
  
  public Task CancelJob()
  {
    // Implement cancel logic for Custom machines
    return Task.CompletedTask;
  } 

  public Task Configure(Machine machine)
  {
    // This gets called after creation or updates, but before the machine data is persisted. 
    // Use this method to define the tools that are supported by the machine
    return Task.CompletedTask;
  }

  public void Start(int interval) 
  {
    // Start monitoring the machine state. 
    // 
    // The interval parameter represents the polling rate in seconds. 
    // If the provider polls for updates it should do it at this interval. 
    // 
    // If the provider supports real-time communication (WebSocket, MQTT, etc.), send updates as they arrive.
    // However, when the machine is idle and no updates are incoming, periodically emit an update
    // to prevent Overseer from marking the machine as offline.
 
    var timer = new System.Timers.Timer(interval);
    timer.Elapsed += async (sender, e) => 
    { 
      var status = await GetMachineStatusAsync();
      StatusUpdated?.Invoke(this, new MachineStatusEventArgs(status));
    };
    timer.Start(); 
  }

  public void Stop() 
  {
    // Should stop monitoring and clean up any resources
  }
}
```

### 3) Implement `IPluginConfiguration` to register services

Create a class that implements `IPluginConfiguration` to register your machine provider and any dependencies:

```csharp
// PluginConfiguration.cs
using Microsoft.Extensions.DependencyInjection;
using Overseer.Server.Integration;

public class PluginConfiguration : IPluginConfiguration
{
  public void ConfigureServices(IServiceCollection services)
  {
    // Optionally register other dependencies required by your provider
    // services.AddSingleton<IHttpClientFactory>();
    // services.AddSingleton<IPrusaApiClient, PrusaApiClient>();
    
    // Register the machine provider
    services.AddTransient<IMachineProvider<CustomMachine>, CustomMachineProvider>();
  }
}
```

> Note: The host will discover `IPluginConfiguration` implementations (for example via reflection) and invoke `ConfigureServices` when composing the application.

--- 

## Available Machine Features

Machines support the following optional features:

- **Webcam**: The `WebcamUrl` property can provide a live stream URL.
- **Webcam Orientation**: Use `WebcamOrientation` to rotate the webcam feed if needed.
- **Tools**: A machine can have multiple tools (e.g., extruders, spindles) defined using the `MachineTool` class.
- **Custom Properties**: The base `Machine` class provides a `Properties` dictionary for storing machine-specific data. Custom machine implementations should expose strongly-typed properties that wrap this dictionary using get/set accessors. Use `MachinePropertyAttribute` to control UI display.

### Working with the Properties Dictionary

The `Properties` dictionary on the base `Machine` class is the persistent storage for all custom machine data. When creating a derived machine class:

1. **Always wrap the Properties dictionary** — Don't create standalone properties; they won't be persisted
2. **Use consistent keys** — The dictionary key should match your property name for clarity
3. **Handle type conversions** — Convert values appropriately when reading from the dictionary
4. **Provide defaults** — Return sensible defaults when a property hasn't been set

Example with different property types:

```csharp
public class CustomMachine : Machine
{
  [MachineProperty(DisplayName = "API Key", DisplayType = MachinePropertyDisplayType.Password)]
  public string ApiKey 
  { 
    get => Properties.TryGetValue(nameof(ApiKey), out var value) ? value?.ToString() ?? string.Empty : string.Empty;
    set => Properties[nameof(ApiKey)] = value;
  }
  
  [MachineProperty(DisplayName = "Port Number")]
  public int Port 
  { 
    get => Properties.TryGetValue(nameof(Port), out var value) && int.TryParse(value?.ToString(), out var port) ? port : 8080;
    set => Properties[nameof(Port)] = value;
  }
  
  [MachineProperty(DisplayName = "Enable SSL")]
  public bool EnableSsl 
  { 
    get => Properties.TryGetValue(nameof(EnableSsl), out var value) && bool.TryParse(value?.ToString(), out var enabled) && enabled;
    set => Properties[nameof(EnableSsl)] = value;
  }
}
```

---

## Best Practices & Tips 💡

- Keep provider behavior focused: implement only the operations your machine type supports.
- Prefer constructor-injected dependencies; register interfaces rather than concrete classes when appropriate.
- Make your provider classes `public` so they can be discovered and instantiated by the host.
- Avoid side effects in `ConfigureServices`; keep service registrations deterministic.
- Use the `Machine` base class to ensure compatibility with the Overseer system.
- Consider how your machine handles disabled state; the host will not load providers for disabled machines.
- Design your provider to be stateless when possible; machines are typically instantiated per request.

---

## Quick Checklist Before Publishing a Machine Provider 📋

- [ ] Project is a class library targeting a compatible framework
- [ ] Machine provider class is `public` and implements `IMachineProvider<TMachine>`
- [ ] `MachineType` property correctly identifies your machine type
- [ ] `IPluginConfiguration` implemented and registers the provider
- [ ] All required interface methods are implemented
- [ ] Dependencies are registered and documented
- [ ] Machine properties (if used) are properly documented

---

## Reference Implementations

For reference implementations, please view the Overseer plugins on the [OverseerApp GitHub organization](https://github.com/OverseerApp).
