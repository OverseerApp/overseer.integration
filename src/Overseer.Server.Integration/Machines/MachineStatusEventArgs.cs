namespace Overseer.Server.Integration.Machines;

public class MachineStatusEventArgs(MachineStatus status) : EventArgs
{
    public MachineStatus Status { get; } = status;
}
