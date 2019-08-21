# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.0.2] - 2019-08-21

### Changed
- Provider downloader will now download latest available build instead of verified if verified version is below 2.0.0. This can happen on 2019 and 2020.1 as the verified package version is 1.x.

### Removed
- Folders and files which are not needed by Adaptive Performance from the package.

## [2.0.1] - 2019-08-10

### Changed
- Change capitalized sample path for Adaptive LUT to work on Linux.

### Removed
- Folders and files which are not needed by Adaptive Performance from the package.

## [2.0.0] - 2019-06-05

### Added
- Samples to show off different Adaptive Performance features.
- Indexer API which allows to create custom Scalers and a number of sample Scalers.
- Settings for Unified Settings Menu with provider installation via Settings.
- Adaptive Performance Simulator and Device Simulator extension to simulate Adaptive Performance events in the editor using the Device Simulator package.
- Verified support for 2020.2 and minimum support 2019 LTS+. Please use Adaptive Performance 1.x for earlier versions supporting 2018.3+.
