using SE2RobotFramework.Core;

namespace SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmMouseController
{
    private readonly DrillArmMechanism _mechanism;
    private readonly DrillArmControlService _controlService;

    public DrillArmMouseController(
        DrillArmMechanism mechanism,
        DrillArmControlService controlService,
        DrillArmMouseControlConfiguration configuration)
    {
        _mechanism = mechanism ?? throw new ArgumentNullException(nameof(mechanism));
        _controlService = controlService ??
            throw new ArgumentNullException(nameof(controlService));
        Configuration = configuration ??
            throw new ArgumentNullException(nameof(configuration));
        Configuration.Validate();
    }

    public DrillArmMouseControlConfiguration Configuration { get; }

    public DrillArmMouseControlResult Apply(DrillArmMouseInput input)
    {
        input.Validate();
        DrillArmAxes axes = _mechanism.Axes;

        double baseRotation = ApplyDelta(
            axes.BaseRotation,
            input.HorizontalDelta * Configuration.BaseRotationDegreesPerUnit);
        double shoulder = ApplyDelta(
            axes.Shoulder,
            input.VerticalDelta * Configuration.ShoulderDegreesPerUnit);
        double elbow = ApplyDelta(
            axes.Elbow,
            input.VerticalDelta * Configuration.ElbowDegreesPerUnit);

        double shoulderChange = shoulder - axes.Shoulder.TargetPosition;
        double elbowChange = elbow - axes.Elbow.TargetPosition;
        double requiredCompensation = -(shoulderChange + elbowChange);
        double forearmHinge = ApplyDelta(
            axes.ForearmHinge,
            requiredCompensation);
        double actualCompensation =
            forearmHinge - axes.ForearmHinge.TargetPosition;

        DrillArmTargets targets = new(
            baseRotation,
            shoulder,
            axes.UpperArmExtension.TargetPosition,
            elbow,
            axes.ForearmExtension.TargetPosition,
            forearmHinge,
            axes.WristRotation.TargetPosition,
            axes.WristHinge.TargetPosition,
            axes.ToolExtension.TargetPosition);

        _controlService.EnableForearmOrientationHold();
        _controlService.MoveTo(targets);

        return new DrillArmMouseControlResult(
            targets,
            requiredCompensation - actualCompensation);
    }

    public void Update(double deltaTime)
    {
        _controlService.Update(deltaTime);
    }

    public DrillArmControlStatus Status => _controlService.Status;

    public bool IsForearmOrientationHoldEnabled =>
        _controlService.IsForearmOrientationHoldEnabled;

    public double ForearmOrientationErrorDegrees =>
        _controlService.ForearmOrientationErrorDegrees;

    private static double ApplyDelta(Axis axis, double delta)
    {
        return Math.Clamp(
            axis.TargetPosition + delta,
            axis.MinimumPosition,
            axis.MaximumPosition);
    }
}
