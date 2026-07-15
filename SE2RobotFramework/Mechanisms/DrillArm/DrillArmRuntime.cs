using SE2RobotFramework.Controllers;

namespace SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmRuntime
{
    public DrillArmRuntime(
        DrillArmMechanism mechanism,
        DrillArmControlService controlService,
        DrillArmManualInputController manualInputController)
    {
        Mechanism = mechanism ?? throw new ArgumentNullException(nameof(mechanism));
        ControlService = controlService ??
            throw new ArgumentNullException(nameof(controlService));
        ManualInputController = manualInputController ??
            throw new ArgumentNullException(nameof(manualInputController));
    }

    public DrillArmMechanism Mechanism { get; }

    public DrillArmControlService ControlService { get; }

    public DrillArmManualInputController ManualInputController { get; }

    public DrillArmControlStatus Status => ControlService.Status;

    public MechanismRuntimeState? LastRuntimeState =>
        ControlService.LastRuntimeState;

    public DrillArmManualInputResult ProcessManualInput(
        DrillArmManualInput input,
        double deltaTime)
    {
        return ManualInputController.Process(input, deltaTime);
    }

    public void Stop()
    {
        ControlService.Stop();
    }
}
