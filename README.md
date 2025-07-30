# ROUNDS Card Hacker - BepInEx Plugin

A client-side cheat for ROUNDS that allows you to select and force specific cards to spawn during card selection, overriding the default random selection.

## Features

### Card Selection Override
- **Force Card Spawning**: Select any card from the complete card pool to guarantee it appears in your next card choice
- **Smart Positioning**: Selected cards spawn at a random position (1-5) in the card choice to avoid detection
- **Auto-Clear Option**: Automatically clear selection after use, or keep it active for multiple uses

### Comprehensive Card Browser
- **Complete Card Database**: Access to all cards in the game, not just those that would normally appear
- **Advanced Search**: Search by card name, description, stats, or rarity
- **Smart Filtering**: Relevance-based search results with intelligent scoring
- **Card Details**: View card descriptions, stats, and rarity information

### User-Friendly GUI
- **F1 Toggle**: Press F1 to open/close the card selection interface
- **Resizable Window**: Drag to resize the interface to your preference
- **Smooth Scrolling**: Efficiently browse through hundreds of cards
- **Real-time Updates**: Refresh card list without restarting the game

## Installation

1. **Install BepInEx** for ROUNDS if you haven't already
2. **Download the latest release** from the [Releases](../../releases) page, or build from source:
   ```powershell
   dotnet build
   ```
3. **Copy the DLL** to your BepInEx plugins folder:
   ```
   ROUNDS/BepInEx/plugins/ROUNDS-Cheat.dll
   ```
4. **Launch ROUNDS** - The plugin will load automatically

## Usage

1. **Start a game** and wait for a card choice to appear
2. **Press F1** to open the card selection interface
3. **Search or browse** for the card you want
4. **Click on a card** to select it (it will be highlighted in cyan)
5. **Close the interface** (F1 or Close button)
6. **Wait for the next card choice** - your selected card will appear at a random position

### Tips
- Enable "Auto-clear selection after card choice" to automatically deselect after one use
- Disable auto-clear to keep the same card selected for multiple rounds
- Use the search function to quickly find specific cards by name, description, or stats
- Click "Refresh Card List" if cards don't appear initially

## Configuration

- **Auto-clear Selection**: Toggle whether selected cards are automatically cleared after use
- **Search Filter**: Real-time search across card names, descriptions, stats, and rarity
- **Window Size**: Resizable interface - drag the bottom-right corner to resize

## Architecture

This mod uses a modern BepInEx plugin architecture with Harmony patches:

### Core Components
- **ROUNDSCheatPlugin**: Main BepInEx plugin entry point and lifecycle management
- **CardSelectionGUI**: Complete card browser interface with search and selection
- **CardChoiceModifier**: Harmony patches that intercept card spawning

### Harmony Patches
- **CardChoice.Spawn Prefix**: Intercepts card spawning to inject selected cards
- **CardChoice.SpawnUniqueCard Postfix**: Fixes source card references for proper game integration
- **Smart Positioning**: Randomizes position of forced cards to avoid detection patterns

### Safety Features
- **Exception Handling**: All game interactions wrapped in try-catch blocks
- **Null Checking**: Extensive validation to prevent crashes
- **Graceful Degradation**: Continues working even if some game systems fail
- **Comprehensive Logging**: Detailed logging for debugging and monitoring

## Technical Details

- **Framework**: .NET Framework 4.7.2 (Unity compatibility)
- **Dependencies**: BepInEx 5.x, Harmony 2.x, Unity assemblies
- **Language**: C# with modern language features
- **Game Integration**: Non-intrusive Harmony patches with minimal performance impact

## How It Works

1. **Card Database Access**: Captures the complete card array when `CardChoice` initializes
2. **User Selection**: GUI allows browsing and selecting from the complete card database
3. **Spawn Interception**: Harmony prefix patch intercepts `CardChoice.Spawn` calls
4. **Smart Injection**: Replaces one of the 5 cards at a random position with the selected card
5. **Reference Fixing**: Postfix patch ensures the source card reference is correct for game systems

## Troubleshooting

### Common Issues

**Plugin not loading**
- Check BepInEx console for error messages
- Ensure BepInEx is properly installed for ROUNDS
- Verify the DLL is in the correct plugins folder

**Cards not appearing in the browser**
- Start a game and wait for a card choice to appear first
- Click "Refresh Card List" in the interface
- Check BepInEx logs for any error messages

**Selected card not spawning**
- Ensure a card is actually selected (highlighted in cyan)
- Wait for the next natural card choice - it won't work on the current one
- Check that auto-clear is configured as desired

**GUI not opening**
- Press F1 to toggle the interface
- Check if other mods might be conflicting with the F1 key
- Look for error messages in the BepInEx console

### Debug Information

Check these log files for detailed information:
- `BepInEx/LogOutput.log` - General BepInEx and plugin logs
- Look for entries prefixed with `[Info   :CardHacker]` for plugin-specific messages

### Performance

This mod has minimal performance impact:
- GUI only renders when visible (F1 toggle)
- Card list virtualization for smooth scrolling
- Harmony patches only execute during card choices
- No continuous background processing
