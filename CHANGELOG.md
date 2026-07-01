# Change Log

## [9.0.9-beta] - 2026-06-30
- macOS: fix loading Grasshopper, Grasshopper 2, and legacy IronPython plugins (they are pre-loaded during core startup, so `PlugIn.LoadPlugIn` returns Success with no plugin id; gate on the result instead of the id)
- macOS: initialize Eto via `Eto.Platform.Detect` instead of a hardcoded WPF platform

## [9.0.8-beta] - 2026-04-13
- Allow Rhino.Inside to find Rhino when RhinoSystemDir is not set in the config file

## [9.0.7-beta] - 2026-04-10
- Fix broken GH Tests
- Find GH on Linux
- add net9.0 to csproj 

## [9.0.6-beta] - 2026-04-07
- Bump Rhino.Inside to 9.0.26084.13070-beta to better support Linux.

## [9.0.5-beta] - 2025-12-16
- Bump Rhino.Inside to 9.0.10-beta for experimental Mac support.
- Bump RhinoCommon and Grasshopper version to 9.0.25350.305-wip

## [9.0.4-beta] - 2025-05-13
- Added support for loading Grasshopper 2

## [9.0.3-beta] - 2025-05-12
- Resolver bug fixes

## [9.0.2-beta] - 2025-05-12
- Resolver bug fixes

## [9.0.1-beta] - 2025-05-09
- Strong-named package

## [9.0.0-beta] - 2025-05-08
- Initial release
