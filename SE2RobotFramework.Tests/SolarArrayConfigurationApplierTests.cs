using SE2RobotFramework.Configuration;
using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.Solar;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class SolarArrayConfigurationApplierTests
{
    [Fact]
    public void Apply_UpdatesBothSolarAxes()
    {
        SolarArrayMechanism mechanism = CreateMechanism(
            SolarArrayType.BaseRotorWithRotor);
        SolarArrayConfiguration configuration = new()
        {
            Type = SolarArrayType.BaseRotorWithRotor,
            AzimuthAxis = CreateAxisConfiguration("Solar.Azimuth", 8.0),
            ElevationAxis = CreateAxisConfiguration("Solar.Elevation", 4.0)
        };

        new SolarArrayConfigurationApplier().Apply(configuration, mechanism);

        Assert.Equal(8.0, mechanism.AzimuthAxis.MotionLimits.MaximumSpeed);
        Assert.Equal(4.0, mechanism.ElevationAxis.MotionLimits.MaximumSpeed);
        Assert.Equal(MotionProfileType.Linear, mechanism.AzimuthAxis.MotionProfileType);
    }

    [Fact]
    public void Apply_WithChangedHardwareType_Throws()
    {
        SolarArrayMechanism mechanism = CreateMechanism(
            SolarArrayType.BaseRotorWithRotor);
        SolarArrayConfiguration configuration = new()
        {
            Type = SolarArrayType.BaseRotorWithDualRotors,
            AzimuthAxis = CreateAxisConfiguration("Solar.Azimuth", 8.0),
            ElevationAxis = CreateAxisConfiguration("Solar.Elevation", 4.0)
        };

        Assert.Throws<InvalidOperationException>(() =>
            new SolarArrayConfigurationApplier().Apply(configuration, mechanism));
    }

    private static SolarArrayMechanism CreateMechanism(SolarArrayType type)
    {
        return new SolarArrayMechanism(
            new SolarArrayHardware(
                type,
                new FakeAxisHardware(),
                new FakeAxisHardware()),
            new RotationalAxis(),
            new RotationalAxis(),
            new MotionProfileFactory(),
            new MotionRequestFactory());
    }

    private static AxisConfiguration CreateAxisConfiguration(
        string name,
        double maximumSpeed)
    {
        return new AxisConfiguration
        {
            Name = name,
            AxisType = AxisType.Rotational,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimitsConfiguration
            {
                MaximumSpeed = maximumSpeed
            },
            Tolerance = 0.1
        };
    }
}
