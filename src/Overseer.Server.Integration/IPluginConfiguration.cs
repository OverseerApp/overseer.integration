using Microsoft.Extensions.DependencyInjection;

namespace Overseer.Server.Integration;

/// <summary>
/// Optional interface for plugin configuration.
/// Implement this interface in your plugin to configure services.
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