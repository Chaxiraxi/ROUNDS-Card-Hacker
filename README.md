# ROUNDS Cheat - BepInEx Plugin

A comprehensive cheat mod for ROUNDS built as a BepInEx plugin with Harmony patches for runtime modifications.

## Features

### Aimbot
- **Toggle Key**: E (configurable)
- **Predictive Aiming**: Automatically aims at opponents with prediction
- **Safe Implementation**: Uses Harmony patches for safe runtime access

### Autoblock
- **Toggle Key**: Q (configurable)  
- **Automatic Blocking**: Blocks incoming projectiles within range
- **Distance Control**: Configurable block distance (default: 1.4f)

### Stat Modifications
- **Health Boost**: C key (configurable) - Add health to your character
- **Respawn Boost**: V key (configurable) - Add extra respawns
- **Configurable Increments**: Adjust how much health to add per press

### GUI System
- **Settings Menu**: F1 key to open/close the cheat menu
- **Real-time Configuration**: Adjust all settings without restarting
- **Status Overlays**: Shows active cheat states on screen

## Installation

1. **Install BepInEx** for ROUNDS if you haven't already
2. **Build the plugin**:
   ```powershell
   dotnet build
   ```
3. **Copy the DLL** from `bin/Debug/net472/` or `bin/Release/net472/` to your BepInEx plugins folder:
   ```
   ROUNDS/BepInEx/plugins/ROUNDS-Cheat.dll
   ```

## Usage

1. **Start ROUNDS** - The plugin will load automatically
2. **Press F1** to open the cheat menu
3. **Configure settings** in the GUI or use default hotkeys:
   - **E**: Toggle Aimbot
   - **Q**: Toggle Autoblock  
   - **C**: Add Health (when stat mods enabled)
   - **V**: Add Respawns (when stat mods enabled)

## Configuration

All settings can be adjusted in real-time through the F1 menu:

- **Aimbot Prediction Multiplier**: How much to lead targets (0.1 - 3.0)
- **Autoblock Distance**: How close bullets must be to trigger block (0.5 - 5.0)
- **Health Increment**: How much health to add per keypress (10 - 200)
- **Toggle Keys**: Currently displayed but not changeable in GUI
- **Status Overlays**: Enable/disable on-screen status indicators

## Architecture

This mod uses a modern BepInEx plugin architecture:

### Core Components
- **ROUNDSCheatPlugin**: Main BepInEx plugin entry point
- **CheatManager**: Coordinates all cheat components
- **Settings**: Centralized configuration system
- **Individual Components**: AimbotComponent, AutoblockComponent, StatModsComponent, CheatGUI

### Harmony Patches
- **PlayerManagerPatch**: Safe player access methods
- **SafeMethodPatches**: Runtime method access for velocity, actions, blocking
- **Reflection-based Access**: Safely accesses private/internal game methods

### Safety Features
- **Exception Handling**: All game interactions wrapped in try-catch
- **Null Checking**: Extensive null checking to prevent crashes  
- **Graceful Degradation**: Continues working even if some game methods fail
- **Logging**: Comprehensive logging for debugging

## Technical Details

- **Framework**: .NET Framework 4.7.2
- **Dependencies**: BepInEx, Harmony, Unity assemblies
- **Language**: C# with latest language features
- **Unsafe Code**: Enabled for P/Invoke mouse operations (currently unused)

## Development

The plugin follows the pattern established in your Achaman mod:

1. **Harmony Patches** for runtime method interception
2. **Reflection** for accessing private/internal members
3. **Component-based Architecture** for modular functionality
4. **Centralized Settings** with real-time configuration
5. **Comprehensive Error Handling** for stability

## Differences from Original

- **BepInEx Plugin**: Proper plugin structure instead of manual injection
- **Harmony Integration**: Uses patches instead of direct method calls
- **Settings System**: Centralized, configurable settings
- **GUI System**: Full settings menu instead of just overlays
- **Error Handling**: Robust error handling and logging
- **Modular Design**: Components can be enabled/disabled independently

## Troubleshooting

- **Plugin not loading**: Check BepInEx console for error messages
- **Features not working**: Check if the correct game assemblies are referenced
- **Crashes**: Check the logs in `BepInEx/LogOutput.log`
- **Missing references**: Ensure all game DLLs are in the correct paths in the .csproj file
