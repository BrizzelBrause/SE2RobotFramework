namespace SE2RobotFramework.Motion;

public class MotionIntegrator
{
    public MotionState Integrate(MotionRequest request, double desiredJerk)
    {
        double limitedJerk = Math.Clamp(desiredJerk, -request.MaximumJerk, request.MaximumJerk);

        double accelerationChange = limitedJerk * request.DeltaTime;

        double desiredAcceleration = request.CurrentAcceleration + accelerationChange;

        desiredAcceleration = Math.Clamp(desiredAcceleration, -request.MaximumAcceleration, request.MaximumAcceleration);

        double desiredVelocity = request.CurrentVelocity + desiredAcceleration * request.DeltaTime;

        desiredVelocity = Math.Clamp(desiredVelocity, -request.MaximumSpeed, request.MaximumSpeed);

        return new MotionState
        {
            DesiredVelocity = desiredVelocity,
            DesiredAcceleration = desiredAcceleration,
            DesiredJerk = limitedJerk,
            RemainingDistance = request.RemainingDistance
        };
    }
}