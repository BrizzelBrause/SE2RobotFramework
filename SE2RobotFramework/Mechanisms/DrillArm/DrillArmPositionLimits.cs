namespace SE2RobotFramework.Mechanisms.DrillArm;

public static class DrillArmPositionLimits
{
    public const double PistonStrokeMeters = 3.5;

    public static void Apply(
        DrillArmAxes axes,
        int upperArmSeriesCount = 4,
        int forearmSeriesCount = 3,
        int toolSeriesCount = 1)
    {
        ArgumentNullException.ThrowIfNull(axes);
        ValidateSeriesCount(upperArmSeriesCount, nameof(upperArmSeriesCount));
        ValidateSeriesCount(forearmSeriesCount, nameof(forearmSeriesCount));
        ValidateSeriesCount(toolSeriesCount, nameof(toolSeriesCount));

        SetLimits(axes.BaseRotation, 0.0, 100.0);
        SetLimits(axes.Shoulder, 90.0, 180.0);
        SetLimits(
            axes.UpperArmExtension,
            0.0,
            upperArmSeriesCount * PistonStrokeMeters);
        SetLimits(axes.Elbow, 0.0, 90.0);
        SetLimits(
            axes.ForearmExtension,
            0.0,
            forearmSeriesCount * PistonStrokeMeters);
        SetLimits(axes.ForearmHinge, 0.0, 180.0);
        SetLimits(
            axes.WristRotation,
            double.NegativeInfinity,
            double.PositiveInfinity);
        SetLimits(axes.WristHinge, 0.0, 180.0);
        SetLimits(
            axes.ToolExtension,
            0.0,
            toolSeriesCount * PistonStrokeMeters);
    }

    private static void SetLimits(
        Core.Axis axis,
        double minimum,
        double maximum)
    {
        axis.MinimumPosition = minimum;
        axis.MaximumPosition = maximum;
        axis.SetTargetPosition(axis.TargetPosition);
    }

    private static void ValidateSeriesCount(int count, string parameterName)
    {
        if (count < 1 || count > Hardware.PistonBankAxisHardware.MaximumPistonsPerRow)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
