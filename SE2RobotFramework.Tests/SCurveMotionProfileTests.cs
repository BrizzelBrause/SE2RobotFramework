using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class SCurveMotionProfileTests
{
    [Fact]
    public void Calculate_FromRestTowardPositiveTarget_ReturnsPositiveJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 10.0,
            CurrentVelocity = 0.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 1.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(1.0, state.DesiredJerk, 10);
        Assert.Equal(0.1, state.DesiredAcceleration, 10);
        Assert.Equal(0.01, state.DesiredVelocity, 10);
    }

    [Fact]
    public void Calculate_WhenMovingAwayFromTarget_ReturnsJerkTowardStop()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 10.0,
            CurrentVelocity = 3.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 1.0
            },
            DeltaTime = 0.1,
            Direction = -1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-1.0, state.DesiredJerk, 10);
        Assert.Equal(-0.1, state.DesiredAcceleration, 10);
        Assert.Equal(2.99, state.DesiredVelocity, 10);
    }

    [Fact]
    public void Calculate_WhenTargetReached_ReturnsZeroJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 0.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 1.0
            },
            DeltaTime = 0.1,
            Direction = 0
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state =
            profile.Calculate(request);

        Assert.Equal(0.0, state.DesiredJerk, 10);
        Assert.Equal(0.0, state.DesiredAcceleration, 10);
        Assert.Equal(0.0, state.DesiredVelocity, 10);
    }

    [Fact]
    public void Calculate_WhenStoppingDistanceExceedsRemainingDistance_ReturnsNegativeJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 1.0,
            CurrentVelocity = 4.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 5.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-2.0, state.DesiredJerk, 10);
        Assert.True(state.DesiredAcceleration < 0.0);
        Assert.True(state.DesiredVelocity < request.CurrentVelocity);
    }

    [Fact]
    public void Calculate_WhenBrakingAndMaximumNegativeAccelerationReached_ReturnsZeroJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 1.0,
            CurrentVelocity = 10.0,
            CurrentAcceleration = -5.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 12.0,
                MaximumAcceleration = 5.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(0.0, state.DesiredJerk, 10);
        Assert.Equal(-5.0, state.DesiredAcceleration, 10);
        Assert.True(state.DesiredVelocity < request.CurrentVelocity);
    }

    [Fact]
    public void Calculate_WhenBrakingAndAccelerationMustBeReleased_ReturnsPositiveJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.2,
            CurrentVelocity = 0.5,
            CurrentAcceleration = -2.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(2.0, state.DesiredJerk, 10);

        Assert.True(state.DesiredAcceleration > request.CurrentAcceleration);

        Assert.True(state.DesiredAcceleration <= 0.0);

        Assert.True(state.DesiredVelocity < request.CurrentVelocity);
    }

    [Fact]
    public void Calculate_WhenFivePhaseBrakingReachesReleasePoint_ReturnsPositiveJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 0.75,
            CurrentAcceleration = -1.7320508075688772,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenFivePhaseBrakingInNegativeDirectionReachesReleasePoint_ReturnsNegativeJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = -0.75,
            CurrentAcceleration = 1.7320508075688772,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = -1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenFivePhaseBrakingStarts_ReturnsNegativeJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 1.5,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenFivePhaseBrakingHasNotReachedPeakDeceleration_ReturnsNegativeJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 1.49,
            CurrentAcceleration = -0.2,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenSevenPhaseBrakingBeforeReleasePoint_ReturnsZeroJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 2.0,
            CurrentAcceleration = -2.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(0.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenThreePhaseBrakingStarts_ReturnsNegativeJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 0.5,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 5.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenSevenPhaseBrakingStarts_ReturnsNegativeJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 10.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 12.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 1.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-1.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenThreePhaseBrakingReachesReleasePoint_ReturnsPositiveJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 0.25,
            CurrentAcceleration = -1.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 5.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenFivePhaseBrakingWithResidualAcceleration_ReturnsNegativeJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 1.5,
            CurrentAcceleration = -0.5,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenSevenPhaseBrakingWithResidualAcceleration_ReturnsNegativeJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 10.0,
            CurrentAcceleration = -0.5,
            Limits = new MotionLimits
            {
                MaximumSpeed = 12.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 1.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-1.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenSevenPhaseBrakingReachesReleasePoint_ReturnsPositiveJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 1.0,
            CurrentAcceleration = -2.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 12.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 1.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(1.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenFivePhaseBrakingHasReachedPeakDeceleration_ReturnsZeroJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 1.5,
            CurrentAcceleration = -2.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(0.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenSevenPhaseBrakingInNegativeDirectionReachesReleasePoint_ReturnsNegativeJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = -1.0,
            CurrentAcceleration = 2.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 12.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 1.0
            },
            DeltaTime = 0.1,
            Direction = -1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-1.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenSevenPhaseBrakingIsJustAboveReleasePoint_ReturnsZeroJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 1.00005,
            CurrentAcceleration = -2.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(0.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenBrakingMustReleaseAcceleration_ReturnsPositiveJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 0.5,
            CurrentAcceleration = -2.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenBrakingAndAccelerationIsReleasing_ReturnsIncreasingAcceleration()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 0.5,
            CurrentAcceleration = -2.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.True(state.DesiredAcceleration > request.CurrentAcceleration);
        Assert.True(state.DesiredAcceleration < 0.0);
    }

    [Fact]
    public void Calculate_WhenAcceleratingAndMaximumAccelerationReached_ReturnsZeroJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 100.0,
            CurrentVelocity = 1.0,
            CurrentAcceleration = 2.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(0.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenAccelerationMustBeReleasedBeforeMaximumSpeed_ReturnsNegativeJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 100.0,
            CurrentVelocity = 4.8,
            CurrentAcceleration = 2.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenCruisingAndAccelerationIsPositive_ReturnsNegativeJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 100.0,
            CurrentVelocity = 5.0,
            CurrentAcceleration = 1.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_FromRestTowardNegativeTarget_ReturnsNegativeJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 100.0,
            CurrentVelocity = 0.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = -1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenAcceleratingNegativeAndAccelerationMustBeReleased_ReturnsPositiveJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 100.0,
            CurrentVelocity = -4.8,
            CurrentAcceleration = -2.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = -1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenBrakingInNegativeDirection_ReturnsPositiveJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 1.0,
            CurrentVelocity = -4.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = -1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenBrakingNegativeAndAccelerationMustBeReleased_ReturnsNegativeJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = -0.5,
            CurrentAcceleration = 2.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = -1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenBrakingNegativeAndMaximumAccelerationReached_ReturnsZeroJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 1.0,
            CurrentVelocity = -10.0,
            CurrentAcceleration = 2.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 12.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = -1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(0.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenCruisingNegativeAndAccelerationIsNegative_ReturnsPositiveJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 100.0,
            CurrentVelocity = -5.0,
            CurrentAcceleration = -1.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = -1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenAccelerationReleaseInNegativeDirection_UpdatesStateCorrectly()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 100.0,
            CurrentVelocity = -4.8,
            CurrentAcceleration = -2.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = -1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(2.0, state.DesiredJerk, 10);
        Assert.True(state.DesiredAcceleration > request.CurrentAcceleration);
        Assert.True(state.DesiredAcceleration < 0.0);
        Assert.True(state.DesiredVelocity < request.CurrentVelocity);
    }

    [Fact]
    public void Calculate_WhenCruisingAndAccelerationIsZero_ReturnsZeroJerk()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 100.0,
            CurrentVelocity = 5.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(0.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Calculate_WhenMovingAwayFromTarget_ReturnsJerkAgainstCurrentVelocity()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 100.0,
            CurrentVelocity = -4.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(2.0, state.DesiredJerk, 10);
        Assert.True(state.DesiredAcceleration > 0.0);
        Assert.True(state.DesiredVelocity > request.CurrentVelocity);
    }

    [Fact]
    public void Calculate_WhenAtTargetAndStopped_ReturnsIdleState()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 0.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 0
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(0.0, state.DesiredJerk, 10);
        Assert.Equal(0.0, state.DesiredAcceleration, 10);
        Assert.Equal(0.0, state.DesiredVelocity, 10);
    }

    [Fact]
    public void Calculate_WhenAtTargetButStillMoving_ReturnsJerkAgainstCurrentVelocity()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 1.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 0
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-2.0, state.DesiredJerk, 10);
        Assert.True(state.DesiredAcceleration < 0.0);
        Assert.True(state.DesiredVelocity < request.CurrentVelocity);
    }

    [Fact]
    public void Calculate_WhenAtTargetButMovingNegative_ReturnsJerkAgainstCurrentVelocity()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = -1.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 0
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(2.0, state.DesiredJerk, 10);
        Assert.True(state.DesiredAcceleration > 0.0);
        Assert.True(state.DesiredVelocity > request.CurrentVelocity);
    }

    [Fact]
    public void Calculate_WhenAtTargetAndVelocityIsZeroButAccelerationRemains_ReleasesAcceleration()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 0.0,
            CurrentAcceleration = 1.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 0
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-2.0, state.DesiredJerk, 10);
        Assert.True(state.DesiredAcceleration < request.CurrentAcceleration);
    }

    [Fact]
    public void Calculate_WhenCruisingAndAccelerationWouldCrossZero_StopsAccelerationAtZero()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 100.0,
            CurrentVelocity = 5.0,
            CurrentAcceleration = 0.1,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-1.0, state.DesiredJerk, 10);
        Assert.Equal(0.0, state.DesiredAcceleration, 10);
    }

    [Fact]
    public void Calculate_WhenBrakingReleaseWouldCrossZero_StopsAccelerationAtZero()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 0.001,
            CurrentAcceleration = -0.1,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 0
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(1.0, state.DesiredJerk, 10);
        Assert.Equal(0.0, state.DesiredAcceleration, 10);
    }

    [Fact]
    public void Calculate_WhenAccelerationReleaseWouldCrossZero_StopsAccelerationAtZero()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 100.0,
            CurrentVelocity = 4.999,
            CurrentAcceleration = 0.1,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-1.0, state.DesiredJerk, 10);
        Assert.Equal(0.0, state.DesiredAcceleration, 10);
    }

    [Fact]
    public void Calculate_WhenAtTargetAndAccelerationReleaseWouldCrossZero_StopsAccelerationAtZero()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 0.0,
            CurrentVelocity = 0.0,
            CurrentAcceleration = 0.1,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 0
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        MotionState state = profile.Calculate(request);

        Assert.Equal(-1.0, state.DesiredJerk, 10);
        Assert.Equal(0.0, state.DesiredAcceleration, 10);
    }

    [Fact]
    public void Calculate_WhenDeltaTimeIsZero_ThrowsArgumentOutOfRangeException()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 100.0,
            CurrentVelocity = 5.0,
            CurrentAcceleration = 0.1,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.0,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        Assert.Throws<ArgumentOutOfRangeException>(() => profile.Calculate(request));
    }

    [Fact]
    public void Calculate_WhenMaximumJerkIsZero_ThrowsArgumentOutOfRangeException()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 100.0,
            CurrentVelocity = 1.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 2.0,
                MaximumJerk = 0.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        Assert.Throws<ArgumentOutOfRangeException>(() => profile.Calculate(request));
    }

    [Fact]
    public void Calculate_WhenMaximumAccelerationIsZero_ThrowsArgumentOutOfRangeException()
    {
        MotionRequest request = new MotionRequest
        {
            RemainingDistance = 100.0,
            CurrentVelocity = 1.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 5.0,
                MaximumAcceleration = 0.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1,
            Direction = 1
        };

        SCurveMotionProfile profile = new SCurveMotionProfile();

        Assert.Throws<ArgumentOutOfRangeException>(() => profile.Calculate(request));
    }
}