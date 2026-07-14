using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Controllers;

public class MotionController
{
    private readonly Axis _axis;
    private readonly IAxisHardware _hardware;
    private readonly IMotionProfile _motionProfile;
    private readonly MotionRequestFactory _requestFactory;

    public MotionController(Axis axis, IAxisHardware hardware, IMotionProfile motionProfile, MotionRequestFactory requestFactory)
    {
        _axis = axis;
        _hardware = hardware;
        _motionProfile = motionProfile;
        _requestFactory = requestFactory;
    }

    public void Update(double deltaTime)
    {
        _axis.UpdatePosition(_hardware.GetPosition());

        if (!CanMove())
        {
            _hardware.Stop();
            return;
        }

        double error = _axis.GetError();
        int direction = Math.Sign(error);

        MotionRequest request = _requestFactory.Create(_axis, _hardware, deltaTime);

        MotionState state = _motionProfile.Calculate(request);

        _hardware.SetVelocity(direction * state.DesiredVelocity);
    }

    private bool CanMove()
    {
        if (!_axis.Enabled)
        {
            return false;
        }

        if (!_hardware.CanExecuteCommand())
        {
            return false;
        }

        if (_axis.IsAtTarget())
        {
            return false;
        }

        return true;
    }
}