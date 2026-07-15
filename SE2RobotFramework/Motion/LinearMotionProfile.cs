namespace SE2RobotFramework.Motion;

public class LinearMotionProfile : IMotionProfile
{
    public MotionState Calculate(MotionRequest request)
    {
        if (request.Limits.MaximumSpeed < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Limits.MaximumSpeed));
        }

        bool isMovingTowardTarget =
            Math.Sign(request.CurrentVelocity) == request.Direction;

        return new MotionState
        {
            DesiredVelocity = request.Direction == 0
                ? 0.0
                : request.Limits.MaximumSpeed,
            DesiredAcceleration = 0.0,
            DesiredJerk = 0.0,
            RemainingDistance = request.RemainingDistance,
            StoppingDistance = 0.0,
            IsBraking = request.Direction == 0,
            IsMovingTowardTarget = isMovingTowardTarget
        };
    }
}
