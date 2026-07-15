using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.Solar;

namespace SE2RobotFramework.Tests;

public class SolarArrayHardwareFactoryTests
{
    [Theory]
    [InlineData(SolarArrayType.BaseRotorWithHinge)]
    [InlineData(SolarArrayType.BaseRotorWithRotor)]
    public void Create_SingleElevationActuator_UsesActuatorDirectly(
        SolarArrayType type)
    {
        SolarArrayHardwareFactory factory = new();
        FakeAxisHardware baseRotor = new();
        FakeAxisHardware elevation = new();

        SolarArrayHardware hardware = factory.Create(
            type,
            baseRotor,
            new[] { elevation });

        Assert.Same(baseRotor, hardware.Azimuth);
        Assert.Same(elevation, hardware.Elevation);
    }

    [Fact]
    public void Create_DualRotors_CommandsOppositePhysicalDirections()
    {
        SolarArrayHardwareFactory factory = new();
        FakeAxisHardware first = new();
        FakeAxisHardware second = new();
        SolarArrayHardware hardware = factory.Create(
            SolarArrayType.BaseRotorWithDualRotors,
            new FakeAxisHardware(),
            new[] { first, second });

        hardware.Elevation.SetVelocity(5.0);

        Assert.Equal(5.0, first.CommandedVelocity);
        Assert.Equal(-5.0, second.CommandedVelocity);
    }

    [Theory]
    [InlineData(SolarArrayType.BaseRotorWithHinge, 2)]
    [InlineData(SolarArrayType.BaseRotorWithRotor, 0)]
    [InlineData(SolarArrayType.BaseRotorWithDualRotors, 1)]
    [InlineData(SolarArrayType.BaseRotorWithDualRotors, 3)]
    public void Create_WithWrongActuatorCount_Throws(
        SolarArrayType type,
        int actuatorCount)
    {
        SolarArrayHardwareFactory factory = new();
        FakeAxisHardware[] actuators = Enumerable
            .Range(0, actuatorCount)
            .Select(_ => new FakeAxisHardware())
            .ToArray();

        Assert.Throws<ArgumentException>(() => factory.Create(
            type,
            new FakeAxisHardware(),
            actuators));
    }
}
