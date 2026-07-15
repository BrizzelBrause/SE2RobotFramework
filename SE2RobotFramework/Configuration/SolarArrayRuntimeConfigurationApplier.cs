using SE2RobotFramework.Mechanisms.Solar;

namespace SE2RobotFramework.Configuration;

public class SolarArrayRuntimeConfigurationApplier
{
    private readonly SolarArrayConfigurationApplier _mechanismApplier;
    private readonly SolarTrackingControllerFactory _trackingControllerFactory;

    public SolarArrayRuntimeConfigurationApplier()
        : this(
            new SolarArrayConfigurationApplier(),
            new SolarTrackingControllerFactory())
    {
    }

    public SolarArrayRuntimeConfigurationApplier(
        SolarArrayConfigurationApplier mechanismApplier,
        SolarTrackingControllerFactory trackingControllerFactory)
    {
        _mechanismApplier = mechanismApplier ??
            throw new ArgumentNullException(nameof(mechanismApplier));
        _trackingControllerFactory = trackingControllerFactory ??
            throw new ArgumentNullException(nameof(trackingControllerFactory));
    }

    public void Apply(
        SolarArrayConfiguration configuration,
        SolarArrayRuntime runtime)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(runtime);
        configuration.Validate();

        runtime.Stop();
        _mechanismApplier.Apply(configuration, runtime.Mechanism);
        SolarTrackingController trackingController =
            _trackingControllerFactory.Create(configuration, runtime.Mechanism);
        SolarTrackingService trackingService = new(
            trackingController,
            runtime.SunDirectionProvider);
        runtime.ReplaceTrackingComponents(
            trackingController,
            trackingService);
        runtime.ReplaceActiveConfiguration(configuration);
    }
}
