namespace SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmKeyboardControlConfiguration
{
    public double UpperArmExtensionMetersPerSecond { get; init; } = 1.0;

    public double ForearmExtensionMetersPerSecond { get; init; } = 1.0;

    public double ForearmHingeDegreesPerSecond { get; init; } = 30.0;

    public double ToolExtensionMetersPerSecond { get; init; } = 1.0;

    public double WristRotationDegreesPerSecond { get; init; } = 30.0;

    public double WristHingeDegreesPerSecond { get; init; } = 30.0;

    public void Validate()
    {
        ValidateRate(
            UpperArmExtensionMetersPerSecond,
            nameof(UpperArmExtensionMetersPerSecond));
        ValidateRate(
            ForearmExtensionMetersPerSecond,
            nameof(ForearmExtensionMetersPerSecond));
        ValidateRate(
            ForearmHingeDegreesPerSecond,
            nameof(ForearmHingeDegreesPerSecond));
        ValidateRate(
            ToolExtensionMetersPerSecond,
            nameof(ToolExtensionMetersPerSecond));
        ValidateRate(
            WristRotationDegreesPerSecond,
            nameof(WristRotationDegreesPerSecond));
        ValidateRate(
            WristHingeDegreesPerSecond,
            nameof(WristHingeDegreesPerSecond));
    }

    private static void ValidateRate(double value, string parameterName)
    {
        if (!double.IsFinite(value) || value < 0.0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
