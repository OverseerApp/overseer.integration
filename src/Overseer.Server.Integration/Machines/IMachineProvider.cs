using System;

namespace Overseer.Server.Integration.Machines;

/// <summary>
/// Interface for machine providers, which are responsible for managing the state and operations of machines in the system.
/// This interface should not be implemented directly, instead implement the generic IMachineProvider<TMachine> interface to provide a strongly
/// typed machine instance.
/// </summary>
public interface IMachineProvider
{
    /// <summary>
    /// Event that is raised whenever the status of the machine is updated. The event args will contain the new status of the machine.
    /// </summary>
    event EventHandler<MachineStatusEventArgs> StatusUpdated;

    /// <summary>
    /// The name of the machine type that this provider supports.
    /// This should be a string that uniquely identifies the type of machine, such as "Prusa", "OctoPrint", "Bambu", etc.,
    /// and should match the MachineType property of the Machine instances that this provider is responsible for.
    /// </summary>
    string MachineType { get; }

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
    /// This method is called when the user creates or updates a machine and is responsible for configuring the tools for the machine.
    /// </summary>
    Task Configure(Machine machine);

    /// <summary>
    /// Called by the server to start the provider
    /// </summary>
    /// <param name="interval">The polling interval in milliseconds</param>
    void Start(int interval);

    /// <summary>
    /// Called by the server to stop the provider.
    /// </summary>
    void Stop();
}

/// <summary>
/// Generic version of the IMachineProvider interface, which provides a strongly typed Machine instance for the provider to manage.
/// </summary>
/// <typeparam name="TMachine"></typeparam>
public interface IMachineProvider<out TMachine> : IMachineProvider
    where TMachine : Machine, new()
{
    /// <summary>
    /// The machine instance that this provider is responsible for managing. This should be a strongly typed instance
    /// of the machine that this provider supports.
    /// </summary>
    TMachine Machine { get; }
}
