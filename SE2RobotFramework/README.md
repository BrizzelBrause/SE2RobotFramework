# SE2RobotFramework

A .NET framework for motion control and robot axis simulation.

## Features

- S-curve motion profile
- Trapezoidal motion profile
- Constant-speed linear motion profile
- Motion state integration
- Linear and rotational axis abstractions
- Direction and zero-point transformation for mounted actuators
- Parallel actuator groups for coupled rotors
- Series actuator groups for piston chains
- Configurable piston banks with 1-6 pistons per row and any number of parallel rows
- Selectable two-axis solar array mechanisms for hinge, single-rotor, and mirrored dual-rotor elevation
- Eight-axis drill-arm mechanism with configurable piston banks and coupled joints
- Serializable and validated runtime configuration for axes, solar arrays, and drill arms
- Live application of profile, limit, tolerance, and range changes without rebuilding controllers
- Configuration-driven factories for complete solar-array and drill-arm mechanisms
- Coordinate-system-independent solar-vector tracking with configurable calibration frame
- Configurable synchronization monitoring for coupled rotors and parallel piston rows
- Fake hardware implementations for testing
- Automated unit tests

## Projects

### SE2RobotFramework

Contains the motion-control implementation and axis abstractions.

### SE2RobotFramework.Tests

Contains the automated test suite for the framework.

## Build

Open `SE2RobotFramework.sln` in Visual Studio and build the solution.

## Tests

Run the test suite using Visual Studio Test Explorer.

## Status

The project is under active development.

