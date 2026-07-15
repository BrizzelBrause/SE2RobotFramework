using SE2RobotFramework.Configuration;
using SE2RobotFramework.Core;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class AxisConfigurationApplierTests
{
    [Fact]
    public void Apply_UpdatesRuntimeMotionSettings()
    {
        RotationalAxis axis = new();
        AxisConfiguration configuration = CreateConfiguration(
            MotionProfileType.Trapezoidal,
            maximumPosition: 180.0);

        new AxisConfigurationApplier().Apply(configuration, axis);

        Assert.Equal("Runtime.Axis", axis.Name);
        Assert.Equal(MotionProfileType.Trapezoidal, axis.MotionProfileType);
        Assert.Equal(12.0, axis.MotionLimits.MaximumSpeed);
        Assert.Equal(3.0, axis.MotionLimits.MaximumAcceleration);
        Assert.Equal(180.0, axis.MaximumPosition);
        Assert.Equal(0.25, axis.Tolerance);
        Assert.True(axis.WrapAround);
    }

    [Fact]
    public void Apply_WithReducedPositionLimit_ReclampsExistingTarget()
    {
        RotationalAxis axis = new();
        axis.SetTargetPosition(150.0);
        AxisConfiguration configuration = CreateConfiguration(
            MotionProfileType.Linear,
            maximumPosition: 90.0);

        new AxisConfigurationApplier().Apply(configuration, axis);

        Assert.Equal(90.0, axis.TargetPosition);
    }

    [Fact]
    public void Apply_WithMismatchedAxisType_Throws()
    {
        LinearAxis axis = new();
        AxisConfiguration configuration = CreateConfiguration(
            MotionProfileType.Linear,
            maximumPosition: 10.0);

        Assert.Throws<ArgumentException>(() =>
            new AxisConfigurationApplier().Apply(configuration, axis));
    }

    private static AxisConfiguration CreateConfiguration(
        MotionProfileType profileType,
        double maximumPosition)
    {
        return new AxisConfiguration
        {
            Name = "Runtime.Axis",
            AxisType = AxisType.Rotational,
            MotionProfileType = profileType,
            MotionLimits = new MotionLimitsConfiguration
            {
                MaximumSpeed = 12.0,
                MaximumAcceleration = profileType == MotionProfileType.Linear ? 0.0 : 3.0,
                MaximumJerk = 0.0
            },
            MinimumPosition = -90.0,
            MaximumPosition = maximumPosition,
            Tolerance = 0.25,
            WrapAround = true
        };
    }
}
