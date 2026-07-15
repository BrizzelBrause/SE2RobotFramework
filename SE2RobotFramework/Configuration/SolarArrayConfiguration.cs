using SE2RobotFramework.Mechanisms.Solar;

namespace SE2RobotFramework.Configuration;

public class SolarArrayConfiguration
{
    public SolarArrayType Type { get; init; }

    public AxisConfiguration AzimuthAxis { get; init; } = new();

    public AxisConfiguration ElevationAxis { get; init; } = new();

    public SolarTrackingFrame TrackingFrame { get; init; } = new();

    public void Validate()
    {
        if (!Enum.IsDefined(Type))
        {
            throw new ArgumentOutOfRangeException(nameof(Type));
        }

        ValidateRotationalAxis(AzimuthAxis, nameof(AzimuthAxis));
        ValidateRotationalAxis(ElevationAxis, nameof(ElevationAxis));
        ArgumentNullException.ThrowIfNull(TrackingFrame);
        TrackingFrame.Validate();
    }

    private static void ValidateRotationalAxis(
        AxisConfiguration configuration,
        string parameterName)
    {
        ArgumentNullException.ThrowIfNull(configuration, parameterName);
        configuration.Validate();

        if (configuration.AxisType != AxisType.Rotational)
        {
            throw new ArgumentException(
                "Solar array axes must be rotational.",
                parameterName);
        }
    }
}
