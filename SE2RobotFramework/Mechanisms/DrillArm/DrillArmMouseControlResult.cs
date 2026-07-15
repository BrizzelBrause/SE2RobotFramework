namespace SE2RobotFramework.Mechanisms.DrillArm;

public readonly record struct DrillArmMouseControlResult(
    DrillArmTargets Targets,
    double UncompensatedOrientationChangeDegrees)
{
    public bool IsOrientationMaintained =>
        Math.Abs(UncompensatedOrientationChangeDegrees) <= 0.000001;
}
