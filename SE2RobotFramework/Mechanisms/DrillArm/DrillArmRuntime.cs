using SE2RobotFramework.Controllers;
using SE2RobotFramework.Configuration;

namespace SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmRuntime
{
    public DrillArmRuntime(
        DrillArmMechanism mechanism,
        DrillArmControlService controlService,
        DrillArmManualInputController manualInputController,
        DrillArmConfiguration activeConfiguration)
    {
        Mechanism = mechanism ?? throw new ArgumentNullException(nameof(mechanism));
        ControlService = controlService ??
            throw new ArgumentNullException(nameof(controlService));
        ManualInputController = manualInputController ??
            throw new ArgumentNullException(nameof(manualInputController));
        ActiveConfiguration = activeConfiguration ??
            throw new ArgumentNullException(nameof(activeConfiguration));
    }

    public DrillArmMechanism Mechanism { get; }

    public DrillArmControlService ControlService { get; }

    public DrillArmManualInputController ManualInputController { get; private set; }

    public DrillArmConfiguration ActiveConfiguration { get; private set; }

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

    public DrillArmRuntimeSnapshot GetSnapshot()
    {
        return new DrillArmRuntimeSnapshot(
            Status,
            ControlService.LastRuntimeState ?? Mechanism.GetRuntimeState(),
            ControlService.ActiveTargets,
            ControlService.IsForearmOrientationHoldEnabled,
            ControlService.ForearmOrientationErrorDegrees);
    }

    internal void ReplaceManualInputController(
        DrillArmManualInputController manualInputController)
    {
        ManualInputController = manualInputController ??
            throw new ArgumentNullException(nameof(manualInputController));
    }

    internal void ReplaceActiveConfiguration(
        DrillArmConfiguration activeConfiguration)
    {
        ActiveConfiguration = activeConfiguration ??
            throw new ArgumentNullException(nameof(activeConfiguration));
    }
}
