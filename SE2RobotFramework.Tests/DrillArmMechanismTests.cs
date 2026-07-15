using SE2RobotFramework.Core;
using SE2RobotFramework.Controllers;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.DrillArm;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class DrillArmMechanismTests
{
    [Fact]
    public void Update_CommandsEveryLogicalAxis()
    {
        FakeAxisHardware[] physicalHardware = Enumerable.Range(0, 9)
            .Select(_ => new FakeAxisHardware())
            .ToArray();
        DrillArmMechanism mechanism = CreateMechanism(new DrillArmHardware
        {
            BaseRotation = physicalHardware[0],
            Shoulder = physicalHardware[1],
            UpperArmExtension = physicalHardware[2],
            Elbow = physicalHardware[3],
            ForearmExtension = physicalHardware[4],
            ForearmHinge = physicalHardware[5],
            WristRotation = physicalHardware[6],
            WristHinge = physicalHardware[7],
            ToolExtension = physicalHardware[8]
        });

        mechanism.SetTargets(new DrillArmTargets(
            10.0,
            10.0,
            10.0,
            10.0,
            10.0,
            10.0,
            10.0,
            10.0,
            10.0));
        mechanism.Update(0.1);

        Assert.All(
            physicalHardware,
            hardware => Assert.Equal(6.0, hardware.CommandedVelocity));
    }

    [Fact]
    public void Update_WithConfiguredPistonBanks_CommandsUpperAndForearmPistons()
    {
        FakeAxisHardware[][] upperArmPistons = CreatePistons(5, 4);
        FakeAxisHardware[][] forearmPistons = CreatePistons(5, 3);
        DrillArmHardware hardware = CreateSimpleHardware();
        hardware = new DrillArmHardware
        {
            BaseRotation = hardware.BaseRotation,
            Shoulder = hardware.Shoulder,
            UpperArmExtension = new PistonBankAxisHardware(upperArmPistons),
            Elbow = hardware.Elbow,
            ForearmExtension = new PistonBankAxisHardware(forearmPistons),
            ForearmHinge = hardware.ForearmHinge,
            WristRotation = hardware.WristRotation,
            WristHinge = hardware.WristHinge,
            ToolExtension = hardware.ToolExtension
        };
        DrillArmMechanism mechanism = CreateMechanism(hardware);

        mechanism.SetTargets(new DrillArmTargets(
            0.0,
            0.0,
            10.0,
            0.0,
            10.0,
            0.0,
            0.0,
            0.0,
            0.0));
        mechanism.Update(0.1);

        Assert.All(
            upperArmPistons.SelectMany(row => row),
            piston => Assert.Equal(1.5, piston.CommandedVelocity));
        Assert.All(
            forearmPistons.SelectMany(row => row),
            piston => Assert.Equal(2.0, piston.CommandedVelocity));
    }

    [Fact]
    public void Update_WithMirroredDoubleElbow_CommandsOppositeDirections()
    {
        FakeAxisHardware firstElbow = new();
        FakeAxisHardware secondElbow = new();
        DrillArmHardware hardware = CreateSimpleHardware();
        hardware = new DrillArmHardware
        {
            BaseRotation = hardware.BaseRotation,
            Shoulder = hardware.Shoulder,
            UpperArmExtension = hardware.UpperArmExtension,
            Elbow = new ParallelAxisHardware(new IAxisHardware[]
            {
                new TransformedAxisHardware(firstElbow),
                new TransformedAxisHardware(secondElbow, scale: -1.0)
            }),
            ForearmExtension = hardware.ForearmExtension,
            ForearmHinge = hardware.ForearmHinge,
            WristRotation = hardware.WristRotation,
            WristHinge = hardware.WristHinge,
            ToolExtension = hardware.ToolExtension
        };
        DrillArmMechanism mechanism = CreateMechanism(hardware);

        mechanism.SetTargets(new DrillArmTargets(
            0.0,
            0.0,
            0.0,
            20.0,
            0.0,
            0.0,
            0.0,
            0.0,
            0.0));
        mechanism.Update(0.1);

        Assert.Equal(6.0, firstElbow.CommandedVelocity);
        Assert.Equal(-6.0, secondElbow.CommandedVelocity);
    }

    [Fact]
    public void GetRuntimeState_ReturnsAllNineLogicalAxes()
    {
        DrillArmMechanism mechanism = CreateMechanism(CreateSimpleHardware());
        mechanism.SetTargets(new DrillArmTargets(
            10.0,
            10.0,
            10.0,
            10.0,
            10.0,
            10.0,
            10.0,
            10.0,
            10.0));
        mechanism.Update(0.1);

        MechanismRuntimeState state = mechanism.GetRuntimeState();

        Assert.Equal(9, state.Axes.Count);
        Assert.Equal("DrillArm.Base", state.Axes[0].Name);
        Assert.Equal("DrillArm.Tool", state.Axes[8].Name);
        Assert.True(state.IsMoving);
    }

    private static DrillArmMechanism CreateMechanism(DrillArmHardware hardware)
    {
        return new DrillArmMechanism(
            hardware,
            new DrillArmAxes
            {
                BaseRotation = CreateRotationalAxis("DrillArm.Base"),
                Shoulder = CreateRotationalAxis("DrillArm.Shoulder"),
                UpperArmExtension = CreateLinearAxis("DrillArm.UpperArm"),
                Elbow = CreateRotationalAxis("DrillArm.Elbow"),
                ForearmExtension = CreateLinearAxis("DrillArm.Forearm"),
                ForearmHinge = CreateRotationalAxis("DrillArm.ForearmHinge"),
                WristRotation = CreateRotationalAxis("DrillArm.WristRotation"),
                WristHinge = CreateRotationalAxis("DrillArm.WristHinge"),
                ToolExtension = CreateLinearAxis("DrillArm.Tool")
            },
            new MotionProfileFactory(),
            new MotionRequestFactory());
    }

    private static DrillArmHardware CreateSimpleHardware()
    {
        return new DrillArmHardware
        {
            BaseRotation = new FakeAxisHardware(),
            Shoulder = new FakeAxisHardware(),
            UpperArmExtension = new FakeAxisHardware(),
            Elbow = new FakeAxisHardware(),
            ForearmExtension = new FakeAxisHardware(),
            ForearmHinge = new FakeAxisHardware(),
            WristRotation = new FakeAxisHardware(),
            WristHinge = new FakeAxisHardware(),
            ToolExtension = new FakeAxisHardware()
        };
    }

    private static RotationalAxis CreateRotationalAxis(string name)
    {
        return new RotationalAxis
        {
            Name = name,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimits { MaximumSpeed = 6.0 },
            Tolerance = 0.1
        };
    }

    private static LinearAxis CreateLinearAxis(string name)
    {
        return new LinearAxis
        {
            Name = name,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimits { MaximumSpeed = 6.0 },
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
