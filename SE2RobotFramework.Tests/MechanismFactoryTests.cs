using System.Numerics;
using SE2RobotFramework.Configuration;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.DrillArm;
using SE2RobotFramework.Mechanisms.Solar;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class MechanismFactoryTests
{
    [Fact]
    public void SolarFactory_CreatesConfiguredMirroredDualRotorMechanism()
    {
        FakeAxisHardware firstElevationRotor = new();
        FakeAxisHardware secondElevationRotor = new();
        SolarArrayConfiguration configuration = new()
        {
            Type = SolarArrayType.BaseRotorWithDualRotors,
            AzimuthAxis = CreateAxis("Solar.Azimuth", AxisType.Rotational, 5.0),
            ElevationAxis = CreateAxis("Solar.Elevation", AxisType.Rotational, 3.0)
        };

        SolarArrayMechanism mechanism = new SolarArrayMechanismFactory().Create(
            configuration,
            new FakeAxisHardware(),
            new[] { firstElevationRotor, secondElevationRotor });
        mechanism.SetTargetOrientation(0.0, 30.0);
        mechanism.Update(0.1);

        Assert.Equal("Solar.Elevation", mechanism.ElevationAxis.Name);
        Assert.Equal(3.0, firstElevationRotor.CommandedVelocity);
        Assert.Equal(-3.0, secondElevationRotor.CommandedVelocity);
    }

    [Fact]
    public void SolarRuntimeFactory_WiresMirroredTrackingPipeline()
    {
        FakeAxisHardware firstElevation = new();
        FakeAxisHardware secondElevation = new();
        SolarArrayRuntime runtime = new SolarArrayRuntimeFactory().Create(
            new SolarArrayConfiguration
            {
                Type = SolarArrayType.BaseRotorWithDualRotors,
                AzimuthAxis = CreateAxis(
                    "Solar.Azimuth",
                    AxisType.Rotational,
                    5.0),
                ElevationAxis = CreateAxis(
                    "Solar.Elevation",
                    AxisType.Rotational,
                    3.0)
            },
            new FakeAxisHardware(),
            new[] { firstElevation, secondElevation },
            new FixedSunDirectionProvider(
                Vector3.Normalize(new Vector3(1.0f, 1.0f, 1.0f))));

        runtime.Update(0.1);

        Assert.Equal(SolarTrackingServiceStatus.Tracking, runtime.Status);
        Assert.NotNull(runtime.LastOrientation);
        Assert.Equal(
            firstElevation.CommandedVelocity,
            -secondElevation.CommandedVelocity);
        Assert.False(runtime.RuntimeState.HasFault);

        runtime.Stop();

        Assert.Equal(SolarTrackingServiceStatus.Stopped, runtime.Status);
    }

    [Fact]
    public void SolarRuntimeConfigurationApplier_UpdatesProfileAndTrackingFrame()
    {
        FixedSunDirectionProvider provider = new(Vector3.UnitY);
        SolarArrayRuntime runtime = new SolarArrayRuntimeFactory().Create(
            new SolarArrayConfiguration
            {
                Type = SolarArrayType.BaseRotorWithRotor,
                AzimuthAxis = CreateAxis(
                    "Solar.Azimuth",
                    AxisType.Rotational,
                    5.0),
                ElevationAxis = CreateAxis(
                    "Solar.Elevation",
                    AxisType.Rotational,
                    3.0)
            },
            new FakeAxisHardware(),
            new[] { new FakeAxisHardware() },
            provider);
        SolarTrackingFrame updatedFrame = new()
        {
            AzimuthOffsetDegrees = 20.0,
            ElevationOffsetDegrees = 10.0
        };
        SolarArrayConfiguration updated = new()
        {
            Type = SolarArrayType.BaseRotorWithRotor,
            AzimuthAxis = CreateAxis(
                "Solar.Azimuth",
                AxisType.Rotational,
                4.0,
                MotionProfileType.SCurve),
            ElevationAxis = CreateAxis(
                "Solar.Elevation",
                AxisType.Rotational,
                2.0,
                MotionProfileType.SCurve),
            TrackingFrame = updatedFrame
        };

        FrameworkConfigurationSerializer serializer = new();
        string json = serializer.SerializeSolarArray(updated);

        SolarArrayConfiguration applied =
            new RuntimeConfigurationService().ApplySolarArray(json, runtime);
        runtime.Update(0.1);

        Assert.Equal(
            MotionProfileType.SCurve,
            runtime.Mechanism.AzimuthAxis.MotionProfileType);
        Assert.Same(applied.TrackingFrame, runtime.TrackingController.Frame);
        Assert.Same(provider, runtime.SunDirectionProvider);
        Assert.Equal(SolarTrackingServiceStatus.Tracking, runtime.Status);
        Assert.Equal(110.0, runtime.LastOrientation?.AzimuthDegrees);
        Assert.Equal(10.0, runtime.LastOrientation?.ElevationDegrees);
        Assert.Equal(20.0, applied.TrackingFrame.AzimuthOffsetDegrees);
    }

    [Fact]
    public void SolarFactory_WithDesynchronizedDualRotors_BlocksMovement()
    {
        FakeAxisHardware firstElevationRotor = new();
        FakeAxisHardware secondElevationRotor = new();
        secondElevationRotor.SetPosition(-5.0);
        SolarArrayConfiguration configuration = new()
        {
            Type = SolarArrayType.BaseRotorWithDualRotors,
            AzimuthAxis = CreateAxis("Solar.Azimuth", AxisType.Rotational, 5.0),
            ElevationAxis = CreateAxis("Solar.Elevation", AxisType.Rotational, 3.0),
            MaximumElevationSynchronizationError = 2.0
        };

        SolarArrayMechanism mechanism = new SolarArrayMechanismFactory().Create(
            configuration,
            new FakeAxisHardware(),
            new[] { firstElevationRotor, secondElevationRotor });
        mechanism.SetTargetOrientation(0.0, 30.0);
        mechanism.Update(0.1);

        Assert.Equal(0.0, firstElevationRotor.CommandedVelocity);
        Assert.Equal(0.0, secondElevationRotor.CommandedVelocity);
    }

    [Fact]
    public void PistonBankFactory_WithMatchingLayout_CreatesHardware()
    {
        PistonBankConfiguration configuration = new()
        {
            SeriesCount = 4,
            ParallelCount = 5
        };

        PistonBankAxisHardware hardware = new PistonBankHardwareFactory().Create(
            configuration,
            CreatePistons(5, 4));

        Assert.Equal(4, hardware.SeriesCount);
        Assert.Equal(5, hardware.ParallelCount);
    }

    [Fact]
    public void PistonBankFactory_WithMismatchedLayout_Throws()
    {
        PistonBankConfiguration configuration = new()
        {
            SeriesCount = 4,
            ParallelCount = 5
        };

        Assert.Throws<ArgumentException>(() =>
            new PistonBankHardwareFactory().Create(
                configuration,
                CreatePistons(4, 4)));
    }

    [Fact]
    public void DrillArmFactory_CreatesAllConfiguredAxesAndValidatesPistonBanks()
    {
        DrillArmConfiguration configuration = CreateDrillArmConfiguration();
        DrillArmHardware hardware = new()
        {
            BaseRotation = new FakeAxisHardware(),
            Shoulder = new FakeAxisHardware(),
            UpperArmExtension = new PistonBankAxisHardware(CreatePistons(5, 4)),
            Elbow = new FakeAxisHardware(),
            ForearmExtension = new PistonBankAxisHardware(CreatePistons(5, 3)),
            ForearmHinge = new FakeAxisHardware(),
            WristRotation = new FakeAxisHardware(),
            WristHinge = new FakeAxisHardware(),
            ToolExtension = new PistonBankAxisHardware(CreatePistons(1, 1))
        };

        DrillArmMechanism mechanism = new DrillArmMechanismFactory().Create(
            configuration,
            hardware);

        Assert.Equal("DrillArm.Base", mechanism.Axes.BaseRotation.Name);
        Assert.Equal("DrillArm.UpperArm", mechanism.Axes.UpperArmExtension.Name);
        Assert.Equal(MotionProfileType.Linear, mechanism.Axes.Elbow.MotionProfileType);
    }

    [Fact]
    public void DrillArmFactory_WithWrongUpperArmLayout_Throws()
    {
        DrillArmConfiguration configuration = CreateDrillArmConfiguration();
        DrillArmHardware hardware = new()
        {
            BaseRotation = new FakeAxisHardware(),
            Shoulder = new FakeAxisHardware(),
            UpperArmExtension = new PistonBankAxisHardware(CreatePistons(4, 4)),
            Elbow = new FakeAxisHardware(),
            ForearmExtension = new PistonBankAxisHardware(CreatePistons(5, 3)),
            ForearmHinge = new FakeAxisHardware(),
            WristRotation = new FakeAxisHardware(),
            WristHinge = new FakeAxisHardware(),
            ToolExtension = new PistonBankAxisHardware(CreatePistons(1, 1))
        };

        Assert.Throws<InvalidOperationException>(() =>
            new DrillArmMechanismFactory().Create(configuration, hardware));
    }

    [Fact]
    public void DrillArmRuntimeFactory_WiresConfiguredManualControlPipeline()
    {
        DrillArmRuntime runtime = new DrillArmRuntimeFactory().Create(
            CreateDrillArmConfiguration(),
            new DrillArmHardware
            {
                BaseRotation = new FakeAxisHardware(),
                Shoulder = new FakeAxisHardware(),
                UpperArmExtension = new PistonBankAxisHardware(
                    CreatePistons(5, 4)),
                Elbow = new FakeAxisHardware(),
                ForearmExtension = new PistonBankAxisHardware(
                    CreatePistons(5, 3)),
                ForearmHinge = new FakeAxisHardware(),
                WristRotation = new FakeAxisHardware(),
                WristHinge = new FakeAxisHardware(),
                ToolExtension = new PistonBankAxisHardware(
                    CreatePistons(1, 1))
            });

        DrillArmManualInputResult result = runtime.ProcessManualInput(
            new DrillArmManualInput(
                default,
                new DrillArmKeyboardInput(1.0, 0.0, 0.0, 0.0)),
            0.1);

        Assert.Equal(0.1, result.Targets.UpperArmExtension);
        Assert.NotNull(runtime.ControlService.ActiveTargets);
        Assert.NotNull(runtime.LastRuntimeState);

        runtime.Stop();

        Assert.Equal(DrillArmControlStatus.Stopped, runtime.Status);
    }

    [Fact]
    public void DrillArmRuntimeConfigurationApplier_UpdatesProfilesAndInputRates()
    {
        DrillArmHardware hardware = new()
        {
            BaseRotation = new FakeAxisHardware(),
            Shoulder = new FakeAxisHardware(),
            UpperArmExtension = new PistonBankAxisHardware(CreatePistons(5, 4)),
            Elbow = new FakeAxisHardware(),
            ForearmExtension = new PistonBankAxisHardware(CreatePistons(5, 3)),
            ForearmHinge = new FakeAxisHardware(),
            WristRotation = new FakeAxisHardware(),
            WristHinge = new FakeAxisHardware(),
            ToolExtension = new PistonBankAxisHardware(CreatePistons(1, 1))
        };
        DrillArmRuntime runtime = new DrillArmRuntimeFactory().Create(
            CreateDrillArmConfiguration(),
            hardware);
        DrillArmConfiguration updated = CreateDrillArmConfiguration(
            MotionProfileType.SCurve,
            upperArmKeyboardRate: 2.0);

        FrameworkConfigurationSerializer serializer = new();
        string json = serializer.SerializeDrillArm(updated);

        DrillArmConfiguration applied =
            new RuntimeConfigurationService().ApplyDrillArm(json, runtime);
        DrillArmManualInputResult result = runtime.ProcessManualInput(
            new DrillArmManualInput(
                default,
                new DrillArmKeyboardInput(1.0, 0.0, 0.0, 0.0)),
            0.1);

        Assert.Equal(
            MotionProfileType.SCurve,
            runtime.Mechanism.Axes.UpperArmExtension.MotionProfileType);
        Assert.Equal(0.2, result.Targets.UpperArmExtension);
        Assert.Equal(
            MotionProfileType.SCurve,
            applied.UpperArmExtension.MotionProfileType);
    }

    private static DrillArmConfiguration CreateDrillArmConfiguration(
        MotionProfileType upperArmProfile = MotionProfileType.Linear,
        double upperArmKeyboardRate = 1.0)
    {
        return new DrillArmConfiguration
        {
            BaseRotation = CreateAxis("DrillArm.Base", AxisType.Rotational, 5.0),
            Shoulder = CreateAxis("DrillArm.Shoulder", AxisType.Rotational, 5.0),
            UpperArmExtension = CreateAxis(
                "DrillArm.UpperArm",
                AxisType.Linear,
                4.0,
                upperArmProfile),
            UpperArmPistons = new PistonBankConfiguration
            {
                SeriesCount = 4,
                ParallelCount = 5
            },
            Elbow = CreateAxis("DrillArm.Elbow", AxisType.Rotational, 3.0),
            ForearmExtension = CreateAxis("DrillArm.Forearm", AxisType.Linear, 4.0),
            ForearmPistons = new PistonBankConfiguration
            {
                SeriesCount = 3,
                ParallelCount = 5
            },
            ForearmHinge = CreateAxis("DrillArm.ForearmHinge", AxisType.Rotational, 3.0),
            WristRotation = CreateAxis("DrillArm.WristRotation", AxisType.Rotational, 2.0),
            WristHinge = CreateAxis("DrillArm.WristHinge", AxisType.Rotational, 2.0),
            ToolExtension = CreateAxis("DrillArm.Tool", AxisType.Linear, 1.0),
            ToolPistons = new PistonBankConfiguration
            {
                SeriesCount = 1,
                ParallelCount = 1
            },
            KeyboardControl = new DrillArmKeyboardControlConfiguration
            {
                UpperArmExtensionMetersPerSecond = upperArmKeyboardRate
            }
        };
    }

    private static AxisConfiguration CreateAxis(
        string name,
        AxisType axisType,
        double maximumSpeed,
        MotionProfileType motionProfileType = MotionProfileType.Linear)
    {
        return new AxisConfiguration
        {
            Name = name,
            AxisType = axisType,
            MotionProfileType = motionProfileType,
            MotionLimits = new MotionLimitsConfiguration
            {
                MaximumSpeed = maximumSpeed,
                MaximumAcceleration = 2.0,
                MaximumJerk = 5.0
            },
            Tolerance = 0.1
        };
    }

    private static FakeAxisHardware[][] CreatePistons(
        int parallelCount,
        int seriesCount)
    {
        return Enumerable.Range(0, parallelCount)
            .Select(_ => Enumerable.Range(0, seriesCount)
                .Select(_ => new FakeAxisHardware())
                .ToArray())
            .ToArray();
    }

    private sealed class FixedSunDirectionProvider : ISunDirectionProvider
    {
        private readonly Vector3 _direction;

        public FixedSunDirectionProvider(Vector3 direction)
        {
            _direction = direction;
        }

        public bool TryGetSunDirection(out Vector3 sunDirection)
        {
            sunDirection = _direction;
            return true;
        }
    }
}
