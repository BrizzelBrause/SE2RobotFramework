using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;

namespace SE2RobotFramework.Motion;


public class MotionRequestFactory
{
    private readonly MotionStateEstimator _stateEstimator = new MotionStateEstimator();

    public MotionRequest Create(Axis axis, IAxisHardware hardware, double deltaTime)
    {

        double currentAcceleration = _stateEstimator.EstimateAcceleration(hardware.GetVelocity(), deltaTime);

        return new MotionRequest
        {
            RemainingDistance = Math.Abs(axis.GetError()),
            CurrentVelocity = hardware.GetVelocity(),
            CurrentAcceleration = currentAcceleration,
            MaximumSpeed = axis.MovementSpeed,
            MaximumAcceleration = axis.MaximumAcceleration,
            Direction = axis.GetDirection(),
            MaximumJerk = axis.MaximumJerk,
            DeltaTime = deltaTime
        };
    }
}