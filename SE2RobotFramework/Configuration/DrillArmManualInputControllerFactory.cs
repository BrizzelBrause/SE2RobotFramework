using SE2RobotFramework.Mechanisms.DrillArm;

namespace SE2RobotFramework.Configuration;

public class DrillArmManualInputControllerFactory
{
    public DrillArmManualInputController Create(
        DrillArmConfiguration configuration,
        DrillArmMechanism mechanism,
        DrillArmControlService controlService)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(mechanism);
        ArgumentNullException.ThrowIfNull(controlService);
        configuration.Validate();

        DrillArmMouseController mouseController = new(
            mechanism,
            controlService,
            configuration.MouseControl);
        DrillArmKeyboardController keyboardController = new(
            mechanism,
            controlService,
            configuration.KeyboardControl);

        return new DrillArmManualInputController(
            mechanism,
            controlService,
            mouseController,
            keyboardController);
    }
}
