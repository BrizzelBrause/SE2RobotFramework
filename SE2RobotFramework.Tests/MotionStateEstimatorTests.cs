using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class MotionStateEstimatorTests
{
    [Fact]
    public void EstimateAcceleration_WithVelocityIncrease_ReturnsPositiveAcceleration()
    {
        MotionStateEstimator estimator = new MotionStateEstimator();

        estimator.EstimateAcceleration(0.0, 0.1);

        double acceleration = estimator.EstimateAcceleration(1.0, 0.1);

        Assert.Equal(10.0, acceleration);
    }
}