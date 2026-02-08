namespace Overseer.Server.Integration.Machines;

public delegate IMachineProvider<TMachine>? MachineProviderFactory<TMachine, TMachineProvider>(TMachine machine)
  where TMachine : Machine, new()
  where TMachineProvider : IMachineProvider<TMachine>;
