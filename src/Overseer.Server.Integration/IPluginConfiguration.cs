using Microsoft.Extensions.DependencyInjection;

namespace Overseer.Server.Integration;

/// <summary>
/// Implement this interface in your plugin to configure services and register you plugin with Overseer.
/// </summary>
public interface IPluginConfiguration
{
  /// <summary>
  /// Configures services for the plugin. Allows for developers to add custom services
  /// to the dependency injection container.
  /// </summary>
  /// <param name="services">The service collection to which services can be added.</param>
  void ConfigureServices(IServiceCollection services);
}
