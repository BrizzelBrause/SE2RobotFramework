namespace SE2RobotFramework.Motion;

public class MotionIntegrator
{
    public MotionState Integrate(MotionRequest request, double desiredJerk)
    {
        double limitedJerk = Math.Clamp(desiredJerk, -request.Limits.MaximumJerk, request.Limits.MaximumJerk);

        double accelerationChange = limitedJerk * request.DeltaTime;

        double desiredAcceleration = request.CurrentAcceleration + accelerationChange;

        desiredAcceleration = Math.Clamp(desiredAcceleration, -request.Limits.MaximumAcceleration, request.Limits.MaximumAcceleration);

        double desiredVelocity = request.CurrentVelocity + desiredAcceleration * request.DeltaTime;

        desiredVelocity = Math.Clamp(desiredVelocity, -request.Limits.MaximumSpeed, request.Limits.MaximumSpeed);

        return new MotionState
        {
            DesiredVelocity = desiredVelocity,
            DesiredAcceleration = desiredAcceleration,
            DesiredJerk = limitedJerk,
            RemainingDistance = request.RemainingDistance
        };
    }
}