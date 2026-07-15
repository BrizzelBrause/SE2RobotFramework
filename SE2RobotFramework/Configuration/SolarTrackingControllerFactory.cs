using SE2RobotFramework.Mechanisms.Solar;

namespace SE2RobotFramework.Configuration;

public class SolarTrackingControllerFactory
{
    private readonly SolarOrientationSolver _orientationSolver;

    public SolarTrackingControllerFactory()
        : this(new SolarOrientationSolver())
    {
    }

    public SolarTrackingControllerFactory(SolarOrientationSolver orientationSolver)
    {
        _orientationSolver = orientationSolver ??
            throw new ArgumentNullException(nameof(orientationSolver));
    }

    public SolarTrackingController Create(
        SolarArrayConfiguration configuration,
        SolarArrayMechanism mechanism)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(mechanism);
        configuration.Validate();

        if (configuration.Type != mechanism.Hardware.Type)
        {
            throw new ArgumentException(
                "The configuration does not match the solar array hardware type.",
                nameof(configuration));
        }

        return new SolarTrackingController(
            mechanism,
            configuration.TrackingFrame,
            _orientationSolver);
    }
}
