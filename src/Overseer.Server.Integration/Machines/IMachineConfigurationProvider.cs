namespace Overseer.Server.Integration.Machines;

/// <summary>
/// The IMachineConfigurationProvider interface is responsible for configuring a machine when it is added to the system.
/// This allows the provider to set up any necessary information about the machine, such as its tools, capabilities, and
/// other relevant data that may be collected from the machine. The Configure method will be called by the server when a
/// new machine is added, and it should return a fully configured instance of the machine that can be used by the
/// IMachineProvider to manage the machine's status and operations.
///
/// Instances of the IMachineConfigurationProvider are expected to be short-lived and only used during the configuration process of a machine.
/// It will be instantiated and invoked when adding and updating machines.
/// </summary>
public interface IMachineConfigurationProvider<TMachine>
  where TMachine : Machine, new()
{
  /// <summary>
  /// When the user add a new machine the system will call this method to allow the provider to configure the machine.
  /// This should be used to configure the tools and set any other machine specific information that may be collected
  /// from the machine.
  /// </summary>
  Task<TMachine> Configure(Machine machine);
}

public record TestMachine : Machine
{
  public string TestProperty { get; set; } = string.Empty;
}

public class TestMachineConfigurationProvider : IMachineConfigurationProvider<TestMachine>
{
  public Task<TestMachine> Configure(Machine machine)
  {
    var testMachine = new TestMachine
    {
      Id = machine.Id,
      Name = machine.Name,
      MachineType = machine.MachineType,
      Disabled = machine.Disabled,
      WebcamUrl = machine.WebcamUrl,
      WebcamOrientation = machine.WebcamOrientation,
      Tools = machine.Tools,
      SortIndex = machine.SortIndex,
      Properties = machine.Properties,
      TestProperty = "This is a test property",
    };

    return Task.FromResult(testMachine);
  }
}
