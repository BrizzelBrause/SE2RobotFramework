using SE2RobotFramework.Mechanisms.DrillArm;

namespace SE2RobotFramework.Configuration;

public class DrillArmRuntimeConfigurationApplier
{
    private readonly DrillArmConfigurationApplier _mechanismApplier;
    private readonly DrillArmManualInputControllerFactory _manualInputFactory;

    public DrillArmRuntimeConfigurationApplier()
        : this(
            new DrillArmConfigurationApplier(),
            new DrillArmManualInputControllerFactory())
    {
    }

    public DrillArmRuntimeConfigurationApplier(
        DrillArmConfigurationApplier mechanismApplier,
        DrillArmManualInputControllerFactory manualInputFactory)
    {
        _mechanismApplier = mechanismApplier ??
            throw new ArgumentNullException(nameof(mechanismApplier));
        _manualInputFactory = manualInputFactory ??
            throw new ArgumentNullException(nameof(manualInputFactory));
    }

    public void Apply(
        DrillArmConfiguration configuration,
        DrillArmRuntime runtime)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(runtime);
        configuration.Validate();

        runtime.Stop();
        _mechanismApplier.Apply(configuration, runtime.Mechanism);
        DrillArmManualInputController manualInputController =
            _manualInputFactory.Create(
                configuration,
                runtime.Mechanism,
                runtime.ControlService);
        runtime.ReplaceManualInputController(manualInputController);
    }
}
