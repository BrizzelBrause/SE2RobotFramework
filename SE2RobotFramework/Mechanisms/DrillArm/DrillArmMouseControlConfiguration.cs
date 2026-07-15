namespace SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmMouseControlConfiguration
{
    public double BaseRotationDegreesPerUnit { get; init; } = 1.0;

    public double ShoulderDegreesPerUnit { get; init; } = 1.0;

    public double ElbowDegreesPerUnit { get; init; } = 1.0;

    public void Validate()
    {
        ValidateSensitivity(BaseRotationDegreesPerUnit, nameof(BaseRotationDegreesPerUnit));
        ValidateSensitivity(ShoulderDegreesPerUnit, nameof(ShoulderDegreesPerUnit));
        ValidateSensitivity(ElbowDegreesPerUnit, nameof(ElbowDegreesPerUnit));
    }

    private static void ValidateSensitivity(double value, string parameterName)
    {
        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
