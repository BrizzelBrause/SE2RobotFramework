namespace SE2RobotFramework.Motion;

public interface IMotionProfile
{
    MotionState Calculate(MotionRequest request);
}