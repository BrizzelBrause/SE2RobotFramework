using SE2RobotFramework.Core;

namespace SE2RobotFramework.Configuration;

public class AxisFactory
{
    public Axis Create(AxisConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        configuration.Validate();

        Axis axis = configuration.AxisType switch
        {
            AxisType.Linear => new LinearAxis(),
            AxisType.Rotational => new RotationalAxis
            {
                WrapAround = configuration.WrapAround
            },
            _ => throw new ArgumentOutOfRangeException(nameof(configuration.AxisType))
        };

        axis.Name = configuration.Name;
        axis.MotionProfileType = configuration.MotionProfileType;
        axis.MotionLimits = configuration.MotionLimits.ToMotionLimits();
        axis.MinimumPosition = configuration.MinimumPosition ?? double.NegativeInfinity;
        axis.MaximumPosition = configuration.MaximumPosition ?? double.PositiveInfinity;
        axis.Tolerance = configuration.Tolerance;
        axis.Enabled = configuration.Enabled;

        return axis;
    }
}
