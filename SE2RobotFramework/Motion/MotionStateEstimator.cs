namespace SE2RobotFramework.Motion;

public class MotionStateEstimator
{
    private double _previousVelocity;
    private bool _hasPreviousVelocity;

    public double EstimateAcceleration(double currentVelocity, double deltaTime)
    {
        if (deltaTime <= 0.0)
        {
            return 0.0;
        }

        if (!_hasPreviousVelocity)
        {
            _previousVelocity = currentVelocity;
            _hasPreviousVelocity = true;

            return 0.0;
        }

        double acceleration = (currentVelocity - _previousVelocity) / deltaTime;

        _previousVelocity = currentVelocity;

        return acceleration;
    }
}