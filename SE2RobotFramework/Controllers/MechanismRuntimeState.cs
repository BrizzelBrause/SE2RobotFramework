namespace SE2RobotFramework.Controllers;

public class MechanismRuntimeState
{
    public MechanismRuntimeState(IEnumerable<AxisRuntimeState> axes)
    {
        ArgumentNullException.ThrowIfNull(axes);
        Axes = axes.ToArray();

        if (Axes.Count == 0)
        {
            throw new ArgumentException(
                "At least one axis state is required.",
                nameof(axes));
        }
    }

    public IReadOnlyList<AxisRuntimeState> Axes { get; }

    public bool HasFault => Axes.Any(axis => axis.ControllerStatus is
        MotionControllerStatus.HardwareUnavailable or
        MotionControllerStatus.InvalidFeedback or
        MotionControllerStatus.SynchronizationLost);

    public bool IsMoving => Axes.Any(axis =>
        axis.ControllerStatus == MotionControllerStatus.Moving);

    public bool IsAtTarget => Axes.All(axis =>
        axis.ControllerStatus == MotionControllerStatus.AtTarget);
}
