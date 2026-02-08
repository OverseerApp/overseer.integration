namespace Overseer.Server.Integration.Common;

/// <summary>
/// Throw the OverseerException when you want to return an error message to the user. The message will be displayed in the UI
/// and the properties can be used to provide additional context for the error. The Unwrap method can be used to extract the
/// OverseerException from an exception chain, allowing you to throw it directly without losing the original message and properties.
/// </summary>
/// <param name="message">The message that will be displayed to the user.</param>
/// <param name="properties">Additional properties providing context for the error.</param>
public class OverseerException(string message, object? properties = null) : Exception(message)
{
  public object? Properties { get; set; } = properties;

  public static void Unwrap(Exception ex)
  {
    if (ex is OverseerException)
      throw ex;

    var innerEx = ex.InnerException;
    while (innerEx != null)
    {
      if (innerEx is OverseerException)
        throw innerEx;
      innerEx = innerEx.InnerException;
    }
  }
}
