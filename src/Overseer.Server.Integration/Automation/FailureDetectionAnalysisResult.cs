
namespace Overseer.Server.Integration.Automation;

/// <summary>
/// Result of failure detection analysis.
/// </summary>
public record FailureDetectionAnalysisResult
{
  public bool IsFailureDetected { get; set; }
  public double ConfidenceScore { get; set; }
  public string? FailureReason { get; set; }
  public string? Details { get; set; }
}
