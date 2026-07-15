using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.DrillArm;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class DrillArmManualInputControllerTests
{
    [Fact]
    public void Process_CombinesMouseAndKeyboardChangesInOneFrame()
    {
        (DrillArmManualInputController controller, DrillArmMechanism mechanism, _) =
            CreateController();

        DrillArmManualInputResult result = controller.Process(
            new DrillArmManualInput(
                new DrillArmMouseInput(2.0, 1.0),
                new DrillArmKeyboardInput(1.0, 0.0, 1.0, 1.0)),
            1.0);

        Assert.Equal(2.0, result.Targets.BaseRotation);
        Assert.Equal(1.0, result.Targets.UpperArmExtension);
        Assert.Equal(30.0, result.Targets.ForearmHinge);
        Assert.Equal(1.0, result.Targets.ToolExtension);
        Assert.NotNull(result.MouseResult);
        Assert.Equal(result.Targets.ForearmHinge,
            mechanism.Axes.ForearmHinge.TargetPosition);
    }

    [Fact]
    public void Process_PistonInputDoesNotCancelMouseOrientationHold()
    {
        (
            DrillArmManualInputController controller,
            DrillArmMechanism mechanism,
            DrillArmControlService service) =
            CreateController();

        controller.Process(
            new DrillArmManualInput(
                new DrillArmMouseInput(0.0, 1.0),
                new DrillArmKeyboardInput(1.0, 0.0, 0.0, 0.0)),
            0.1);

        Assert.Equal(0.1, mechanism.Axes.UpperArmExtension.TargetPosition);
        Assert.Equal(0.0, mechanism.Axes.ForearmHinge.TargetPosition);
        Assert.True(service.IsForearmOrientationHoldEnabled);
    }

    private static (
        DrillArmManualInputController Controller,
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
        DrillArmMouseController mouseController = new(
            mechanism,
            service,
            new DrillArmMouseControlConfiguration());
        DrillArmKeyboardController keyboardController = new(
            mechanism,
            service,
            new DrillArmKeyboardControlConfiguration());

        return (
            new DrillArmManualInputController(
                mechanism,
                service,
                mouseController,
                keyboardController),
            mechanism,
            service);
    }

    private static TAxis CreateAxis<TAxis>(string name)
        where TAxis : Axis, new()
    {
        return new TAxis
        {
            Name = name,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimits { MaximumSpeed = 40.0 },
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
}
