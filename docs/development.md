# Development Guide

## Plugins — How to build one ✅

This section describes how to create a plugin for Overseer. Currently there is one plugin *type* (the `IFailureDetectionAnalyzer`), but the plugin system is designed to be generic and expandable: the app discovers implementations of `IPluginConfiguration` and calls them to register plugin services and any dependencies they require.

### Goals

- Keep plugin code isolated in a class library.
- Your plugin should be in the Overseer namespace.
  - This is so your assemblies are prefixed with "Overseer", E.g. "Overseer.Your.Plugin.Namespace.dll"
- Implement `IPluginConfiguration` to configure and register your plugin's services.
- Register your plugin's public plugin interfaces (for example, `IFailureDetectionAnalyzer`) and any supporting dependencies in `ConfigureServices`.

---

### Example: failure detection analyzer plugin 🔧

This example shows the minimal pieces you typically need to provide.

1) Implement the plugin interface(s):

```csharp
// MyAnalyzer.cs
public class MyAnalyzer : IFailureDetectionAnalyzer
{
  public void Start(string url) { /* ... */ }
  public void Stop() { /* ... */ }
  public FailureDetectionAnalysisResult Analyze() { /* ... */ }
}
```

2) Implement `IPluginConfiguration` to register the analyzer and any dependencies:

```csharp
// PluginConfiguration.cs
using Microsoft.Extensions.DependencyInjection;
using Overseer.Server.Integration;

public class PluginConfiguration : IPluginConfiguration
{
  public void ConfigureServices(IServiceCollection services)
  {
    // Optionally register other dependencies required by your plugin
    // services.AddSingleton<IMyDependency, MyDependencyImplementation>();
    
    // Register the plugin implementation for the plugin interface
    services.AddTransient<IFailureDetectionAnalyzer, MyAnalyzer>();    
  }
}
```

> Note: The host will discover `IPluginConfiguration` implementations (for example via reflection) and invoke `ConfigureServices` when composing the application.

---

### Best practices & tips 💡

- Keep plugin behaviour focused: try to register only the public plugin interfaces and the minimum dependencies needed.
- Prefer constructor-injected dependencies; register interfaces rather than concrete classes when appropriate.
- Make your plugin classes `public` so they can be discovered and instantiated by the host.
- If your plugin needs configuration, use the host's existing configuration mechanisms (e.g., `IOptions<T>`) and document the configuration keys your plugin expects.
- Avoid side effects in `ConfigureServices`; keep service registrations deterministic.

---

### Future plugin types

Design your `IPluginConfiguration` registrations to be generic — register implementations against abstractions (interfaces) rather than concrete types. This allows the host to support multiple plugin interfaces over time without requiring plugin authors to change their registration approach.

---

### Quick checklist before publishing a plugin 📋

- [ ] Project is a class library targeting a compatible framework
- [ ] All plugin types are `public`
- [ ] `IPluginConfiguration` implemented and registers services
- [ ] Dependencies are registered and documented
- [ ] Any required configuration keys are documented

---

If you need a reference implementation please view the [Overseer Print Guard](https://github.com/OverseerApp/overseer.print-guard) repository.
