# Changelog

All notable changes to this project will be documented in this file.

## [0.0.1-beta] - 2025-09-02

### Added

- **CLI Interface**: Added command-line arguments for controlling media (`--play-pause`, `--next`, `--prev`).
- **JSON Output**: Added `--json` flag to output media metadata, timeline, and status in machine-readable JSON format.
- **Timeline Tracking**: Implemented `TimelineInfo` to display current position, duration, and progress percentage.
- **Artwork Support**: Added functionality to extract and save album artwork from the current media session.
- **Media Controls**: Integrated with Windows System Media Transport Controls (SMTC) to control playback of any supported app.
- **Formatted Output**: Created a stylish terminal output for human-readable status updates.
- **Build Configuration**:
  - Single-file deployment with compression (~17MB executable).
  - Full trimming enabled for minimal size.
  - .NET 9.0 targeting with Windows 10.0.17763.0 support.

### Fixed

- Addressed potential issues with artwork filename sanitization.
- Improved error handling when no media session is active.
