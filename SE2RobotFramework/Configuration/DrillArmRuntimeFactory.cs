using SE2RobotFramework.Mechanisms.DrillArm;

namespace SE2RobotFramework.Configuration;

public class DrillArmRuntimeFactory
{
    private readonly DrillArmMechanismFactory _mechanismFactory;
    private readonly DrillArmManualInputControllerFactory _manualInputFactory;

    public DrillArmRuntimeFactory()
        : this(
            new DrillArmMechanismFactory(),
            new DrillArmManualInputControllerFactory())
    {
    }

    public DrillArmRuntimeFactory(
        DrillArmMechanismFactory mechanismFactory,
        DrillArmManualInputControllerFactory manualInputFactory)
    {
        _mechanismFactory = mechanismFactory ??
            throw new ArgumentNullException(nameof(mechanismFactory));
        _manualInputFactory = manualInputFactory ??
            throw new ArgumentNullException(nameof(manualInputFactory));
    }

    public DrillArmRuntime Create(
        DrillArmConfiguration configuration,
        DrillArmHardware hardware)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(hardware);
        configuration.Validate();

        DrillArmMechanism mechanism = _mechanismFactory.Create(
            configuration,
            hardware);
        DrillArmControlService controlService = new(mechanism);
        DrillArmManualInputController manualInputController =
            _manualInputFactory.Create(
                configuration,
                mechanism,
                controlService);

        return new DrillArmRuntime(
            mechanism,
            controlService,
            manualInputController);
    }
}
