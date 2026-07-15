using SE2RobotFramework.Configuration;
using SE2RobotFramework.Mechanisms.Solar;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class MechanismConfigurationTests
{
    [Fact]
    public void PistonBank_AllowsSixInSeriesAndArbitraryParallelCount()
    {
        PistonBankConfiguration configuration = new()
        {
            SeriesCount = 6,
            ParallelCount = 20
        };

        configuration.Validate();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    public void PistonBank_RejectsInvalidSeriesCount(int seriesCount)
    {
        PistonBankConfiguration configuration = new()
        {
            SeriesCount = seriesCount,
            ParallelCount = 1
        };

        Assert.Throws<ArgumentOutOfRangeException>(configuration.Validate);
    }

    [Fact]
    public void SolarArray_WithTwoRotationalAxes_IsValid()
    {
        SolarArrayConfiguration configuration = new()
        {
            Type = SolarArrayType.BaseRotorWithDualRotors,
            AzimuthAxis = CreateAxis("Solar.Azimuth", AxisType.Rotational),
            ElevationAxis = CreateAxis("Solar.Elevation", AxisType.Rotational)
        };

        configuration.Validate();
    }

    [Fact]
    public void SolarArray_WithLinearElevation_Throws()
    {
        SolarArrayConfiguration configuration = new()
        {
            Type = SolarArrayType.BaseRotorWithHinge,
            AzimuthAxis = CreateAxis("Solar.Azimuth", AxisType.Rotational),
            ElevationAxis = CreateAxis("Solar.Elevation", AxisType.Linear)
        };

        Assert.Throws<ArgumentException>(configuration.Validate);
    }

    private static AxisConfiguration CreateAxis(string name, AxisType type)
    {
        return new AxisConfiguration
        {
            Name = name,
            AxisType = type,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimitsConfiguration
            {
                MaximumSpeed = 5.0
            },
            Tolerance = 0.1
        };
    }
}
