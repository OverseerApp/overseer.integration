namespace Overseer.Server.Integration.Machines;

public interface IMachineProvider
{
  /// <summary>
  /// Event that is raised whenever the status of the machine is updated. The event args will contain the new status of the machine.
  /// </summary>
  event EventHandler<MachineStatusEventArgs>? StatusUpdated;

  /// <summary>
  /// Pauses the current job on the machine.
  /// </summary>
  Task PauseJob();

  /// <summary>
  /// Resumes a paused job on the machine.
  /// </summary>
  Task ResumeJob();

  /// <summary>
  /// Cancels the current job on the machine.
  /// </summary>
  Task CancelJob();

  /// <summary>
  /// Called by the server to start the provider
  ///
  /// The instance create to monitoring the machine will be long lived and can be used to maintain any state related to the machine,
  /// such as the current job status, tool information, etc. This instance will be passed to the provider on each call to Start,
  /// so it can be used to maintain state across multiple calls to Start and Stop.
  /// </summary>
  /// <param name="interval">The polling interval in milliseconds</param>
  void Start<TMachine>(int interval, TMachine machine)
    where TMachine : Machine, new();

  /// <summary>
  /// Called by the server to stop the provider.
  /// </summary>
  void Stop();
}

/// <summary>
/// The IMachineProvider interface is responsible for managing the status and operations of a machine.
/// It provides methods to pause, resume, and cancel jobs on the machine, as well as an event that is raised
/// whenever the status of the machine is updated. The Start method is called by the server to start the provider,
/// and it can be used to maintain any state related to the machine. The Stop method is called by the server to stop the provider.
///
/// Instances of the IMachineProvider are expected to be long-lived and can maintain state across multiple calls to Start and Stop.
/// </summary>
public interface IMachineProvider<TMachine> : IMachineProvider
  where TMachine : Machine, new() { }
