# Architecture

SE2RobotFramework separates game-specific block access from motion planning and
mechanism behavior. The core library has no dependency on Space Engineers 2.

## Layers

1. `Hardware` defines the minimum actuator contracts and compositions.
2. `Core` represents logical linear and rotational axes.
3. `Motion` calculates Linear, Trapezoidal, and S-Curve commands.
4. `Controllers` connect one logical command to hardware feedback.
5. `Mechanisms` coordinate complete solar arrays and drill arms.
6. `Configuration` validates, creates, persists, and updates runtimes.

Game adapters belong above these layers. They translate SE2 blocks, mouse state,
keyboard state, and sun direction into the framework interfaces.

## Hardware composition

`IAxisHardware` is the adapter boundary for rotors, hinges, and pistons.

- `TransformedAxisHardware` changes sign, scale, and zero point.
- `ParallelAxisHardware` drives physically separate actuators as one logical axis.
- `SeriesAxisHardware` sums piston travel in one row.
- `PistonBankAxisHardware` combines parallel rows of series pistons.
- `ISwitchableHardware` represents the optional drill head.

The mirrored solar elevation pair and mirrored double elbow use transformed
actuators inside a parallel group. A positive logical command therefore produces
opposite physical commands without special cases in the motion controllers.

## Runtime ownership

`DrillArmRuntime` owns the drill-arm mechanism, safe control service, manual input
controller, optional drill-head controller, active configuration, and HUD snapshot.

`SolarArrayRuntime` owns the solar mechanism, orientation controller, automatic
tracking service, sun-direction provider, active configuration, and HUD snapshot.

Runtime factories are the recommended construction entry points. Runtime
configuration appliers update profile and calibration settings while preserving
hardware bindings. Changes to physical layouts require rebuilding the runtime.

## Safety model

- Every numeric input and configuration is validated before use.
- Logical targets are clamped to mechanical limits.
- Hardware faults stop all axes in the affected mechanism.
- Invalid manual input or an arm-axis fault stops the drill head.
- Missing or invalid sun vectors stop solar tracking.
- Synchronization limits protect parallel rotors and piston rows.
- Runtime reconfiguration stops active commands before replacing controllers.

The core does not bypass game safety or ownership rules. A future SE2 adapter must
report unavailable blocks through the hardware contracts.

## Scope boundary

The game-independent core is complete without an SE2 reference. The remaining
game-specific package will need the final SE2 modding API for:

- finding and grouping blocks;
- reading rotor, hinge, and piston feedback;
- writing actuator velocities and enabled states;
- reading mouse and keyboard input;
- obtaining the sun direction in a known coordinate system;
- presenting configuration and diagnostics in the game UI.

Preprogrammed drill-arm routines and inverse kinematics are intentionally outside
the current manual-control scope.
