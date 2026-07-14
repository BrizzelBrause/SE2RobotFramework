namespace SE2RobotFramework.Motion;

public class MotionRequest
{
    public double RemainingDistance { get; init; }

    public double MaximumSpeed { get; init; }

    public double MaximumAcceleration { get; init; }

    public double MaximumJerk { get; init; }

    public double DeltaTime { get; init; }

    public double CurrentVelocity { get; init; }

    public double CurrentAcceleration { get; init; }

    public int Direction { get; init; }
}