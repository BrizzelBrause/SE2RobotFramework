using SE2RobotFramework.Mechanisms.DrillArm;
using SE2RobotFramework.Mechanisms.Solar;

namespace SE2RobotFramework.Configuration;

public class RuntimeConfigurationService
{
    private readonly FrameworkConfigurationSerializer _serializer;
    private readonly DrillArmRuntimeConfigurationApplier _drillArmApplier;
    private readonly SolarArrayRuntimeConfigurationApplier _solarArrayApplier;

    public RuntimeConfigurationService()
        : this(
            new FrameworkConfigurationSerializer(),
            new DrillArmRuntimeConfigurationApplier(),
            new SolarArrayRuntimeConfigurationApplier())
    {
    }

    public RuntimeConfigurationService(
        FrameworkConfigurationSerializer serializer,
        DrillArmRuntimeConfigurationApplier drillArmApplier,
        SolarArrayRuntimeConfigurationApplier solarArrayApplier)
    {
        _serializer = serializer ??
            throw new ArgumentNullException(nameof(serializer));
        _drillArmApplier = drillArmApplier ??
            throw new ArgumentNullException(nameof(drillArmApplier));
        _solarArrayApplier = solarArrayApplier ??
            throw new ArgumentNullException(nameof(solarArrayApplier));
    }

    public DrillArmConfiguration ApplyDrillArm(
        string json,
        DrillArmRuntime runtime)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        DrillArmConfiguration configuration =
            _serializer.DeserializeDrillArm(json);
        _drillArmApplier.Apply(configuration, runtime);
        return configuration;
    }

    public SolarArrayConfiguration ApplySolarArray(
        string json,
        SolarArrayRuntime runtime)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        SolarArrayConfiguration configuration =
            _serializer.DeserializeSolarArray(json);
        _solarArrayApplier.Apply(configuration, runtime);
        return configuration;
    }
}
