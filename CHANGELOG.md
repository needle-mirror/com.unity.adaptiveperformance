# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.2.1] - 2021-06-06

### Changed
- Indexer Thermal action returns StateAction.Decrease and StateAction.FastDecrease instead of StateAction.Increase at 0.1 and 0.5 of thermal trend when not in throttling or throttling imminent to activate scalers sooner than Throttling immindent state.
- Scalers do not use switch-case to determine their value, instead use min, max scale and max level to calculate a scale and increment and apply it to their target when the level changes.
- Simulator integration for scalers. Offers now min, max scale and max level additionally to the level override to easily simulate the scaler behaviour.

### Added
- Fix FB 1321990: Sliders don't match their values when in the Simulator View Control Panel
- Scaler profiles
- Boost mode
- Cluster info
- Feature API
- Adaptive view distance
- More samples for boost, scaler profiles, cluster info, and adaptive view distance.

## [2.1.1] - 2021-02-03

### Added
- Fix FB 1304020: Min and Max values are not saved after it is typed and the labels are too long.
- Fix FB 1303986: When the settings view is narrowed "View license" text appears on top of the toggle.
- Fix FB 1297030: Scaler options Checkboxes are misaligned in the Simulator window.
- Fix FB 1296672: Errors are thrown on installing Adaptive Performance Package when Simulator window is open.

## [2.1.0] - 2020-10-12

### Added
- Added a transparency scaler.
- Added settings to control the Indexers thermal and performance actions when using the Device Simulator.
- Updated the version defines for the device simulator to support it in 2021.1 without package.

### Changed
- Session bugfixes for Adaptive Performance provider subsystem management.

## [2.0.2] - 2020-08-21

### Changed
- Provider downloader will now download latest available build instead of verified if verified version is below 2.0.0. This can happen on 2019 and 2020.1 as the verified package version is 1.x.

### Removed
- Folders and files which are not needed by Adaptive Performance from the package.

## [2.0.1] - 2020-08-10

### Changed
- Change capitalized sample path for Adaptive LUT to work on Linux.

### Removed
- Folders and files which are not needed by Adaptive Performance from the package.

## [2.0.0] - 2020-06-05

### Added
- Samples to show off different Adaptive Performance features.
- Indexer API which allows to create custom Scalers and a number of sample Scalers.
- Settings for Unified Settings Menu with provider installation via Settings.
- Adaptive Performance Simulator and Device Simulator extension to simulate Adaptive Performance events in the editor using the Device Simulator package.
- Verified support for 2020.2 and minimum support 2019 LTS+. Please use Adaptive Performance 1.x for earlier versions supporting 2018.3+.
