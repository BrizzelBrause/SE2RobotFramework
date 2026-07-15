using SE2RobotFramework.Controllers;

namespace SE2RobotFramework.Mechanisms.Solar;

public class SolarArrayRuntime
{
    public SolarArrayRuntime(
        SolarArrayMechanism mechanism,
        SolarTrackingController trackingController,
        SolarTrackingService trackingService,
        ISunDirectionProvider sunDirectionProvider)
    {
        Mechanism = mechanism ?? throw new ArgumentNullException(nameof(mechanism));
        TrackingController = trackingController ??
            throw new ArgumentNullException(nameof(trackingController));
        TrackingService = trackingService ??
            throw new ArgumentNullException(nameof(trackingService));
        SunDirectionProvider = sunDirectionProvider ??
            throw new ArgumentNullException(nameof(sunDirectionProvider));
    }

    public SolarArrayMechanism Mechanism { get; }

    public SolarTrackingController TrackingController { get; private set; }

    public SolarTrackingService TrackingService { get; private set; }

    public ISunDirectionProvider SunDirectionProvider { get; }

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

    internal void ReplaceTrackingComponents(
        SolarTrackingController trackingController,
        SolarTrackingService trackingService)
    {
        TrackingController = trackingController ??
            throw new ArgumentNullException(nameof(trackingController));
        TrackingService = trackingService ??
            throw new ArgumentNullException(nameof(trackingService));
    }
}
