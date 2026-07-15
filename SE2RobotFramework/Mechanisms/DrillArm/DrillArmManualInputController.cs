namespace SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmManualInputController
{
    private readonly DrillArmMechanism _mechanism;
    private readonly DrillArmControlService _controlService;
    private readonly DrillArmMouseController _mouseController;
    private readonly DrillArmKeyboardController _keyboardController;

    public DrillArmManualInputController(
        DrillArmMechanism mechanism,
        DrillArmControlService controlService,
        DrillArmMouseController mouseController,
        DrillArmKeyboardController keyboardController)
    {
        _mechanism = mechanism ?? throw new ArgumentNullException(nameof(mechanism));
        _controlService = controlService ??
            throw new ArgumentNullException(nameof(controlService));
        _mouseController = mouseController ??
            throw new ArgumentNullException(nameof(mouseController));
        _keyboardController = keyboardController ??
            throw new ArgumentNullException(nameof(keyboardController));
    }

    public DrillArmManualInputResult Process(
        DrillArmManualInput input,
        double deltaTime)
    {
        input.Mouse.Validate();
        input.Keyboard.Validate();
        if (!double.IsFinite(deltaTime) || deltaTime <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaTime));
        }

        DrillArmMouseControlResult? mouseResult = null;
        if (HasMouseInput(input.Mouse))
        {
            mouseResult = _mouseController.Apply(input.Mouse);
        }

        if (HasKeyboardInput(input.Keyboard))
        {
            _keyboardController.Apply(input.Keyboard, deltaTime);
        }

        _controlService.Update(deltaTime);

        return new DrillArmManualInputResult(
            GetTargets(),
            mouseResult,
            _controlService.Status);
    }

    private static bool HasMouseInput(DrillArmMouseInput input)
    {
        return input.HorizontalDelta != 0.0 || input.VerticalDelta != 0.0;
    }

    private static bool HasKeyboardInput(DrillArmKeyboardInput input)
    {
        return
            input.UpperArmExtension != 0.0 ||
            input.ForearmExtension != 0.0 ||
            input.ForearmHinge != 0.0 ||
            input.ToolExtension != 0.0;
    }

    private DrillArmTargets GetTargets()
    {
        DrillArmAxes axes = _mechanism.Axes;
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
