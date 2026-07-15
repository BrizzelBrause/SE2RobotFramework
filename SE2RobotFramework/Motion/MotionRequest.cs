namespace SE2RobotFramework.Motion;

public class MotionRequest
{
    public double RemainingDistance { get; init; }

    public MotionLimits Limits { get; init; } = new();

    public double DeltaTime { get; init; }

    public double CurrentVelocity { get; init; }

    public double CurrentAcceleration { get; init; }

    public int Direction { get; init; }
}