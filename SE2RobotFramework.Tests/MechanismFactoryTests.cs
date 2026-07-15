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
            WristRotation = new FakeAxisHardware(),
            WristHinge = new FakeAxisHardware(),
            ToolExtension = new PistonBankAxisHardware(CreatePistons(1, 1))
        };

        Assert.Throws<InvalidOperationException>(() =>
            new DrillArmMechanismFactory().Create(configuration, hardware));
    }

    private static DrillArmConfiguration CreateDrillArmConfiguration()
    {
        return new DrillArmConfiguration
        {
            BaseRotation = CreateAxis("DrillArm.Base", AxisType.Rotational, 5.0),
            Shoulder = CreateAxis("DrillArm.Shoulder", AxisType.Rotational, 5.0),
            UpperArmExtension = CreateAxis("DrillArm.UpperArm", AxisType.Linear, 4.0),
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
            WristRotation = CreateAxis("DrillArm.WristRotation", AxisType.Rotational, 2.0),
            WristHinge = CreateAxis("DrillArm.WristHinge", AxisType.Rotational, 2.0),
            ToolExtension = CreateAxis("DrillArm.Tool", AxisType.Linear, 1.0),
            ToolPistons = new PistonBankConfiguration
            {
                SeriesCount = 1,
                ParallelCount = 1
            }
        };
    }

    private static AxisConfiguration CreateAxis(
        string name,
        AxisType axisType,
        double maximumSpeed)
    {
        return new AxisConfiguration
        {
            Name = name,
            AxisType = axisType,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimitsConfiguration
            {
                MaximumSpeed = maximumSpeed
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
}
