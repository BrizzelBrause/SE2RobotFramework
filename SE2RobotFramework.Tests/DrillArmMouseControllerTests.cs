using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.DrillArm;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class DrillArmMouseControllerTests
{
    [Fact]
    public void Apply_HorizontalMouseMovement_ControlsLimitedBaseRotor()
    {
        (DrillArmMouseController controller, DrillArmMechanism mechanism) =
            CreateController();

        DrillArmMouseControlResult result = controller.Apply(
            new DrillArmMouseInput(150.0, 0.0));

        Assert.Equal(100.0, result.Targets.BaseRotation);
        Assert.Equal(100.0, mechanism.Axes.BaseRotation.TargetPosition);
    }

    [Fact]
    public void Apply_VerticalMouseMovement_MovesShoulderAndElbowAndCompensatesForearm()
    {
        (DrillArmMouseController controller, DrillArmMechanism mechanism) =
            CreateController();
        SetPose(mechanism, shoulder: 100.0, elbow: 20.0, forearmHinge: 100.0);

        DrillArmMouseControlResult result = controller.Apply(
            new DrillArmMouseInput(0.0, 10.0));

        Assert.Equal(110.0, result.Targets.Shoulder);
        Assert.Equal(30.0, result.Targets.Elbow);
        Assert.Equal(80.0, result.Targets.ForearmHinge);
        Assert.True(result.IsOrientationMaintained);
    }

    [Fact]
    public void Apply_UsesActualLimitedJointChangesForCompensation()
    {
        (DrillArmMouseController controller, DrillArmMechanism mechanism) =
            CreateController();
        SetPose(mechanism, shoulder: 175.0, elbow: 85.0, forearmHinge: 100.0);

        DrillArmMouseControlResult result = controller.Apply(
            new DrillArmMouseInput(0.0, 20.0));

        Assert.Equal(180.0, result.Targets.Shoulder);
        Assert.Equal(90.0, result.Targets.Elbow);
        Assert.Equal(90.0, result.Targets.ForearmHinge);
        Assert.True(result.IsOrientationMaintained);
    }

    [Fact]
    public void Apply_WhenForearmHingeCannotCompensate_ReportsRemainingChange()
    {
        (DrillArmMouseController controller, DrillArmMechanism mechanism) =
            CreateController();
        SetPose(mechanism, shoulder: 100.0, elbow: 20.0, forearmHinge: 0.0);

        DrillArmMouseControlResult result = controller.Apply(
            new DrillArmMouseInput(0.0, 10.0));

        Assert.False(result.IsOrientationMaintained);
        Assert.Equal(-20.0, result.UncompensatedOrientationChangeDegrees);
    }

    [Fact]
    public void PositionLimits_UsePistonCountAndSpecifiedJointBounds()
    {
        (_, DrillArmMechanism mechanism) = CreateController();

        Assert.Equal(0.0, mechanism.Axes.BaseRotation.MinimumPosition);
        Assert.Equal(100.0, mechanism.Axes.BaseRotation.MaximumPosition);
        Assert.Equal(90.0, mechanism.Axes.Shoulder.MinimumPosition);
        Assert.Equal(180.0, mechanism.Axes.Shoulder.MaximumPosition);
        Assert.Equal(14.0, mechanism.Axes.UpperArmExtension.MaximumPosition);
        Assert.Equal(90.0, mechanism.Axes.Elbow.MaximumPosition);
        Assert.Equal(10.5, mechanism.Axes.ForearmExtension.MaximumPosition);
        Assert.Equal(180.0, mechanism.Axes.ForearmHinge.MaximumPosition);
        Assert.Equal(double.PositiveInfinity, mechanism.Axes.WristRotation.MaximumPosition);
        Assert.Equal(180.0, mechanism.Axes.WristHinge.MaximumPosition);
        Assert.Equal(3.5, mechanism.Axes.ToolExtension.MaximumPosition);
    }

    private static (
        DrillArmMouseController Controller,
        DrillArmMechanism Mechanism) CreateController()
    {
        DrillArmAxes axes = new()
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
        };
        DrillArmPositionLimits.Apply(axes);
        DrillArmMechanism mechanism = new(
            CreateHardware(),
            axes,
            new MotionProfileFactory(),
            new MotionRequestFactory());
        DrillArmControlService service = new(mechanism);

        return (
            new DrillArmMouseController(
                mechanism,
                service,
                new DrillArmMouseControlConfiguration()),
            mechanism);
    }

    private static void SetPose(
        DrillArmMechanism mechanism,
        double shoulder,
        double elbow,
        double forearmHinge)
    {
        mechanism.SetTargets(new DrillArmTargets(
            0.0,
            shoulder,
            0.0,
            elbow,
            0.0,
            forearmHinge,
            0.0,
            0.0,
            0.0));
    }

    private static DrillArmHardware CreateHardware()
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
