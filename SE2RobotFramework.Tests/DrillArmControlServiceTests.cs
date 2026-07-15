using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.DrillArm;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class DrillArmControlServiceTests
{
    [Fact]
    public void MoveToAndUpdate_CommandsAllAxes()
    {
        (DrillArmControlService service, FakeAxisHardware[] hardware) =
            CreateService();

        service.MoveTo(CreateTargets(10.0));
        service.Update(0.1);

        Assert.Equal(DrillArmControlStatus.Moving, service.Status);
        Assert.All(
            hardware,
            axisHardware => Assert.Equal(4.0, axisHardware.CommandedVelocity));
    }

    [Fact]
    public void Update_WhenAllAxesAreAtTarget_ReportsAtTarget()
    {
        (DrillArmControlService service, _) = CreateService();

        service.MoveTo(CreateTargets(0.0));
        service.Update(0.1);

        Assert.Equal(DrillArmControlStatus.AtTarget, service.Status);
    }

    [Fact]
    public void Update_WithHardwareFault_StopsArmAndReportsFault()
    {
        (DrillArmControlService service, FakeAxisHardware[] hardware) =
            CreateService();
        hardware[3].IsAvailable = false;

        service.MoveTo(CreateTargets(10.0));
        service.Update(0.1);

        Assert.Equal(DrillArmControlStatus.Faulted, service.Status);
        Assert.All(
            hardware,
            axisHardware => Assert.Equal(0.0, axisHardware.CommandedVelocity));
    }

    [Fact]
    public void MoveTo_WithNonFiniteTarget_Throws()
    {
        (DrillArmControlService service, _) = CreateService();
        DrillArmTargets targets = CreateTargets(0.0) with
        {
            Elbow = double.NaN
        };

        Assert.Throws<ArgumentException>(() => service.MoveTo(targets));
    }

    [Fact]
    public void Axis_SetTargetPositionWithNonFiniteValue_Throws()
    {
        LinearAxis axis = new();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            axis.SetTargetPosition(double.PositiveInfinity));
    }

    private static (
        DrillArmControlService Service,
        FakeAxisHardware[] Hardware) CreateService()
    {
        FakeAxisHardware[] hardware = Enumerable.Range(0, 8)
            .Select(_ => new FakeAxisHardware())
            .ToArray();
        DrillArmMechanism mechanism = new(
            new DrillArmHardware
            {
                BaseRotation = hardware[0],
                Shoulder = hardware[1],
                UpperArmExtension = hardware[2],
                Elbow = hardware[3],
                ForearmExtension = hardware[4],
                WristRotation = hardware[5],
                WristHinge = hardware[6],
                ToolExtension = hardware[7]
            },
            new DrillArmAxes
            {
                BaseRotation = CreateRotationalAxis("DrillArm.Base"),
                Shoulder = CreateRotationalAxis("DrillArm.Shoulder"),
                UpperArmExtension = CreateLinearAxis("DrillArm.UpperArm"),
                Elbow = CreateRotationalAxis("DrillArm.Elbow"),
                ForearmExtension = CreateLinearAxis("DrillArm.Forearm"),
                WristRotation = CreateRotationalAxis("DrillArm.WristRotation"),
                WristHinge = CreateRotationalAxis("DrillArm.WristHinge"),
                ToolExtension = CreateLinearAxis("DrillArm.Tool")
            },
            new MotionProfileFactory(),
            new MotionRequestFactory());

        return (new DrillArmControlService(mechanism), hardware);
    }

    private static DrillArmTargets CreateTargets(double value)
    {
        return new DrillArmTargets(
            value,
            value,
            value,
            value,
            value,
            value,
            value,
            value);
    }

    private static RotationalAxis CreateRotationalAxis(string name)
    {
        return new RotationalAxis
        {
            Name = name,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimits { MaximumSpeed = 4.0 },
            Tolerance = 0.1
        };
    }

    private static LinearAxis CreateLinearAxis(string name)
    {
        return new LinearAxis
        {
            Name = name,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimits { MaximumSpeed = 4.0 },
            Tolerance = 0.1
        };
    }
}
