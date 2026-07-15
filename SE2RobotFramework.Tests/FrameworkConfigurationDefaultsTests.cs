using SE2RobotFramework.Configuration;
using SE2RobotFramework.Mechanisms.Solar;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class FrameworkConfigurationDefaultsTests
{
    [Theory]
    [InlineData(SolarArrayType.BaseRotorWithHinge)]
    [InlineData(SolarArrayType.BaseRotorWithDualRotors)]
    [InlineData(SolarArrayType.BaseRotorWithRotor)]
    public void CreateSolarArray_CreatesValidConfigurationForEveryVariant(
        SolarArrayType type)
    {
        SolarArrayConfiguration configuration =
            FrameworkConfigurationDefaults.CreateSolarArray(type);

        configuration.Validate();
        Assert.Equal(type, configuration.Type);
        Assert.Equal(
            MotionProfileType.SCurve,
            configuration.AzimuthAxis.MotionProfileType);
        Assert.True(configuration.AzimuthAxis.WrapAround);
    }

    [Fact]
    public void CreateSolarArray_HingeUsesHingeLimitsInsteadOfWrapAround()
    {
        SolarArrayConfiguration configuration =
            FrameworkConfigurationDefaults.CreateSolarArray(
                SolarArrayType.BaseRotorWithHinge);

        Assert.Equal(0.0, configuration.ElevationAxis.MinimumPosition);
        Assert.Equal(180.0, configuration.ElevationAxis.MaximumPosition);
        Assert.False(configuration.ElevationAxis.WrapAround);
    }

    [Fact]
    public void CreateDrillArm_UsesSpecifiedDefaultLayoutAndLimits()
    {
        DrillArmConfiguration configuration =
            FrameworkConfigurationDefaults.CreateDrillArm();

        configuration.Validate();
        Assert.Equal(4, configuration.UpperArmPistons.SeriesCount);
        Assert.Equal(5, configuration.UpperArmPistons.ParallelCount);
        Assert.Equal(14.0, configuration.UpperArmExtension.MaximumPosition);
        Assert.Equal(3, configuration.ForearmPistons.SeriesCount);
        Assert.Equal(5, configuration.ForearmPistons.ParallelCount);
        Assert.Equal(10.5, configuration.ForearmExtension.MaximumPosition);
        Assert.Equal(90.0, configuration.Shoulder.MinimumPosition);
        Assert.Equal(100.0, configuration.BaseRotation.MaximumPosition);
    }

    [Fact]
    public void CreateDrillArm_AllowsSixPistonsAndArbitraryParallelRows()
    {
        DrillArmConfiguration configuration =
            FrameworkConfigurationDefaults.CreateDrillArm(
                MotionProfileType.Trapezoidal,
                upperArmSeriesCount: 6,
                upperArmParallelCount: 7,
                forearmSeriesCount: 1,
                forearmParallelCount: 8);

        Assert.Equal(6, configuration.UpperArmPistons.SeriesCount);
        Assert.Equal(7, configuration.UpperArmPistons.ParallelCount);
        Assert.Equal(21.0, configuration.UpperArmExtension.MaximumPosition);
        Assert.Equal(8, configuration.ForearmPistons.ParallelCount);
        Assert.Equal(
            MotionProfileType.Trapezoidal,
            configuration.Elbow.MotionProfileType);
    }

    [Fact]
    public void CreateDrillArm_RejectsMoreThanSixPistonsInSeries()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            FrameworkConfigurationDefaults.CreateDrillArm(
                upperArmSeriesCount: 7));
    }
}
