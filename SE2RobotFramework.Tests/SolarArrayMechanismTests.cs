using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.Solar;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class SolarArrayMechanismTests
{
    [Fact]
    public void Update_CommandsBothLogicalAxes()
    {
        FakeAxisHardware baseRotor = new();
        FakeAxisHardware elevationRotor = new();
        SolarArrayHardware hardware = new(
            SolarArrayType.BaseRotorWithRotor,
            baseRotor,
            elevationRotor);
        RotationalAxis azimuth = CreateAxis("Solar.Azimuth");
        RotationalAxis elevation = CreateAxis("Solar.Elevation");
        SolarArrayMechanism mechanism = new(
            hardware,
            azimuth,
            elevation,
            new MotionProfileFactory(),
            new MotionRequestFactory());

        mechanism.SetTargetOrientation(90.0, 30.0);
        mechanism.Update(0.1);

        Assert.Equal(4.0, baseRotor.CommandedVelocity);
        Assert.Equal(4.0, elevationRotor.CommandedVelocity);
    }

    [Fact]
    public void Update_DualRotors_CommandsMirroredElevation()
    {
        FakeAxisHardware first = new();
        FakeAxisHardware second = new();
        SolarArrayHardware hardware = new SolarArrayHardwareFactory().Create(
            SolarArrayType.BaseRotorWithDualRotors,
            new FakeAxisHardware(),
            new[] { first, second });
        SolarArrayMechanism mechanism = new(
            hardware,
            CreateAxis("Solar.Azimuth"),
            CreateAxis("Solar.Elevation"),
            new MotionProfileFactory(),
            new MotionRequestFactory());

        mechanism.SetTargetOrientation(0.0, 30.0);
        mechanism.Update(0.1);

        Assert.Equal(4.0, first.CommandedVelocity);
        Assert.Equal(-4.0, second.CommandedVelocity);
    }

    private static RotationalAxis CreateAxis(string name)
    {
        return new RotationalAxis
        {
            Name = name,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimits { MaximumSpeed = 4.0 },
            Tolerance = 0.1
        };
    }
}
