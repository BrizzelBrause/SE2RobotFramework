using System.Numerics;

namespace SE2RobotFramework.Mechanisms.Solar;

public class SolarTrackingService
{
    private readonly SolarTrackingController _trackingController;
    private readonly ISunDirectionProvider _sunDirectionProvider;

    public SolarTrackingService(
        SolarTrackingController trackingController,
        ISunDirectionProvider sunDirectionProvider)
    {
        _trackingController = trackingController ??
            throw new ArgumentNullException(nameof(trackingController));
        _sunDirectionProvider = sunDirectionProvider ??
            throw new ArgumentNullException(nameof(sunDirectionProvider));
    }

    public SolarTrackingServiceStatus Status { get; private set; } =
        SolarTrackingServiceStatus.Stopped;

    public SolarOrientation? LastOrientation { get; private set; }

    public void Update(double deltaTime)
    {
        if (!double.IsFinite(deltaTime) || deltaTime <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaTime));
        }

        if (!_sunDirectionProvider.TryGetSunDirection(out Vector3 sunDirection))
        {
            Stop(SolarTrackingServiceStatus.SunDirectionUnavailable);
            return;
        }

        try
        {
            LastOrientation = _trackingController.SetSunDirection(sunDirection);
        }
        catch (ArgumentException)
        {
            Stop(SolarTrackingServiceStatus.InvalidSunDirection);
            return;
        }

        _trackingController.Update(deltaTime);

        if (_trackingController.GetRuntimeState().HasFault)
        {
            Stop(SolarTrackingServiceStatus.MechanismFault);
            return;
        }

        Status = SolarTrackingServiceStatus.Tracking;
    }

    public void Stop()
    {
        Stop(SolarTrackingServiceStatus.Stopped);
    }

    private void Stop(SolarTrackingServiceStatus status)
    {
        _trackingController.Stop();
        Status = status;
    }
}
