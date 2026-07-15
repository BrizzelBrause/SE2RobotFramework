using SE2RobotFramework.Controllers;
using SE2RobotFramework.Configuration;

namespace SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmRuntime
{
    public DrillArmRuntime(
        DrillArmMechanism mechanism,
        DrillArmControlService controlService,
        DrillArmManualInputController manualInputController,
        DrillArmConfiguration activeConfiguration,
        SwitchableController? drillHeadController)
    {
        Mechanism = mechanism ?? throw new ArgumentNullException(nameof(mechanism));
        ControlService = controlService ??
            throw new ArgumentNullException(nameof(controlService));
        ManualInputController = manualInputController ??
            throw new ArgumentNullException(nameof(manualInputController));
        ActiveConfiguration = activeConfiguration ??
            throw new ArgumentNullException(nameof(activeConfiguration));
        DrillHeadController = drillHeadController;
    }

    public DrillArmMechanism Mechanism { get; }

    public DrillArmControlService ControlService { get; }

    public DrillArmManualInputController ManualInputController { get; private set; }

    public DrillArmConfiguration ActiveConfiguration { get; private set; }

    public SwitchableController? DrillHeadController { get; }

    public DrillArmControlStatus Status => ControlService.Status;

    public MechanismRuntimeState? LastRuntimeState =>
        ControlService.LastRuntimeState;

    public DrillArmManualInputResult ProcessManualInput(
        DrillArmManualInput input,
        double deltaTime)
    {
        DrillArmManualInputResult result;
        try
        {
            result = ManualInputController.Process(input, deltaTime);
        }
        catch
        {
            DrillHeadController?.Stop();
            throw;
        }

        if (result.Status == DrillArmControlStatus.Faulted)
        {
            DrillHeadController?.Stop();
            return result with
            {
                IsDrillHeadCommandAccepted = DrillHeadController is null
                    ? null
                    : false,
                DrillHeadStatus = DrillHeadController?.Status
            };
        }

        bool? isDrillHeadCommandAccepted = DrillHeadController is null
            ? null
            : SetDrillHeadEnabled(input.DrillHeadEnabled);
        return result with
        {
            IsDrillHeadCommandAccepted = isDrillHeadCommandAccepted,
            DrillHeadStatus = DrillHeadController?.Status
        };
    }

    public void Stop()
    {
        ControlService.Stop();
        DrillHeadController?.Stop();
    }

    public bool SetDrillHeadEnabled(bool enabled)
    {
        return DrillHeadController?.SetEnabled(enabled) ?? false;
    }

    public DrillArmRuntimeSnapshot GetSnapshot()
    {
        return new DrillArmRuntimeSnapshot(
            Status,
            ControlService.LastRuntimeState ?? Mechanism.GetRuntimeState(),
            ControlService.ActiveTargets,
            ControlService.IsForearmOrientationHoldEnabled,
            ControlService.IsForearmCompensationLimitLatched,
            ControlService.ForearmOrientationErrorDegrees,
            DrillHeadController?.Status);
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
