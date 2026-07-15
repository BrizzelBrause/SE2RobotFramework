using SE2RobotFramework.Core;

namespace SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmKeyboardController
{
    private readonly DrillArmMechanism _mechanism;
    private readonly DrillArmControlService _controlService;

    public DrillArmKeyboardController(
        DrillArmMechanism mechanism,
        DrillArmControlService controlService,
        DrillArmKeyboardControlConfiguration configuration)
    {
        _mechanism = mechanism ?? throw new ArgumentNullException(nameof(mechanism));
        _controlService = controlService ??
            throw new ArgumentNullException(nameof(controlService));
        Configuration = configuration ??
            throw new ArgumentNullException(nameof(configuration));
        Configuration.Validate();
    }

    public DrillArmKeyboardControlConfiguration Configuration { get; }

    public DrillArmTargets Apply(DrillArmKeyboardInput input, double deltaTime)
    {
        input.Validate();
        if (!double.IsFinite(deltaTime) || deltaTime <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaTime));
        }

        DrillArmAxes axes = _mechanism.Axes;
        if (input.ForearmHinge != 0.0)
        {
            _controlService.DisableForearmOrientationHold();
        }

        DrillArmTargets targets = new(
            axes.BaseRotation.TargetPosition,
            axes.Shoulder.TargetPosition,
            ApplyInput(
                axes.UpperArmExtension,
                input.UpperArmExtension,
                Configuration.UpperArmExtensionMetersPerSecond,
                deltaTime),
            axes.Elbow.TargetPosition,
            ApplyInput(
                axes.ForearmExtension,
                input.ForearmExtension,
                Configuration.ForearmExtensionMetersPerSecond,
                deltaTime),
            ApplyInput(
                axes.ForearmHinge,
                input.ForearmHinge,
                Configuration.ForearmHingeDegreesPerSecond,
                deltaTime),
            ApplyInput(
                axes.WristRotation,
                input.WristRotation,
                Configuration.WristRotationDegreesPerSecond,
                deltaTime),
            ApplyInput(
                axes.WristHinge,
                input.WristHinge,
                Configuration.WristHingeDegreesPerSecond,
                deltaTime),
            ApplyInput(
                axes.ToolExtension,
                input.ToolExtension,
                Configuration.ToolExtensionMetersPerSecond,
                deltaTime));

        _controlService.MoveTo(targets);
        return targets;
    }

    public void Update(double deltaTime)
    {
        _controlService.Update(deltaTime);
    }

    public DrillArmControlStatus Status => _controlService.Status;

    private static double ApplyInput(
        Axis axis,
        double input,
        double rate,
        double deltaTime)
    {
        return Math.Clamp(
            axis.TargetPosition + input * rate * deltaTime,
            axis.MinimumPosition,
            axis.MaximumPosition);
    }
}
