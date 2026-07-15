using System.Text.Json;
using SE2RobotFramework.Configuration;
using SE2RobotFramework.Core;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class AxisConfigurationTests
{
    [Fact]
    public void Factory_WithRotationalConfiguration_CreatesConfiguredAxis()
    {
        AxisConfiguration configuration = CreateConfiguration(
            AxisType.Rotational,
            MotionProfileType.SCurve) with
        {
            MinimumPosition = -90.0,
            MaximumPosition = 90.0,
            WrapAround = true
        };

        Axis axis = new AxisFactory().Create(configuration);

        RotationalAxis rotationalAxis = Assert.IsType<RotationalAxis>(axis);
        Assert.Equal("Configured.Axis", axis.Name);
        Assert.Equal(MotionProfileType.SCurve, axis.MotionProfileType);
        Assert.Equal(8.0, axis.MotionLimits.MaximumSpeed);
        Assert.Equal(-90.0, axis.MinimumPosition);
        Assert.Equal(90.0, axis.MaximumPosition);
        Assert.True(rotationalAxis.WrapAround);
    }

    [Fact]
    public void Factory_WithLinearConfiguration_CreatesLinearAxis()
    {
        AxisConfiguration configuration = CreateConfiguration(
            AxisType.Linear,
            MotionProfileType.Linear);

        Axis axis = new AxisFactory().Create(configuration);

        Assert.IsType<LinearAxis>(axis);
    }

    [Fact]
    public void Validate_SCurveWithoutJerk_Throws()
    {
        AxisConfiguration configuration = new()
        {
            Name = "Invalid.Axis",
            AxisType = AxisType.Rotational,
            MotionProfileType = MotionProfileType.SCurve,
            MotionLimits = new MotionLimitsConfiguration
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 0.0
            }
        };

        Assert.Throws<ArgumentException>(configuration.Validate);
    }

    [Fact]
    public void Configuration_CanRoundTripThroughJson()
    {
        AxisConfiguration original = CreateConfiguration(
            AxisType.Linear,
            MotionProfileType.Trapezoidal);

        string json = JsonSerializer.Serialize(original);
        AxisConfiguration? restored = JsonSerializer.Deserialize<AxisConfiguration>(json);

        Assert.NotNull(restored);
        restored.Validate();
        Assert.Equal(original.Name, restored.Name);
        Assert.Equal(original.AxisType, restored.AxisType);
        Assert.Equal(original.MotionProfileType, restored.MotionProfileType);
        Assert.Equal(
            original.MotionLimits.MaximumAcceleration,
            restored.MotionLimits.MaximumAcceleration);
    }

    private static AxisConfiguration CreateConfiguration(
        AxisType axisType,
        MotionProfileType profileType)
    {
        return new AxisConfiguration
        {
            Name = "Configured.Axis",
            AxisType = axisType,
            MotionProfileType = profileType,
            MotionLimits = new MotionLimitsConfiguration
            {
                MaximumSpeed = 8.0,
                MaximumAcceleration = profileType == MotionProfileType.Linear ? 0.0 : 3.0,
                MaximumJerk = profileType == MotionProfileType.SCurve ? 4.0 : 0.0
            },
            Tolerance = 0.1
        };
    }
}
