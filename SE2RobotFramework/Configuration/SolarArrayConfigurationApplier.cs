using SE2RobotFramework.Mechanisms.Solar;
using SE2RobotFramework.Hardware;

namespace SE2RobotFramework.Configuration;

public class SolarArrayConfigurationApplier
{
    private readonly AxisConfigurationApplier _axisApplier;

    public SolarArrayConfigurationApplier()
        : this(new AxisConfigurationApplier())
    {
    }

    public SolarArrayConfigurationApplier(AxisConfigurationApplier axisApplier)
    {
        _axisApplier = axisApplier ?? throw new ArgumentNullException(nameof(axisApplier));
    }

    public void Apply(
        SolarArrayConfiguration configuration,
        SolarArrayMechanism mechanism)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(mechanism);
        configuration.Validate();

        if (configuration.Type != mechanism.Hardware.Type)
        {
            throw new InvalidOperationException(
                "Changing the solar array hardware type requires rebuilding its hardware binding.");
        }

        if (mechanism.Hardware.Elevation is ParallelAxisHardware parallelHardware &&
            (configuration.MaximumElevationSynchronizationError ??
                double.PositiveInfinity) != parallelHardware.MaximumPositionDeviation)
        {
            throw new InvalidOperationException(
                "Changing the synchronization limit requires rebuilding the solar hardware binding.");
        }

        _axisApplier.Apply(configuration.AzimuthAxis, mechanism.AzimuthAxis);
        _axisApplier.Apply(configuration.ElevationAxis, mechanism.ElevationAxis);
    }
}
