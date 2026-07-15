using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Controllers;

public class MotionController
{
    private readonly Axis _axis;
    private readonly IAxisHardware _hardware;
    private readonly IMotionProfileFactory _profileFactory;
    private readonly MotionRequestFactory _requestFactory;
    private IMotionProfile? _motionProfile;
    private MotionProfileType? _currentProfileType;

    public MotionControllerStatus Status { get; private set; } =
        MotionControllerStatus.AtTarget;

    public MotionController(Axis axis, IAxisHardware hardware, IMotionProfileFactory profileFactory, MotionRequestFactory requestFactory)
    {
        _axis = axis;
        _hardware = hardware;
        _profileFactory = profileFactory;
        _requestFactory = requestFactory;
    }

    public void Update(double deltaTime)
    {
        if (!double.IsFinite(deltaTime) || deltaTime <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaTime));
        }

        if (!_axis.Enabled)
        {
            _hardware.Stop();
            Status = MotionControllerStatus.Disabled;
            return;
        }

        AxisHardwareStatus hardwareStatus =
            AxisHardwareDiagnostics.GetStatus(_hardware);

        if (hardwareStatus != AxisHardwareStatus.Ready)
        {
            _hardware.Stop();
            Status = MapStatus(hardwareStatus);
            return;
        }

        _axis.UpdatePosition(_hardware.GetPosition());

        if (_axis.IsAtTarget())
        {
            _hardware.Stop();
            Status = MotionControllerStatus.AtTarget;
            return;
        }

        double error = _axis.GetError();
        int direction = Math.Sign(error);

        MotionRequest request = _requestFactory.Create(_axis, _hardware, deltaTime);

        if (_motionProfile is null || _currentProfileType != _axis.MotionProfileType)
        {
            _motionProfile = _profileFactory.Create(_axis.MotionProfileType);
            _currentProfileType = _axis.MotionProfileType;
        }

        MotionState state = _motionProfile.Calculate(request);

        _hardware.SetVelocity(direction * state.DesiredVelocity);
        Status = MotionControllerStatus.Moving;
    }

    public AxisRuntimeState GetRuntimeState()
    {
        return new AxisRuntimeState(
            _axis.Name,
            _axis.CurrentPosition,
            _axis.TargetPosition,
            _axis.GetError(),
            _axis.GetDirection(),
            _axis.Enabled,
            _axis.MotionProfileType,
            Status,
            AxisHardwareDiagnostics.GetStatus(_hardware));
    }

    private static MotionControllerStatus MapStatus(
        AxisHardwareStatus hardwareStatus)
    {
        return hardwareStatus switch
        {
            AxisHardwareStatus.Unavailable =>
                MotionControllerStatus.HardwareUnavailable,
            AxisHardwareStatus.InvalidFeedback =>
                MotionControllerStatus.InvalidFeedback,
            AxisHardwareStatus.SynchronizationLost =>
                MotionControllerStatus.SynchronizationLost,
            _ => throw new ArgumentOutOfRangeException(nameof(hardwareStatus))
        };
    }
}
