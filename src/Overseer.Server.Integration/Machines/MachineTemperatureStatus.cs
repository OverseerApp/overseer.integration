namespace Overseer.Server.Integration.Machines;

public class MachineTemperatureStatus
{
    public int HeaterIndex { get; set; }

    public double Actual { get; set; }

    public double Target { get; set; }
}
