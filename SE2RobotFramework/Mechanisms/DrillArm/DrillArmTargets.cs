namespace SE2RobotFramework.Mechanisms.DrillArm;

public readonly record struct DrillArmTargets(
    double BaseRotation,
    double Shoulder,
    double UpperArmExtension,
    double Elbow,
    double ForearmExtension,
    double WristRotation,
    double WristHinge,
    double ToolExtension)
{
    public void Validate()
    {
        double[] targets =
        {
            BaseRotation,
            Shoulder,
            UpperArmExtension,
            Elbow,
            ForearmExtension,
            WristRotation,
            WristHinge,
            ToolExtension
        };

        if (targets.Any(target => !double.IsFinite(target)))
        {
            throw new ArgumentException(
                "All drill-arm targets must be finite values.");
        }
    }
}
