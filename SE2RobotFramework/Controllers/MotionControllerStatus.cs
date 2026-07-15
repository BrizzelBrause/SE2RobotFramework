namespace SE2RobotFramework.Controllers;

public enum MotionControllerStatus
{
    AtTarget,
    Moving,
    Disabled,
    HardwareUnavailable,
    InvalidFeedback,
    SynchronizationLost
}
