namespace SE2RobotFramework.Motion;

public interface IMotionProfileFactory
{
    IMotionProfile Create(MotionProfileType profileType);
}