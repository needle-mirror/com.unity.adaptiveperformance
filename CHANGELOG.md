# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.2.3] - 2022-06-15

### Changed
- Minor corrections to documentation.

## [1.2.2] - 2021-02-05

### Changed
- Fix package author to show up correctly in the package manager
- Sync version with Samsung package

## [1.2.0] - 2020-07-29

### Changed
- Updates to Subsystem Registry 1.2.0 to fix installation issues if internal subystem module is disabled.
- Update minimum required Unity version to 2019.4.

## [1.1.9] - 2020-07-23

### Changed
- Automatic Performance Mode: gpuUtilizationThreshold increased from 0.7 to 0.9 to increase effeciency.
- Automatic Performance Mode: gpuFactor increased from 0.72 to 0.92 to increase effeciency.
- Automatic Performance Mode: lower CPU and GPU levels at the same time instead of one at a time to increase effeciency and higher power savings.
- Changed Documentation to make clear that changing CPU and GPU levels is risky and the Automatic Performance Mode should be used instead.
- Fixed Automatic Performance Control flag to respect the function and not be read only anymore.
- Inrease GPU Active time Ratio.

## [1.1.6] - 2020-04-29

### Changed
- Fix Analytics system error with unloaded subsystem.

## [1.1.0] - 2019-11-07

### Changed
- Fixed initial version log to represent the correct version (subsystem)
- Fixed non reachable code warning.
- Fixed .net 3.5 breaking warning of unused variables and unassigned variables.

### Added
- Analytics events for subsystem manager and thermal status.

## [1.0.1] - 2019-08-29

### Changed
- Subsystem API to ensure compatibility with Subsystems in Unity 2019.3
- The default performance levels are to the maximum levels in automatic and manual mode

## [1.0.0] - 2019-08-19

### This is the first release of *Adaptive Performance*.
