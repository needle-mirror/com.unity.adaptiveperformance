# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2019-12-03

### Adaptive Performance Simulator and Extension
- Add Adaptive Performance Simulator and Device Simulator extension to simulate Adaptive Performance events in the editor using the device simulator package. 

### Bump Min supported Version
- Adaptive Performance Device Simulator extension requires the Device Simulator package which uses features of Unity 2019.3 and therefore we update Adaptive Performance 2.0.0 min support to 2019.3. Please use Adaptive Performance 1.x for earlier versions supporting 2018.3+.  

## [1.1.0] - 2019-11-07

### Fix warnings and logging
- Fixed initial version log to represent the correct version (subsystem)
- Fixed non reachable code warning. 
- Fixed .net 3.5 breaking warning of unused variables and unassigned variables.

### Analytics Support
- Added Analytics events for subsystem manager and thermal status.   

## [1.0.1] - 2019-08-29

### Compatibility with Subsystem 
- Subsystem API changes in Unity 2019.3

### Change default performance levels
- The default performance levels are changed to the maximum levels in automatic and manual mode

## [1.0.0] - 2019-08-19

### Ensure compatiblity with On Demand Rendering (Unity 2019.3 feature)

## [0.2.0-preview.1] - 2019-06-19

### New API
- CPU frame time
- Reworked AdaptivePerformanceSubsystem interface
- Configurable logging frequency

### Package Split
- The Adaptive Performance package was split into a Adaptive Performance base package *Adaptive Performance* and the first provider package *AP Samsung (Android)*

### Other
- Compatibility with Unity 2017.4

## [0.1.1-preview.1] - 2019-04-29

### Bugfixes
- Only try to initialize Samsung GameSDK on Samsung devices
- Fix exception on shutdown

## [0.1.0-preview.3] - 2019-03-27

### Update Samsung GameSDK license information

## [0.0.1-preview.2] - 2019-03-12

### GameSDK 1.6 support

## [0.0.1-preview.1] - 2019-03-11

### This is the first release of *Unity Package Adaptive Performance*.

Adaptive Performance support for Samsung S10 phones using the Samsung GameSDK
