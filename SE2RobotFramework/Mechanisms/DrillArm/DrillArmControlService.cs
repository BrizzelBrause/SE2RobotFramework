using SE2RobotFramework.Controllers;

namespace SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmControlService
{
    private readonly DrillArmMechanism _mechanism;
    private bool _hasActiveCommand;
    private double? _forearmOrientationReference;
    private double? _forearmCompensationLimit;

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

    public bool IsForearmCompensationLimitLatched =>
        _forearmCompensationLimit.HasValue;

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
        _forearmCompensationLimit = null;
    }

    public void DisableForearmOrientationHold()
    {
        _forearmOrientationReference = null;
        _forearmCompensationLimit = null;
    }

    private void StopInternal(DrillArmControlStatus status)
    {
        _mechanism.Stop();
        _hasActiveCommand = false;
        _forearmOrientationReference = null;
        _forearmCompensationLimit = null;
        Status = status;
    }

    private void UpdateForearmHingeTarget()
    {
        if (!_forearmOrientationReference.HasValue)
        {
            return;
        }

        if (_forearmCompensationLimit.HasValue)
        {
            _mechanism.Axes.ForearmHinge.SetTargetPosition(
                _forearmCompensationLimit.Value);
            return;
        }

        double target =
            _forearmOrientationReference.Value -
            _mechanism.Axes.Shoulder.CurrentPosition -
            _mechanism.Axes.Elbow.CurrentPosition;

        if (target < _mechanism.Axes.ForearmHinge.MinimumPosition)
        {
            _forearmCompensationLimit =
                _mechanism.Axes.ForearmHinge.MinimumPosition;
        }
        else if (target > _mechanism.Axes.ForearmHinge.MaximumPosition)
        {
            _forearmCompensationLimit =
                _mechanism.Axes.ForearmHinge.MaximumPosition;
        }

        _mechanism.Axes.ForearmHinge.SetTargetPosition(
            _forearmCompensationLimit ?? target);
    }

    private double GetCurrentForearmOrientation()
    {
        return
            _mechanism.Axes.Shoulder.CurrentPosition +
            _mechanism.Axes.Elbow.CurrentPosition +
            _mechanism.Axes.ForearmHinge.CurrentPosition;
    }
}
