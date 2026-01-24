
namespace Overseer.Server.Integration.Automation;

public record FailureAnalysisResult
{
  public bool IsFailureDetected { get; set; }
  public double ConfidenceScore { get; set; }
  public string? FailureReason { get; set; }
  public string? Details { get; set; }
}
