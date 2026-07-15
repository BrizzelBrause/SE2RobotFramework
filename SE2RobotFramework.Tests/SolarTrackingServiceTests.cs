using System.Numerics;
using SE2RobotFramework.Controllers;
using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.Solar;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class SolarTrackingServiceTests
{
    [Fact]
    public void Update_WithSunDirection_TracksSun()
    {
        FakeAxisHardware azimuth = new();
        FakeAxisHardware elevation = new();
        TestSunDirectionProvider provider = new()
        {
            Direction = Vector3.Normalize(new Vector3(1.0f, 1.0f, 1.0f)),
            IsAvailable = true
        };
        SolarTrackingService service = CreateService(provider, azimuth, elevation);

        service.Update(0.1);

        Assert.Equal(SolarTrackingServiceStatus.Tracking, service.Status);
        Assert.NotNull(service.LastOrientation);
        Assert.Equal(3.0, azimuth.CommandedVelocity);
        Assert.Equal(3.0, elevation.CommandedVelocity);
    }

    [Fact]
    public void Update_WhenSunBecomesUnavailable_StopsBothAxes()
    {
        FakeAxisHardware azimuth = new();
        FakeAxisHardware elevation = new();
        TestSunDirectionProvider provider = new()
        {
            Direction = Vector3.Normalize(new Vector3(1.0f, 1.0f, 1.0f)),
            IsAvailable = true
        };
        SolarTrackingService service = CreateService(provider, azimuth, elevation);
        service.Update(0.1);
        provider.IsAvailable = false;

        service.Update(0.1);

        Assert.Equal(
            SolarTrackingServiceStatus.SunDirectionUnavailable,
            service.Status);
        Assert.Equal(0.0, azimuth.CommandedVelocity);
        Assert.Equal(0.0, elevation.CommandedVelocity);
    }

    [Fact]
    public void Update_WithInvalidSunDirection_StopsMechanism()
    {
        TestSunDirectionProvider provider = new()
        {
            Direction = Vector3.Zero,
            IsAvailable = true
        };
        SolarTrackingService service = CreateService(
            provider,
            new FakeAxisHardware(),
            new FakeAxisHardware());

        service.Update(0.1);

        Assert.Equal(
            SolarTrackingServiceStatus.InvalidSunDirection,
            service.Status);
    }

    [Fact]
    public void Update_WithHardwareFault_StopsAndReportsFault()
    {
        FakeAxisHardware azimuth = new() { IsAvailable = false };
        TestSunDirectionProvider provider = new()
        {
            Direction = Vector3.Normalize(new Vector3(1.0f, 1.0f, 1.0f)),
            IsAvailable = true
        };
        SolarTrackingService service = CreateService(
            provider,
            azimuth,
            new FakeAxisHardware());

        service.Update(0.1);

        Assert.Equal(SolarTrackingServiceStatus.MechanismFault, service.Status);
    }

    [Fact]
    public void Stop_SetsControllerAxesToStopped()
    {
        TestSunDirectionProvider provider = new();
        SolarTrackingController controller = CreateController(
            new FakeAxisHardware(),
            new FakeAxisHardware());
        SolarTrackingService service = new(controller, provider);

        service.Stop();

        Assert.Equal(SolarTrackingServiceStatus.Stopped, service.Status);
        Assert.All(
            controller.GetRuntimeState().Axes,
            axis => Assert.Equal(MotionControllerStatus.Stopped, axis.ControllerStatus));
    }

    private static SolarTrackingService CreateService(
        ISunDirectionProvider provider,
        FakeAxisHardware azimuth,
        FakeAxisHardware elevation)
    {
        return new SolarTrackingService(
            CreateController(azimuth, elevation),
            provider);
    }

    private static SolarTrackingController CreateController(
        FakeAxisHardware azimuth,
        FakeAxisHardware elevation)
    {
        SolarArrayMechanism mechanism = new(
            new SolarArrayHardware(
                SolarArrayType.BaseRotorWithRotor,
                azimuth,
                elevation),
            CreateAxis("Solar.Azimuth"),
            CreateAxis("Solar.Elevation"),
            new MotionProfileFactory(),
            new MotionRequestFactory());

        return new SolarTrackingController(
            mechanism,
            new SolarTrackingFrame(),
            new SolarOrientationSolver());
    }

    private static RotationalAxis CreateAxis(string name)
    {
        return new RotationalAxis
        {
            Name = name,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimits { MaximumSpeed = 3.0 },
            Tolerance = 0.1,
            WrapAround = true
        };
    }

    private sealed class TestSunDirectionProvider : ISunDirectionProvider
    {
        public Vector3 Direction { get; set; } = Vector3.UnitX;

        public bool IsAvailable { get; set; }

        public bool TryGetSunDirection(out Vector3 sunDirection)
        {
            sunDirection = Direction;
            return IsAvailable;
        }
    }
}
