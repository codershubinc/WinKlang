# Changelog

All notable changes to this project will be documented in this file.

## [0.0.1-beta] - 2025-09-02

### Added

- **Core Media Control Integration**: Windows Media Session Manager integration for accessing currently playing media
- **Timeline Information System**:
  - `TimelineInfo` class with structured timeline data
  - Real-time position tracking with formatted time display (`mm:ss`)
  - Progress percentage calculation
  - Raw position data in milliseconds and TimeSpan format
- **Artwork Management System**:
  - `SaveArtworkAsync()` method for saving album artwork to files
  - `GetArtworkBufferAsync()` method for retrieving artwork as byte arrays
  - Automatic filename sanitization with artist and title
  - Support for various image formats
- **Optimized Build Configuration**:
  - Single-file deployment with compression (~11MB executable)
  - Full trimming enabled for minimal size
  - .NET 9.0 targeting with Windows 10.0.17763.0 support
  - Self-contained deployment option

### Features

- **Media Information Display**: Shows title, artist, album, and playback status
- **Timeline Tracking**: Real-time position updates with progress indicators
- **Artwork Handling**: Save and retrieve album artwork with proper error handling
- **Cross-Player Compatibility**: Works with most Windows media players (Spotify, Windows Media Player, etc.)
- **Robust Error Handling**: Graceful fallbacks when media or artwork unavailable

### Technical Details

- **Framework**: .NET 9.0 with Windows Runtime APIs
- **Architecture**: Modular utility classes for timeline and artwork management
- **Performance**: Optimized with async/await patterns and proper resource disposal
- **Size**: Compressed single executable (~11MB with full .NET runtime included)

### Known Limitations

- Requires Windows 10 version 1809 (build 17763) or later
- Some media players may not expose timeline position data
- Position tracking accuracy depends on media player implementation

### Development Notes

- Initial beta release for testing core functionality
- Structured for easy extension with additional media control features
- Clean separation between timeline and artwork utilities
