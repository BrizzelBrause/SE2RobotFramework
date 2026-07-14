namespace SE2RobotFramework.Motion;

public class MotionState
{
    public double DesiredVelocity { get; init; }

    public double DesiredAcceleration { get; init; }

    public double DesiredJerk { get; init; }

    public double RemainingDistance { get; init; }

    public double StoppingDistance { get; init; }

    public bool IsBraking { get; init; }

    public bool IsMovingTowardTarget { get; init; }
}