using System.Text.Json.Serialization;

namespace Overseer.Server.Integration.Machines;

public class MachineStatus
{
    /// <summary>
    /// A unique identifier for this status update, used for tracking and correlation purposes.
    /// This is not the id of the machine, but rather a unique id for this specific status update.
    /// It can be used by clients to determine if a status update is new or if it has already been processed.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The id of the configured machine that this status is for
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// The current state of the machine
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MachineState State { get; set; }

    /// <summary>
    /// The total amount of time the machine has been operational
    /// </summary>
    public int ElapsedJobTime { get; set; }

    /// <summary>
    /// The estimated time remaining for a job
    /// </summary>
    /// <remarks>
    public int EstimatedTimeRemaining { get; set; }

    /// <summary>
    /// The percentage of completion
    /// </summary>
    public double Progress { get; set; }

    /// <summary>
    /// The current temperatures for each heater, the key is the heater index defined by the machine.
    /// </summary>
    public Dictionary<int, MachineTemperatureStatus> Temperatures { get; set; } = [];

    public override bool Equals(object? obj)
    {
        if (obj is not MachineStatus other)
            return false;

        return MachineId == other.MachineId
            && State == other.State
            && ElapsedJobTime == other.ElapsedJobTime
            && EstimatedTimeRemaining == other.EstimatedTimeRemaining
            && Progress.Equals(other.Progress)
            && Temperatures.SequenceEqual(other.Temperatures);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + MachineId.GetHashCode();
            hash = hash * 23 + State.GetHashCode();
            hash = hash * 23 + ElapsedJobTime.GetHashCode();
            hash = hash * 23 + EstimatedTimeRemaining.GetHashCode();
            hash = hash * 23 + Progress.GetHashCode();
            foreach (var kvp in Temperatures)
            {
                hash = hash * 23 + kvp.Key.GetHashCode();
                hash = hash * 23 + (kvp.Value?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }
}
