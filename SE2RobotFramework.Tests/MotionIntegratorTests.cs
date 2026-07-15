using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class MotionIntegratorTests
{
    [Fact]
    public void Integrate_WithPositiveJerk_IncreasesAccelerationAndVelocity()
    {
        MotionRequest request = new MotionRequest
        {
            CurrentVelocity = 0.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 10.0,
                MaximumAcceleration = 5.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1
        };

        MotionIntegrator integrator = new MotionIntegrator();

        MotionState state = integrator.Integrate(request, desiredJerk: 2.0);

        Assert.Equal(0.2, state.DesiredAcceleration, 10);
        Assert.Equal(0.02, state.DesiredVelocity, 10);
        Assert.Equal(2.0, state.DesiredJerk, 10);
    }

    [Fact]
    public void Integrate_WithJerkAboveLimit_ClampsJerk()
    {
        MotionRequest request = new MotionRequest
        {
            CurrentVelocity = 0.0,
            CurrentAcceleration = 0.0,
            Limits = new MotionLimits
            {
                MaximumSpeed = 10.0,
                MaximumAcceleration = 5.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1
        };

        MotionIntegrator integrator = new MotionIntegrator();

        MotionState state = integrator.Integrate(request, desiredJerk: 10.0);

        Assert.Equal(2.0, state.DesiredJerk, 10);
        Assert.Equal(0.2, state.DesiredAcceleration, 10);
        Assert.Equal(0.02, state.DesiredVelocity, 10);
    }

    [Fact]
    public void Integrate_WhenAccelerationWouldExceedLimit_ClampsAcceleration()
    {
        MotionRequest request = new MotionRequest
        {
            CurrentVelocity = 0.0,
            CurrentAcceleration = 4.9,
            Limits = new MotionLimits
            {
                MaximumSpeed = 10.0,
                MaximumAcceleration = 5.0,
                MaximumJerk = 2.0
            },
            DeltaTime = 0.1
        };

        MotionIntegrator integrator = new MotionIntegrator();

        MotionState state =
            integrator.Integrate(request, desiredJerk: 2.0);

        Assert.Equal(5.0, state.DesiredAcceleration, 10);
        Assert.Equal(0.5, state.DesiredVelocity, 10);
    }
}