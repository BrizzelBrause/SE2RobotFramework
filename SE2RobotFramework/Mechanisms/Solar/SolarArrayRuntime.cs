using SE2RobotFramework.Controllers;

namespace SE2RobotFramework.Mechanisms.Solar;

public class SolarArrayRuntime
{
    public SolarArrayRuntime(
        SolarArrayMechanism mechanism,
        SolarTrackingController trackingController,
        SolarTrackingService trackingService)
    {
        Mechanism = mechanism ?? throw new ArgumentNullException(nameof(mechanism));
        TrackingController = trackingController ??
            throw new ArgumentNullException(nameof(trackingController));
        TrackingService = trackingService ??
            throw new ArgumentNullException(nameof(trackingService));
    }

    public SolarArrayMechanism Mechanism { get; }

    public SolarTrackingController TrackingController { get; }

    public SolarTrackingService TrackingService { get; }

    public SolarTrackingServiceStatus Status => TrackingService.Status;

    public SolarOrientation? LastOrientation => TrackingService.LastOrientation;

    public MechanismRuntimeState RuntimeState =>
        TrackingController.GetRuntimeState();

    public void Update(double deltaTime)
    {
        TrackingService.Update(deltaTime);
    }

    public void Stop()
    {
        TrackingService.Stop();
    }
}
