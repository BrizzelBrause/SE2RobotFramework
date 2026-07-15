using SE2RobotFramework.Core;

namespace SE2RobotFramework.Configuration;

public class AxisConfigurationApplier
{
    public void Apply(AxisConfiguration configuration, Axis axis)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(axis);
        configuration.Validate();
        ValidateAxisType(configuration.AxisType, axis);

        axis.Name = configuration.Name;
        axis.MotionProfileType = configuration.MotionProfileType;
        axis.MotionLimits = configuration.MotionLimits.ToMotionLimits();
        axis.MinimumPosition = configuration.MinimumPosition ?? double.NegativeInfinity;
        axis.MaximumPosition = configuration.MaximumPosition ?? double.PositiveInfinity;
        axis.Tolerance = configuration.Tolerance;
        axis.Enabled = configuration.Enabled;

        if (axis is RotationalAxis rotationalAxis)
        {
            rotationalAxis.WrapAround = configuration.WrapAround;
        }

        axis.SetTargetPosition(axis.TargetPosition);
    }

    private static void ValidateAxisType(AxisType expectedType, Axis axis)
    {
        bool matches = expectedType switch
        {
            AxisType.Linear => axis is LinearAxis,
            AxisType.Rotational => axis is RotationalAxis,
            _ => false
        };

        if (!matches)
        {
            throw new ArgumentException(
                $"The configuration requires a {expectedType} axis.",
                nameof(axis));
        }
    }
}
