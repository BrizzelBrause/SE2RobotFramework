using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class LinearMotionProfileTests
{
    [Fact]
    public void Calculate_WithTargetDirection_ReturnsConstantMaximumSpeed()
    {
        LinearMotionProfile profile = new();
        MotionRequest request = new()
        {
            Direction = 1,
            RemainingDistance = 10.0,
            Limits = new MotionLimits { MaximumSpeed = 4.0 }
        };

        MotionState state = profile.Calculate(request);

        Assert.Equal(4.0, state.DesiredVelocity);
        Assert.Equal(0.0, state.DesiredAcceleration);
        Assert.Equal(0.0, state.DesiredJerk);
    }

    [Fact]
    public void Calculate_WithoutTargetDirection_ReturnsZeroSpeed()
    {
        LinearMotionProfile profile = new();
        MotionRequest request = new()
        {
            Direction = 0,
            Limits = new MotionLimits { MaximumSpeed = 4.0 }
        };

        MotionState state = profile.Calculate(request);

        Assert.Equal(0.0, state.DesiredVelocity);
    }

    [Fact]
    public void Factory_CreateLinear_ReturnsLinearMotionProfile()
    {
        MotionProfileFactory factory = new();

        IMotionProfile profile = factory.Create(MotionProfileType.Linear);

        Assert.IsType<LinearMotionProfile>(profile);
    }
}
