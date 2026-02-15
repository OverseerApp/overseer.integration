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
// CustomMachine.cs
[MachineType("Custom")]
public class CustomMachine : Machine
{
  [MachineProperty(displayName: "API Key", isSensitive: true, isRequired: true)]
  public string ApiKey
  {
    get => Properties.GetValueOrDefault("ApiKey");
    set => Properties["ApiKey"] = value;
  }

  /// This property is collected when adding the printer to Overseer, but now on the machine edit screen.
  /// Available options are "Both", "SetupOnly", and "UpdateOnly"
  [MachineProperty(displayName: "Hostname", displayType: MachinePropertyDisplayType.SetupOnly)]
  public string Hostname
  {
    get => GetProperty(nameof(Hostname));
    set => SetProperty(nameof(Hostname));
  }

  /// This property is persisted with the machine, but won't be displayed to the user
  [MachineProperty(isIgnored: true)]
  public string PersistedProperty
  {
    get => GetProperty(nameof(PersistedProperty));
    set => SetProperty(nameof(PersistedProperty));
  }
}
```

> **Important:** Custom properties should wrap the `Properties` dictionary from the base `Machine` class. This ensures that property values are properly persisted, retrieved, and displayed by the Overseer system. The `Machine` class contains methods to help with managing the property wrappers.
> However, it's perfectly acceptable to interact with the `Properties` dictionary directly as well.

### 2) Implement the `IMachineConfigurationProvider<TMachine>` interface

Create a class that implements `IMachineConfigurationProvider<TMachine>` where `TMachine` is a strongly-typed machine class derived from or compatible with the `Machine` type:

```csharp
using Overseer.Server.Integration.Machines;

public class CustomMachineConfigurationProvider(ICustomApiClient customApiClient) : IMachineConfigurationProvider<CustomMachine>
{
  async Task<CustomMachine> Configure(Machine machine) {
    var machineInfo = await customApiClient.GetMachineInfo();

    return new CustomMachine {
      // if creating a new instance make sure to provide the providers that will be set by Overseer
      Name = machine.Name,
      MachineType = machine.MachineType,
      Disabled = machine.Disabled,
      SortIndex = machine.SortIndex,
      Properties = machine.Properties,
      //...set tools, webcam, etc.
    }
  }
}
```

### 3) Implement the `IMachineProvider<TMachine>` interface

Create a class that implements `IMachineProvider<TMachine>` where `TMachine` is a strongly-typed machine class derived from or compatible with the `Machine` type:

```csharp
// CustomMachineProvider.cs
using Overseer.Server.Integration.Machines;

public class CustomMachineProvider(ICustomApiClient customApiClient) : IMachineProvider<CustomMachine>
{
  private System.Timers.Timer _timer;

  // This event handler is how status updates are sent to Overseer
  public event EventHandler<MachineStatusEventArgs> StatusUpdated;

  private CustomMachine Machine { get; set; }

  public Task PauseJob()
  {
    return customApiClient.Pause();
  }

  public Task ResumeJob()
  {
    return customApiClient.Resume();
  }

  public Task CancelJob()
  {
    return customApiClient.Cancel();
  }

  public void Start(int interval, CustomMachine machine)
  {
    // It's recommended to capture the machine instance
    Machine = machine;

    // Start monitoring the machine state.
    //
    // The interval parameter represents the polling rate in seconds.
    // If the provider polls for updates it should do it at this interval.
    //
    // If the provider supports real-time communication (WebSocket, MQTT, etc.), send updates as they arrive.
    // However, when the machine is idle and no updates are incoming, periodically emit an update
    // to prevent Overseer from marking the machine as offline.
    _timer?.Dispose();
    _timer = new System.Timers.Timer(interval);
    _timer.Elapsed += async (sender, e) =>
    {
      var status = await customApiClient.GetMachineStatusAsync();
      StatusUpdated?.Invoke(this, new MachineStatusEventArgs(status));
    };
    _timer.Start();
  }

  public void Stop()
  {
    // Should stop monitoring and clean up any resources
    _timer.Dispose();
    _timer = null;
  }
}
```

### 4) Implement `IPluginConfiguration` to register services

Create a class that implements `IPluginConfiguration` to register your machine provider factory function:

```csharp
// PluginConfiguration.cs
using Microsoft.Extensions.DependencyInjection;
using Overseer.Server.Integration;

public class CustomMachinePluginConfiguration : IPluginConfiguration
{
  public void ConfigureServices(IServiceCollection services)
  {
    // register any dependencies for your provider
    services.AddTransient<ICustomApiClient, CustomApiClient>();

    // This is a short lived service and is only used for creating and updating machine through the UI
    services.AddTransient<IMachineConfigurationProvider<CustomMachine>, CustomMachineConfigurationProvider>();

    // This is a longed lived service and is used for receiving statuses from and sending commands to the machine
    // DI will be used to create the instance so any dependencies are provided
    // but Overseer will hold on to an instance of this as long as there is an enabled machine configured for the provider
    services.AddTransient<IMachineProvider<CustomMachine>, CustomMachineProvider>();
  }
}
```

> Note: A user can have multiple machines of the same type, so design your provider to efficiently support multiple provider instances.

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
  [MachineProperty(DisplayName = "API Key", DisplayType = MachinePropertyDisplayType.SetupOnly)]
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
