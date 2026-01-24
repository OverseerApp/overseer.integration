# Releasing an Overseer Server Plugin

This guide explains how to release your Overseer server plugin and make it available to other users.

## Prerequisites

Before releasing your plugin, make sure you have developed and tested it thoroughly. See [development.md](development.md) for information about developing a plugin.

You will need a GitHub account, as your plugin must exist in a GitHub repository.


## Plugin Release Structure

You can use the [Overseer Print Guard](https://github.com/OverseerApp/overseer.print-guard/blob/main/.github/workflows/release.yml) plugin as an example of how to automate your build process.

Below are the steps required to build and release your plugin with the correct build configuration and format.

### Building Your Plugin

1. Build your C# library in production mode:
   ```bash
   dotnet build -c Release
   ```

2. The build output will be located in your project's `bin/Release` directory.

### Creating a Release Package

1. Create a zip archive containing the build output from your plugin.
2. Place all the build artifacts in the **root** of the zip archive (not in a subdirectory).

### Publishing on GitHub

1. Create a new GitHub release in your plugin's repository.
2. Add the zip archive as an artifact to the release.
3. Use semantic versioning for your release tag (e.g., `v1.0.0`).


## Adding to the Plugin Registry

To make your plugin discoverable and available in Overseer:

1. Create a pull request in the [Overseer Plugin Registry](https://github.com/OverseerApp/overseer.plugin-registry) repository.
2. Follow the registry's guidelines for adding your plugin metadata.
3. Once your PR is merged, your plugin will be available in Overseer for all users.
