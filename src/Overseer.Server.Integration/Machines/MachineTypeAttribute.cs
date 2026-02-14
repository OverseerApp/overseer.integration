namespace Overseer.Server.Integration.Machines;

/// <summary>
/// The type of the machine, used to determine which provider should be used to load the machine. This should be a string that uniquely
/// identifies the type of machine, such as "Prusa", "OctoPrint", "Bambu", etc.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MachineTypeAttribute(string name) : Attribute
{
  public string Name { get; } = name;
}
