using System.Numerics;
using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.Solar;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class SolarTrackingControllerTests
{
    [Fact]
    public void SetSunDirection_UpdatesMechanismTargets()
    {
        SolarArrayMechanism mechanism = CreateMechanism();
        SolarTrackingController controller = new(
            mechanism,
            new SolarTrackingFrame(),
            new SolarOrientationSolver());

        SolarOrientation orientation = controller.SetSunDirection(
            Vector3.Normalize(new Vector3(1.0f, 1.0f, 1.0f)));

        Assert.Equal(45.0, orientation.AzimuthDegrees, 5);
        Assert.Equal(45.0, mechanism.AzimuthAxis.TargetPosition, 5);
        Assert.Equal(orientation.ElevationDegrees, mechanism.ElevationAxis.TargetPosition, 5);
    }

    [Fact]
    public void Update_AfterSunDirection_CommandsBothAxes()
    {
        FakeAxisHardware azimuthHardware = new();
        FakeAxisHardware elevationHardware = new();
        SolarArrayMechanism mechanism = CreateMechanism(
            azimuthHardware,
            elevationHardware);
        SolarTrackingController controller = new(
            mechanism,
            new SolarTrackingFrame(),
            new SolarOrientationSolver());

        controller.SetSunDirection(Vector3.Normalize(new Vector3(1.0f, 1.0f, 1.0f)));
        controller.Update(0.1);

        Assert.Equal(5.0, azimuthHardware.CommandedVelocity);
        Assert.Equal(5.0, elevationHardware.CommandedVelocity);
    }

    private static SolarArrayMechanism CreateMechanism(
        FakeAxisHardware? azimuthHardware = null,
        FakeAxisHardware? elevationHardware = null)
    {
        return new SolarArrayMechanism(
            new SolarArrayHardware(
                SolarArrayType.BaseRotorWithRotor,
                azimuthHardware ?? new FakeAxisHardware(),
                elevationHardware ?? new FakeAxisHardware()),
            CreateAxis("Solar.Azimuth"),
            CreateAxis("Solar.Elevation"),
            new MotionProfileFactory(),
            new MotionRequestFactory());
    }

    private static RotationalAxis CreateAxis(string name)
    {
        return new RotationalAxis
        {
            Name = name,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimits { MaximumSpeed = 5.0 },
            Tolerance = 0.1,
            WrapAround = true
        };
    }
}
