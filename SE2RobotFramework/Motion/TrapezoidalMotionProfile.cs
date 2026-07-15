namespace SE2RobotFramework.Motion;

public class TrapezoidalMotionProfile : IMotionProfile
{
    public MotionState Calculate(MotionRequest request)
    {
        bool isMovingTowardTarget = Math.Sign(request.CurrentVelocity) == request.Direction;

        if (!isMovingTowardTarget && Math.Abs(request.CurrentVelocity) > 0.0001)
        {
            return new MotionState
            {
                DesiredVelocity = 0.0,
                DesiredAcceleration = 0.0,
                DesiredJerk = 0.0,
                RemainingDistance = request.RemainingDistance,
                StoppingDistance = 0.0,
                IsBraking = true,
                IsMovingTowardTarget = false
            };
        }

        if (request.Limits.MaximumAcceleration <= 0.0)
        {
            return new MotionState
            {
                DesiredVelocity = request.Limits.MaximumSpeed,
                RemainingDistance = request.RemainingDistance,
                IsMovingTowardTarget = isMovingTowardTarget
            };
        }

        double brakingLimitedSpeed = Math.Sqrt(2.0 * request.Limits.MaximumAcceleration * request.RemainingDistance);

        double stoppingDistance = request.Limits.MaximumSpeed * request.Limits.MaximumSpeed / (2.0 * request.Limits.MaximumAcceleration);

        return new MotionState
        {
            DesiredVelocity = Math.Min(request.Limits.MaximumSpeed, brakingLimitedSpeed),

            DesiredAcceleration = 0.0,
            DesiredJerk = 0.0,
            RemainingDistance = request.RemainingDistance,
            StoppingDistance = stoppingDistance,
            IsBraking = request.RemainingDistance < stoppingDistance,
            IsMovingTowardTarget = isMovingTowardTarget
        };
    }
}