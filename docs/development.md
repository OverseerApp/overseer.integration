# Development Guide

## Building Plugins for Overseer

This workspace contains the core integration interfaces for building Overseer plugins. The plugin system is designed to be generic and expandable: the app discovers implementations of `IPluginConfiguration` and calls them to register plugin services and any dependencies they require.

### Plugin Architecture

All plugins follow a common pattern:

1. **Implement a plugin interface** — Define the functionality your plugin provides (e.g., `IMachineProvider`, `IFailureDetectionAnalyzer`)
2. **Implement `IPluginConfiguration`** — Register your plugin services and dependencies with the dependency injection container
3. **Package as a class library** — Your plugin should be in the Overseer namespace so assemblies are prefixed with "Overseer"

---

## Implementing `IPluginConfiguration`

Every plugin must implement `IPluginConfiguration` to register its services with the host application.

### Basic Structure

```csharp
// PluginConfiguration.cs
using Microsoft.Extensions.DependencyInjection;
using Overseer.Server.Integration;

public class PluginConfiguration : IPluginConfiguration
{
  public void ConfigureServices(IServiceCollection services)
  {
    // Register your plugin implementation(s)
    services.AddTransient<IPluginInterface, YourPluginImplementation>();
    
    // Optionally register dependencies required by your plugin
    // services.AddSingleton<IMyDependency, MyDependencyImplementation>();
    // services.AddHttpClient<IMyApiClient, MyApiClient>();
  }
}
```

> **Note:** The host will discover `IPluginConfiguration` implementations via reflection and invoke `ConfigureServices` when composing the application.

### Service Lifetimes

Choose the appropriate service lifetime for your registrations:

- **`AddTransient`** — A new instance is created each time it's requested. Use for lightweight, stateless services.
- **`AddScoped`** — A new instance is created per scope (typically per request). Use when you need request-specific state.
- **`AddSingleton`** — A single instance is shared across the application lifetime. Use for stateless services or shared resources.

### Best Practices

- Keep plugin behavior focused: register only the public plugin interfaces and minimum dependencies needed
- Prefer constructor-injected dependencies; register interfaces rather than concrete classes when appropriate
- Make your plugin classes `public` so they can be discovered and instantiated by the host
- Avoid side effects in `ConfigureServices`; keep service registrations deterministic
- Register implementations against abstractions (interfaces) rather than concrete types

---

## Plugin Types

Overseer supports multiple plugin types. See the specific guides for detailed implementation instructions:

### [Failure Detection Analyzers](failure-detection-analyzer.md)

Implement `IFailureDetectionAnalyzer` to create plugins that monitor print jobs and detect failures in real-time.

### [Machine Providers](machine-provider-development.md)

Implement `IMachineProvider<TMachine>` to add support for new machine types (3D printers, CNC machines, etc.).

---

## Quick Checklist Before Publishing 📋

- [ ] Project is a class library targeting a compatible framework
- [ ] All plugin types are `public`
- [ ] `IPluginConfiguration` implemented and registers services
- [ ] Dependencies are registered and documented
- [ ] Plugin is in the Overseer namespace

---

## Reference Implementations

For reference implementations, please view existing plugins on the [OverseerApp GitHub organization](https://github.com/OverseerApp).
