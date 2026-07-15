namespace SE2RobotFramework.Mechanisms.DrillArm;

public readonly record struct DrillArmKeyboardInput(
    double UpperArmExtension,
    double ForearmExtension,
    double ForearmHinge,
    double ToolExtension)
{
    public void Validate()
    {
        ValidateDirection(UpperArmExtension, nameof(UpperArmExtension));
        ValidateDirection(ForearmExtension, nameof(ForearmExtension));
        ValidateDirection(ForearmHinge, nameof(ForearmHinge));
        ValidateDirection(ToolExtension, nameof(ToolExtension));
    }

    private static void ValidateDirection(double value, string parameterName)
    {
        if (!double.IsFinite(value) || value < -1.0 || value > 1.0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
