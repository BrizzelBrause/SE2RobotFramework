using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.Solar;

namespace SE2RobotFramework.Configuration;

public class SolarArrayRuntimeFactory
{
    private readonly SolarArrayMechanismFactory _mechanismFactory;
    private readonly SolarTrackingControllerFactory _trackingControllerFactory;

    public SolarArrayRuntimeFactory()
        : this(
            new SolarArrayMechanismFactory(),
            new SolarTrackingControllerFactory())
    {
    }

    public SolarArrayRuntimeFactory(
        SolarArrayMechanismFactory mechanismFactory,
        SolarTrackingControllerFactory trackingControllerFactory)
    {
        _mechanismFactory = mechanismFactory ??
            throw new ArgumentNullException(nameof(mechanismFactory));
        _trackingControllerFactory = trackingControllerFactory ??
            throw new ArgumentNullException(nameof(trackingControllerFactory));
    }

    public SolarArrayRuntime Create(
        SolarArrayConfiguration configuration,
        IAxisHardware baseRotor,
        IEnumerable<IAxisHardware> elevationActuators,
        ISunDirectionProvider sunDirectionProvider)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(baseRotor);
        ArgumentNullException.ThrowIfNull(elevationActuators);
        ArgumentNullException.ThrowIfNull(sunDirectionProvider);
        configuration.Validate();

        SolarArrayMechanism mechanism = _mechanismFactory.Create(
            configuration,
            baseRotor,
            elevationActuators);
        SolarTrackingController trackingController =
            _trackingControllerFactory.Create(configuration, mechanism);
        SolarTrackingService trackingService = new(
            trackingController,
            sunDirectionProvider);

        return new SolarArrayRuntime(
            mechanism,
            trackingController,
            trackingService,
            sunDirectionProvider,
            configuration);
    }
}
