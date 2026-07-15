using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Configuration;

public record class AxisConfiguration
{
    public string Name { get; init; } = string.Empty;

    public AxisType AxisType { get; init; }

    public MotionProfileType MotionProfileType { get; init; } = MotionProfileType.SCurve;

    public MotionLimitsConfiguration MotionLimits { get; init; } = new();

    public double? MinimumPosition { get; init; }

    public double? MaximumPosition { get; init; }

    public double Tolerance { get; init; }

    public bool Enabled { get; init; } = true;

    public bool WrapAround { get; init; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new ArgumentException("An axis name is required.", nameof(Name));
        }

        if (!Enum.IsDefined(AxisType))
        {
            throw new ArgumentOutOfRangeException(nameof(AxisType));
        }

        if (!Enum.IsDefined(MotionProfileType))
        {
            throw new ArgumentOutOfRangeException(nameof(MotionProfileType));
        }

        ArgumentNullException.ThrowIfNull(MotionLimits);
        MotionLimits.Validate(MotionProfileType);

        ValidateOptionalFinite(MinimumPosition, nameof(MinimumPosition));
        ValidateOptionalFinite(MaximumPosition, nameof(MaximumPosition));

        if (MinimumPosition > MaximumPosition)
        {
            throw new ArgumentException(
                "Minimum position cannot be greater than maximum position.");
        }

        if (!double.IsFinite(Tolerance) || Tolerance < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(Tolerance));
        }

        if (WrapAround && AxisType != Configuration.AxisType.Rotational)
        {
            throw new ArgumentException(
                "Wrap-around is only valid for rotational axes.",
                nameof(WrapAround));
        }
    }

    private static void ValidateOptionalFinite(double? value, string parameterName)
    {
        if (value.HasValue && !double.IsFinite(value.Value))
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
