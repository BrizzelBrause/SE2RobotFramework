using SE2RobotFramework.Controllers;

namespace SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmControlService
{
    private readonly DrillArmMechanism _mechanism;
    private bool _hasActiveCommand;
    private double? _forearmOrientationReference;

    public DrillArmControlService(DrillArmMechanism mechanism)
    {
        _mechanism = mechanism ?? throw new ArgumentNullException(nameof(mechanism));
    }

    public DrillArmControlStatus Status { get; private set; } =
        DrillArmControlStatus.Stopped;

    public DrillArmTargets? ActiveTargets { get; private set; }

    public MechanismRuntimeState? LastRuntimeState { get; private set; }

    public bool IsForearmOrientationHoldEnabled =>
        _forearmOrientationReference.HasValue;

    public double ForearmOrientationErrorDegrees =>
        _forearmOrientationReference.HasValue
            ? GetCurrentForearmOrientation() - _forearmOrientationReference.Value
            : 0.0;

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

        UpdateForearmHingeTarget();

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

    public void EnableForearmOrientationHold()
    {
        _forearmOrientationReference = GetCurrentForearmOrientation();
    }

    public void DisableForearmOrientationHold()
    {
        _forearmOrientationReference = null;
    }

    private void StopInternal(DrillArmControlStatus status)
    {
        _mechanism.Stop();
        _hasActiveCommand = false;
        _forearmOrientationReference = null;
        Status = status;
    }

    private void UpdateForearmHingeTarget()
    {
        if (!_forearmOrientationReference.HasValue)
        {
            return;
        }

        double target =
            _forearmOrientationReference.Value -
            _mechanism.Axes.Shoulder.CurrentPosition -
            _mechanism.Axes.Elbow.CurrentPosition;

        _mechanism.Axes.ForearmHinge.SetTargetPosition(target);
    }

    private double GetCurrentForearmOrientation()
    {
        return
            _mechanism.Axes.Shoulder.CurrentPosition +
            _mechanism.Axes.Elbow.CurrentPosition +
            _mechanism.Axes.ForearmHinge.CurrentPosition;
    }
}
