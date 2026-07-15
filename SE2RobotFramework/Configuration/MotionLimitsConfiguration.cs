using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Configuration;

public class MotionLimitsConfiguration
{
    public double MaximumSpeed { get; init; }

    public double MaximumAcceleration { get; init; }

    public double MaximumJerk { get; init; }

    public void Validate(MotionProfileType profileType)
    {
        ValidatePositiveFinite(MaximumSpeed, nameof(MaximumSpeed));
        ValidateNonNegativeFinite(MaximumAcceleration, nameof(MaximumAcceleration));
        ValidateNonNegativeFinite(MaximumJerk, nameof(MaximumJerk));

        if (profileType is MotionProfileType.Trapezoidal or MotionProfileType.SCurve &&
            MaximumAcceleration <= 0.0)
        {
            throw new ArgumentException(
                "Acceleration must be greater than zero for trapezoidal and S-curve profiles.",
                nameof(MaximumAcceleration));
        }

        if (profileType == MotionProfileType.SCurve && MaximumJerk <= 0.0)
        {
            throw new ArgumentException(
                "Jerk must be greater than zero for an S-curve profile.",
                nameof(MaximumJerk));
        }
    }

    public MotionLimits ToMotionLimits()
    {
        return new MotionLimits
        {
            MaximumSpeed = MaximumSpeed,
            MaximumAcceleration = MaximumAcceleration,
            MaximumJerk = MaximumJerk
        };
    }

    private static void ValidatePositiveFinite(double value, string parameterName)
    {
        if (!double.IsFinite(value) || value <= 0.0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }

    private static void ValidateNonNegativeFinite(double value, string parameterName)
    {
        if (!double.IsFinite(value) || value < 0.0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
