using System.Numerics;

namespace SE2RobotFramework.Mechanisms.Solar;

public class SolarTrackingController
{
    private readonly SolarArrayMechanism _mechanism;
    private readonly SolarOrientationSolver _orientationSolver;

    public SolarTrackingController(
        SolarArrayMechanism mechanism,
        SolarTrackingFrame frame,
        SolarOrientationSolver orientationSolver)
    {
        _mechanism = mechanism ?? throw new ArgumentNullException(nameof(mechanism));
        Frame = frame ?? throw new ArgumentNullException(nameof(frame));
        _orientationSolver = orientationSolver ??
            throw new ArgumentNullException(nameof(orientationSolver));
        Frame.Validate();
    }

    public SolarTrackingFrame Frame { get; }

    public SolarOrientation SetSunDirection(Vector3 sunDirection)
    {
        SolarOrientation orientation = _orientationSolver.Calculate(
            sunDirection,
            Frame);

        _mechanism.SetTargetOrientation(
            orientation.AzimuthDegrees,
            orientation.ElevationDegrees);

        return orientation;
    }

    public void Update(double deltaTime)
    {
        _mechanism.Update(deltaTime);
    }
}
