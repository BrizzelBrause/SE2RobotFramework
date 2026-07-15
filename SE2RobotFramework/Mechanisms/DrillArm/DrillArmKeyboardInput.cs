namespace SE2RobotFramework.Mechanisms.DrillArm;

public readonly record struct DrillArmKeyboardInput(
    double UpperArmExtension,
    double ForearmExtension,
    double ForearmHinge,
    double ToolExtension,
    double WristRotation = 0.0,
    double WristHinge = 0.0)
{
    public void Validate()
    {
        ValidateDirection(UpperArmExtension, nameof(UpperArmExtension));
        ValidateDirection(ForearmExtension, nameof(ForearmExtension));
        ValidateDirection(ForearmHinge, nameof(ForearmHinge));
        ValidateDirection(ToolExtension, nameof(ToolExtension));
        ValidateDirection(WristRotation, nameof(WristRotation));
        ValidateDirection(WristHinge, nameof(WristHinge));
    }

    private static void ValidateDirection(double value, string parameterName)
    {
        if (!double.IsFinite(value) || value < -1.0 || value > 1.0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
