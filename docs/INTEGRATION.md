# Integration guide

This guide describes how a game adapter uses the framework. It does not assume a
specific Space Engineers 2 API.

## Create valid configurations

```csharp
SolarArrayConfiguration solarConfiguration =
    FrameworkConfigurationDefaults.CreateSolarArray(
        SolarArrayType.BaseRotorWithDualRotors,
        MotionProfileType.SCurve);

DrillArmConfiguration armConfiguration =
    FrameworkConfigurationDefaults.CreateDrillArm(
        MotionProfileType.SCurve,
        upperArmSeriesCount: 4,
        upperArmParallelCount: 5,
        forearmSeriesCount: 3,
        forearmParallelCount: 5);
```

Parallel row counts are unbounded. Series counts must be between one and six.

## Solar hardware variants

All variants use one base rotor for azimuth.

| Variant | Elevation hardware | Logical behavior |
| --- | --- | --- |
| `BaseRotorWithHinge` | One hinge | Limited to 0-180 degrees |
| `BaseRotorWithRotor` | One rotor | Continuous rotational axis |
| `BaseRotorWithDualRotors` | Two opposite rotors | Equal logical motion, opposite physical signs |

For the dual-rotor variant, pass both physical actuators to
`SolarArrayRuntimeFactory`. The hardware factory applies the mirrored transform.

```csharp
SolarArrayRuntime solar = new SolarArrayRuntimeFactory().Create(
    solarConfiguration,
    baseRotorHardware,
    new[] { firstElevationRotor, secondElevationRotor },
    sunDirectionProvider);

solar.Update(deltaTime);
SolarArrayRuntimeSnapshot solarSnapshot = solar.GetSnapshot();
```

`ISunDirectionProvider` must return a finite, non-zero direction. The configured
`SolarTrackingFrame` maps world or game coordinates into logical azimuth and
elevation.

## Drill-arm hardware layout

`DrillArmHardware` contains nine logical axes:

| Axis | Default physical composition | Mechanical range |
| --- | --- | --- |
| Base rotation | Rotor | 0-100 degrees |
| Shoulder | Hinge | 90-180 degrees |
| Upper-arm extension | Piston bank | 0 to series count x 3.5 m |
| Elbow | Mirrored double hinge | 0-90 degrees |
| Forearm extension | Piston bank | 0 to series count x 3.5 m |
| Forearm hinge | Hinge | 0-180 degrees |
| Wrist rotation | Rotor | Unlimited |
| Wrist hinge | Hinge | 0-180 degrees |
| Tool extension | Piston bank | 0 to series count x 3.5 m |

The optional drill head implements `ISwitchableHardware`.

Use `SeriesAxisHardware` for each piston row and `PistonBankAxisHardware` for the
complete set of parallel rows. Use a negative `TransformedAxisHardware` inside a
`ParallelAxisHardware` for the mirrored elbow hinge.

```csharp
DrillArmRuntime arm = new DrillArmRuntimeFactory().Create(
    armConfiguration,
    drillArmHardware);
```

## Manual input frame

Mouse input is expressed as relative deltas. Keyboard input uses normalized
directions from `-1` to `+1`.

```csharp
DrillArmManualInput input = new(
    new DrillArmMouseInput(horizontalDelta, verticalDelta),
    new DrillArmKeyboardInput(
        upperArmExtension,
        forearmExtension,
        forearmHinge,
        toolExtension,
        wristRotation,
        wristHinge),
    DrillHeadEnabled: drillRequested);

DrillArmManualInputResult result = arm.ProcessManualInput(input, deltaTime);
DrillArmRuntimeSnapshot snapshot = arm.GetSnapshot();
```

Mouse X controls the base rotor. Mouse Y changes the shoulder and elbow logical
angles. The forearm hinge compensates both changes to preserve its world
orientation. If compensation reaches 0 or 180 degrees, it remains latched at that
limit until non-zero manual forearm-hinge input releases the hold.

Keyboard piston and wrist commands may run in the same frame as mouse input.
`DrillHeadEnabled` is a desired state: an adapter may implement hold-to-run or keep
a toggle state. Invalid input or an arm-axis fault stops the drill head.

## Runtime configuration

Configurations are versioned JSON documents.

```csharp
RuntimeConfigurationService configurationService = new();

string json = configurationService.ExportDrillArm(arm);
RuntimeConfigurationApplyResult<DrillArmConfiguration> applyResult =
    configurationService.TryApplyDrillArm(updatedJson, arm);
```

`TryApply` reports invalid documents, unsupported schema versions, invalid values,
and changes that require rebuilding hardware. Profile, speed, acceleration, jerk,
tolerance, input sensitivity, and solar tracking-frame changes can be applied to
an existing runtime.

The following changes require a new hardware binding:

- solar hardware variant;
- solar synchronization topology or limit;
- piston series or parallel count;
- piston-row synchronization topology or limit.

## Frame loop and shutdown

Call each active runtime exactly once per game simulation frame with a finite,
positive `deltaTime`. Always stop runtimes when blocks unload, ownership changes,
or the adapter shuts down.

```csharp
arm.Stop();
solar.Stop();
```

Snapshots are read-only UI data and contain controller status, hardware status,
targets, measured positions, motion profile, faults, and mechanism-specific state.
