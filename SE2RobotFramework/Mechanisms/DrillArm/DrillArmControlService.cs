using SE2RobotFramework.Controllers;

namespace SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmControlService
{
    private readonly DrillArmMechanism _mechanism;
    private bool _hasActiveCommand;

    public DrillArmControlService(DrillArmMechanism mechanism)
    {
        _mechanism = mechanism ?? throw new ArgumentNullException(nameof(mechanism));
    }

    public DrillArmControlStatus Status { get; private set; } =
        DrillArmControlStatus.Stopped;

    public DrillArmTargets? ActiveTargets { get; private set; }

    public MechanismRuntimeState? LastRuntimeState { get; private set; }

    public void MoveTo(DrillArmTargets targets)
    {
        targets.Validate();
        _mechanism.SetTargets(targets);
        ActiveTargets = targets;
        _hasActiveCommand = true;
        Status = DrillArmControlStatus.Moving;
    }

    public void Update(double deltaTime)
    {
        if (!double.IsFinite(deltaTime) || deltaTime <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaTime));
        }

        if (!_hasActiveCommand)
        {
            return;
        }

        _mechanism.Update(deltaTime);
        LastRuntimeState = _mechanism.GetRuntimeState();

        if (LastRuntimeState.HasFault)
        {
            StopInternal(DrillArmControlStatus.Faulted);
            return;
        }

        if (LastRuntimeState.IsAtTarget)
        {
            _hasActiveCommand = false;
            Status = DrillArmControlStatus.AtTarget;
            return;
        }

        Status = LastRuntimeState.IsMoving
            ? DrillArmControlStatus.Moving
            : DrillArmControlStatus.Blocked;
    }

    public void Stop()
    {
        StopInternal(DrillArmControlStatus.Stopped);
    }

    private void StopInternal(DrillArmControlStatus status)
    {
        _mechanism.Stop();
        _hasActiveCommand = false;
        Status = status;
    }
}
