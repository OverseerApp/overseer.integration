# Failure Detection Analyzer Plugin Guide

This guide describes how to create a failure detection analyzer plugin for Overseer. Failure detection analyzers monitor print jobs and detect potential failures in real-time using video analysis, sensor data, or other detection methods.

## Overview

A failure detection analyzer plugin implements the `IFailureDetectionAnalyzer` interface, which provides methods for:
- Starting analysis on a video stream or machine
- Stopping analysis
- Retrieving analysis results

---

## Creating a Failure Detection Analyzer

### 1) Implement the `IFailureDetectionAnalyzer` interface

Create a class that implements `IFailureDetectionAnalyzer`:

```csharp
// MyAnalyzer.cs
using Overseer.Server.Integration.Automation;

public class MyAnalyzer : IFailureDetectionAnalyzer
{
  public void Start(string url)
  {
    // Start analyzing the video stream or machine at the given URL
    // This might involve:
    // - Connecting to a webcam stream
    // - Starting background processing
    // - Initializing ML models
  }
  
  public void Stop()
  {
    // Stop the analysis and clean up resources
    // This might involve:
    // - Disconnecting from streams
    // - Stopping background tasks
    // - Releasing resources
  }
  
  public FailureDetectionAnalysisResult Analyze()
  {
    // Perform analysis and return the result
    // Return a FailureDetectionAnalysisResult indicating:
    // - Whether a failure was detected
    // - Confidence level
    // - Any additional details
    
    return new FailureDetectionAnalysisResult
    {
      FailureDetected = false,
      Confidence = 0.95,
      Details = "Print appears normal"
    };
  }
}
```

### 2) Implement `IPluginConfiguration`

Register your analyzer and any dependencies:

```csharp
// PluginConfiguration.cs
using Microsoft.Extensions.DependencyInjection;
using Overseer.Server.Integration;

public class PluginConfiguration : IPluginConfiguration
{
  public void ConfigureServices(IServiceCollection services)
  {
    // Register dependencies your analyzer needs
    // services.AddSingleton<IImageProcessor, MyImageProcessor>();
    // services.AddHttpClient<IStreamClient, MyStreamClient>();
    
    // Register the analyzer implementation
    services.AddTransient<IFailureDetectionAnalyzer, MyAnalyzer>();
  }
}
```

---

## Analysis Lifecycle

Your analyzer follows this lifecycle:

1. **`Start(string url)`** — Called when a print job begins. Initialize your analyzer and begin monitoring the provided stream URL.
2. **`Analyze()`** — Called periodically during the print job to check for failures. Return the current analysis state.
3. **`Stop()`** — Called when the print job ends or is cancelled. Clean up resources and stop monitoring.

### Threading Considerations

- `Start()` may initialize background tasks or threads for continuous analysis
- `Analyze()` should be thread-safe and return quickly with the current state
- `Stop()` should gracefully shut down any background processing

---

## Analysis Results

The `FailureDetectionAnalysisResult` type provides:

- **`FailureDetected`** (bool) — Whether a failure was detected
- **`Confidence`** (double) — Confidence level of the detection (0.0 to 1.0)
- **`Details`** (string) — Additional information about the detection or analysis state

### Result Guidelines

- Return `FailureDetected = true` only when you have reasonable confidence a failure occurred
- Use `Confidence` to indicate certainty (e.g., 0.95 = 95% confident)
- Provide helpful `Details` for debugging and user feedback
- Return consistent results quickly from `Analyze()` to avoid blocking

---

## Best Practices

- **Performance**: Keep `Analyze()` fast; do heavy processing in background tasks
- **Resource management**: Properly clean up resources in `Stop()`
- **Error handling**: Gracefully handle network issues, stream interruptions, and errors
- **Thread safety**: Ensure `Analyze()` can be called safely while background analysis runs
- **Logging**: Log important events and errors for debugging
- **Configuration**: Make thresholds and settings configurable when possible

---

## Quick Checklist 📋

- [ ] `IFailureDetectionAnalyzer` interface fully implemented
- [ ] `Start()` initializes analysis and handles invalid URLs gracefully
- [ ] `Analyze()` returns quickly and is thread-safe
- [ ] `Stop()` properly cleans up all resources
- [ ] Analysis results provide meaningful confidence and details
- [ ] Background tasks use cancellation tokens for clean shutdown
- [ ] Error handling is robust and logged appropriately
- [ ] `IPluginConfiguration` registers the analyzer and dependencies

---

## Reference Implementation

For a complete reference implementation, see the [Overseer Print Guard](https://github.com/OverseerApp/overseer.print-guard) repository.
