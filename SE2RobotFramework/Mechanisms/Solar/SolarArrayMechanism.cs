using SE2RobotFramework.Controllers;
using SE2RobotFramework.Core;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Mechanisms.Solar;

public class SolarArrayMechanism
{
    private readonly MotionController _azimuthController;
    private readonly MotionController _elevationController;

    public SolarArrayMechanism(
        SolarArrayHardware hardware,
        RotationalAxis azimuthAxis,
        RotationalAxis elevationAxis,
        IMotionProfileFactory profileFactory,
        MotionRequestFactory requestFactory)
    {
        Hardware = hardware ?? throw new ArgumentNullException(nameof(hardware));
        AzimuthAxis = azimuthAxis ?? throw new ArgumentNullException(nameof(azimuthAxis));
        ElevationAxis = elevationAxis ?? throw new ArgumentNullException(nameof(elevationAxis));
        ArgumentNullException.ThrowIfNull(profileFactory);
        ArgumentNullException.ThrowIfNull(requestFactory);

        _azimuthController = new MotionController(
            AzimuthAxis,
            Hardware.Azimuth,
            profileFactory,
            requestFactory);
        _elevationController = new MotionController(
            ElevationAxis,
            Hardware.Elevation,
            profileFactory,
            requestFactory);
    }

    public SolarArrayHardware Hardware { get; }

    public RotationalAxis AzimuthAxis { get; }

    public RotationalAxis ElevationAxis { get; }

    public void SetTargetOrientation(double azimuth, double elevation)
    {
        AzimuthAxis.SetTargetPosition(azimuth);
        ElevationAxis.SetTargetPosition(elevation);
    }

    public void Update(double deltaTime)
    {
        _azimuthController.Update(deltaTime);
        _elevationController.Update(deltaTime);
    }

    public MechanismRuntimeState GetRuntimeState()
    {
        return new MechanismRuntimeState(new[]
        {
            _azimuthController.GetRuntimeState(),
            _elevationController.GetRuntimeState()
        });
    }

    public void Stop()
    {
        _azimuthController.Stop();
        _elevationController.Stop();
    }
}
