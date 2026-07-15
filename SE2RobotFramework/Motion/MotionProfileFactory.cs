namespace SE2RobotFramework.Motion;

public class MotionProfileFactory : IMotionProfileFactory
{
    public IMotionProfile Create(MotionProfileType profileType)
    {
        return profileType switch
        {
            MotionProfileType.SCurve => new SCurveMotionProfile(),

            MotionProfileType.Trapezoidal => new TrapezoidalMotionProfile(),

            _ => throw new ArgumentOutOfRangeException(nameof(profileType))
        };
    }
}