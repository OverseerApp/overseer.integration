namespace Overseer.Server.Integration.Machines;

public class MachineTemperatureStatus
{
    public int HeaterIndex { get; set; }

    public double Actual { get; set; }

    public double Target { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not MachineTemperatureStatus other)
            return false;

        return HeaterIndex == other.HeaterIndex
            && Actual.Equals(other.Actual)
            && Target.Equals(other.Target);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(HeaterIndex, Actual, Target);
    }
}
