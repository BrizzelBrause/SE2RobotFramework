using SE2RobotFramework.Controllers;
using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class MotionControllerTests
{
    [Fact]
    public void Controller_MovesAxisToTargetWithinTolerance()
    {
        RotationalAxis axis = new RotationalAxis
        {
            MovementSpeed = 5.0,
            MaximumAcceleration = 1.0,
            MotionProfileType = MotionProfileType.Trapezoidal,
            Tolerance = 0.5,
            WrapAround = true
        };

        FakeAxisHardware hardware = new FakeAxisHardware
        {
            MaximumAcceleration = 1.0
        };

        hardware.SetPosition(0.0);
        axis.SetTargetPosition(90.0);

        IMotionProfileFactory profileFactory = new MotionProfileFactory();

        MotionRequestFactory requestFactory = new MotionRequestFactory();

        MotionController controller = new MotionController(axis, hardware, profileFactory, requestFactory);

        double deltaTime = 0.1;
        double simulationTime = 30.0;

        int steps = (int)(simulationTime / deltaTime);

        for (int step = 0; step < steps; step++)
        {
            controller.Update(0.1);
            hardware.Simulate(deltaTime);
        }

        controller.Update(0.1);

        Assert.True(
            axis.IsAtTarget(),
            $"Restfehler war {axis.GetError():0.000}°");
    }

    [Fact]
    public void Controller_UsesShortestPathAcrossZeroDegrees()
    {
        RotationalAxis axis = new RotationalAxis
        {
            MovementSpeed = 5.0,
            MaximumAcceleration = 1.0,
            MotionProfileType = MotionProfileType.Trapezoidal,
            Tolerance = 0.5,
            WrapAround = true
        };

        FakeAxisHardware hardware = new FakeAxisHardware
        {
            MaximumAcceleration = 1.0
        };

        hardware.SetPosition(350.0);
        axis.SetTargetPosition(10.0);

        IMotionProfileFactory profileFactory = new MotionProfileFactory();

        MotionRequestFactory requestFactory = new MotionRequestFactory();

        MotionController controller = new MotionController(axis, hardware, profileFactory, requestFactory);

        double deltaTime = 0.1;
        int steps = 100;

        for (int step = 0; step < steps; step++)
        {
            controller.Update(0.1);
            hardware.Simulate(deltaTime);
        }

        controller.Update(0.1);

        Assert.True(
            axis.IsAtTarget(),
            $"Restfehler war {axis.GetError():0.000}°");
    }

    [Fact]
    public void Controller_BrakesToStopBeforeChangingDirection()
    {
        RotationalAxis axis = new RotationalAxis
        {
            MovementSpeed = 5.0,
            MaximumAcceleration = 1.0,
            MotionProfileType = MotionProfileType.Trapezoidal,
            Tolerance = 0.5,
            WrapAround = true
        };

        FakeAxisHardware hardware = new FakeAxisHardware
        {
            MaximumAcceleration = 1.0
        };

        hardware.SetPosition(0.0);
        hardware.SetVelocity(5.0);

        for (int step = 0; step < 50; step++)
        {
            hardware.Simulate(0.1);
        }

        axis.SetTargetPosition(350.0);

        IMotionProfileFactory profileFactory = new MotionProfileFactory();

        MotionRequestFactory requestFactory = new MotionRequestFactory();

        MotionController controller = new MotionController(axis, hardware, profileFactory, requestFactory);

        controller.Update(0.1);

        Assert.Equal(0.0, hardware.CommandedVelocity);
    }
}