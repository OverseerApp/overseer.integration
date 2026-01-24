# Overseer Integration

This repository contains code for integrating with [Overseer](https://github.com/michaelfdeberry/overseer), a utility that allows for monitoring multiple 3d printers.
 
## Overseer.Server.Integration

This package provides server-side integration utilities for Overseer. 

### Installation

Install the NuGet package from the GitHub registry:

```bash
dotnet add package Overseer.Server.Integration
```

#### Configuring GitHub Packages

To use packages from the GitHub registry, you need to configure NuGet with your GitHub credentials:

1. **Create a GitHub Personal Access Token (PAT)**:
   - Go to GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)
   - Click "Generate new token"
   - Give it at least `read:packages` scope
   - Copy the token

2. **Add the GitHub feed to NuGet**:

   ```powershell
   dotnet nuget add source https://nuget.pkg.github.com/OverseerApp/index.json `
     -n overseer `
     -u YOUR_GITHUB_USERNAME `
     -p YOUR_PAT `
     --store-password-in-clear-text
   ```

   Replace `YOUR_GITHUB_USERNAME` with your GitHub username and `YOUR_PAT` with your personal access token.

   **Note:** The `--store-password-in-clear-text` flag is required for .NET on Windows with GitHub Packages.