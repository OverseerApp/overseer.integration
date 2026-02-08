using System.Text.Json.Serialization;

namespace Overseer.Server.Integration.Machines;

[AttributeUsage(AttributeTargets.Property)]
public class MachinePropertyAttribute(
  string? displayName = null,
  MachinePropertyDisplayType displayType = MachinePropertyDisplayType.Both,
  string? description = null,
  bool isRequired = false,
  bool isSensitive = false,
  bool isIgnored = false
) : Attribute
{
  /// <summary>
  /// The display name of the property, used by the client when displaying the property to the user.
  /// This should be a user-friendly name that describes the property.
  /// </summary>
  public string? DisplayName { get; } = displayName;

  /// <summary>
  /// The display type of the property, used by the client to determine when to display the property to the user.
  /// </summary>
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public MachinePropertyDisplayType DisplayType { get; } = displayType;

  /// <summary>
  /// A description of the property, used by the client to provide additional information about the property to the user.
  /// This can be used to explain what the property is for, what kind of values it expects, or any other relevant information.
  /// </summary>
  public string? Description { get; } = description;

  /// <summary>
  /// Whether the property is required. If true, the client should enforce that a value is provided for this property when creating or editing
  ///  a machine.
  /// </summary>
  public bool IsRequired { get; } = isRequired;

  /// <summary>
  /// Whether the property is sensitive. If true, the client should take extra precautions when displaying and editing this property,
  ///  such as masking the value or requiring additional confirmation before allowing the value to be edited.
  /// </summary>
  public bool IsSensitive { get; } = isSensitive;

  /// <summary>
  /// Whether the property should be ignored by the client. If true, the client should not display or allow editing of this property.
  /// </summary>
  /// <remarks>
  /// This can be used to store properties that are only relevant to the provider and should not be exposed to the user,
  /// such as internal identifiers or metadata.
  /// </remarks>
  public bool IsIgnored { get; } = isIgnored;
}
