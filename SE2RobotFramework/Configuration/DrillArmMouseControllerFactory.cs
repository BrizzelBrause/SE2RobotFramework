using SE2RobotFramework.Mechanisms.DrillArm;

namespace SE2RobotFramework.Configuration;

public class DrillArmMouseControllerFactory
{
    public DrillArmMouseController Create(
        DrillArmConfiguration configuration,
        DrillArmMechanism mechanism,
        DrillArmControlService controlService)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(mechanism);
        ArgumentNullException.ThrowIfNull(controlService);
        configuration.Validate();

        return new DrillArmMouseController(
            mechanism,
            controlService,
            configuration.MouseControl);
    }
}
