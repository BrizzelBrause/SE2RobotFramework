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
- Nine-axis drill-arm mechanism with configurable piston banks and coupled joints
- Serializable and validated runtime configuration for axes, solar arrays, and drill arms
- Live application of profile, limit, tolerance, and range changes without rebuilding controllers
- Configuration-driven factories for complete solar-array and drill-arm mechanisms
- Coordinate-system-independent solar-vector tracking with configurable calibration frame
- Configurable synchronization monitoring for coupled rotors and parallel piston rows
- Runtime diagnostics for readiness, invalid feedback, unavailability, and synchronization loss
- Aggregated runtime snapshots for displaying every mechanism axis in a user interface
- Automatic solar-tracking service with a replaceable sun-vector provider and safe stop behavior
- Versioned, human-readable JSON persistence for solar-array and drill-arm configurations
- Safe joint-level drill-arm control service with target validation and fault handling
- Configurable mouse control for base, shoulder, and elbow with compensating forearm hinge
- Configurable keyboard control for piston banks and manual forearm-hinge adjustment
- Combined per-frame manual input for simultaneous mouse and keyboard control
- Configuration-driven drill-arm runtime facade for game-adapter integration
- Configuration-driven solar runtime with automatic sun-vector tracking
- Fake hardware implementations for testing
- Automated unit tests

## Projects

### SE2RobotFramework

Contains the motion-control implementation and axis abstractions.

This project builds as a reusable class library and contains no game or console entry point.

### SE2RobotFramework.Demo

Contains a standalone console simulation demonstrating the framework without game dependencies.

### SE2RobotFramework.Tests

Contains the automated test suite for the framework.

## Build

Open `SE2RobotFramework.slnx` in Visual Studio or run:

```shell
dotnet build SE2RobotFramework.slnx
```

## Tests

Run the test suite using Visual Studio Test Explorer.

The console demonstration includes solar-axis and combined drill-arm manual-input
simulations. It can be started with:

```shell
dotnet run --project SE2RobotFramework.Demo
```

## Continuous integration

Every push and pull request to `master` restores and builds the complete solution,
runs all unit tests, and executes the console demonstration as a smoke test.

## Status

The project is under active development.

