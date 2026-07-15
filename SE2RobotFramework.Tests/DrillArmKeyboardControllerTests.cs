using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.DrillArm;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class DrillArmKeyboardControllerTests
{
    [Fact]
    public void Apply_ChangesPistonTargetsByConfiguredRates()
    {
        (DrillArmKeyboardController controller, DrillArmMechanism mechanism, _) =
            CreateController();

        DrillArmTargets targets = controller.Apply(
            new DrillArmKeyboardInput(1.0, 0.5, 0.0, 1.0),
            2.0);

        Assert.Equal(2.0, targets.UpperArmExtension);
        Assert.Equal(1.0, targets.ForearmExtension);
        Assert.Equal(2.0, targets.ToolExtension);
        Assert.Equal(targets, GetTargets(mechanism.Axes));
    }

    [Fact]
    public void Apply_ClampsPistonAndHingeTargetsToPositionLimits()
    {
        (DrillArmKeyboardController controller, DrillArmMechanism mechanism, _) =
            CreateController();
        mechanism.Axes.UpperArmExtension.SetTargetPosition(13.5);
        mechanism.Axes.ForearmExtension.SetTargetPosition(0.25);
        mechanism.Axes.ForearmHinge.SetTargetPosition(175.0);
        mechanism.Axes.ToolExtension.SetTargetPosition(3.25);

        DrillArmTargets targets = controller.Apply(
            new DrillArmKeyboardInput(1.0, -1.0, 1.0, 1.0),
            1.0);

        Assert.Equal(14.0, targets.UpperArmExtension);
        Assert.Equal(0.0, targets.ForearmExtension);
        Assert.Equal(180.0, targets.ForearmHinge);
        Assert.Equal(3.5, targets.ToolExtension);
    }

    [Fact]
    public void Apply_ManualForearmInput_DisablesOrientationHold()
    {
        (
            DrillArmKeyboardController controller,
            DrillArmMechanism mechanism,
            DrillArmControlService service) =
            CreateController();
        service.EnableForearmOrientationHold();
        mechanism.Axes.Shoulder.UpdatePosition(100.0);
        service.MoveTo(GetTargets(mechanism.Axes));
        service.Update(0.1);

        Assert.True(service.IsForearmCompensationLimitLatched);

        controller.Apply(
            new DrillArmKeyboardInput(0.0, 0.0, -1.0, 0.0),
            1.0);

        Assert.False(service.IsForearmOrientationHoldEnabled);
        Assert.False(service.IsForearmCompensationLimitLatched);
    }

    [Fact]
    public void Apply_ControlsWristRotationAndLimitedWristHinge()
    {
        (DrillArmKeyboardController controller, DrillArmMechanism mechanism, _) =
            CreateController();
        mechanism.Axes.WristHinge.SetTargetPosition(100.0);

        DrillArmTargets targets = controller.Apply(
            new DrillArmKeyboardInput(
                0.0,
                0.0,
                0.0,
                0.0,
                WristRotation: 1.0,
                WristHinge: -1.0),
            2.0);

        Assert.Equal(60.0, targets.WristRotation);
        Assert.Equal(40.0, targets.WristHinge);
    }

    [Fact]
    public void Input_RejectsValuesOutsideNormalizedRange()
    {
        DrillArmKeyboardInput input = new(1.01, 0.0, 0.0, 0.0);

        Assert.Throws<ArgumentOutOfRangeException>(() => input.Validate());
    }

    private static (
        DrillArmKeyboardController Controller,
        DrillArmMechanism Mechanism,
        DrillArmControlService Service) CreateController()
    {
        DrillArmAxes axes = new()
        {
            BaseRotation = CreateAxis<RotationalAxis>("Base"),
            Shoulder = CreateAxis<RotationalAxis>("Shoulder"),
            UpperArmExtension = CreateAxis<LinearAxis>("UpperArm"),
            Elbow = CreateAxis<RotationalAxis>("Elbow"),
            ForearmExtension = CreateAxis<LinearAxis>("Forearm"),
            ForearmHinge = CreateAxis<RotationalAxis>("ForearmHinge"),
            WristRotation = CreateAxis<RotationalAxis>("WristRotation"),
            WristHinge = CreateAxis<RotationalAxis>("WristHinge"),
            ToolExtension = CreateAxis<LinearAxis>("Tool")
        };
        DrillArmPositionLimits.Apply(axes);
        DrillArmMechanism mechanism = new(
            CreateHardware(),
            axes,
            new MotionProfileFactory(),
            new MotionRequestFactory());
        DrillArmControlService service = new(mechanism);
        DrillArmKeyboardController controller = new(
            mechanism,
            service,
            new DrillArmKeyboardControlConfiguration());

        return (controller, mechanism, service);
    }

    private static TAxis CreateAxis<TAxis>(string name)
        where TAxis : Axis, new()
    {
        return new TAxis
        {
            Name = name,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimits { MaximumSpeed = 4.0 },
            Tolerance = 0.1
        };
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

    private static DrillArmTargets GetTargets(DrillArmAxes axes)
    {
        return new DrillArmTargets(
            axes.BaseRotation.TargetPosition,
            axes.Shoulder.TargetPosition,
            axes.UpperArmExtension.TargetPosition,
            axes.Elbow.TargetPosition,
            axes.ForearmExtension.TargetPosition,
            axes.ForearmHinge.TargetPosition,
            axes.WristRotation.TargetPosition,
            axes.WristHinge.TargetPosition,
            axes.ToolExtension.TargetPosition);
    }
}
