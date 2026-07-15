namespace SE2RobotFramework.Controllers;

public enum MotionControllerStatus
{
    AtTarget,
    Moving,
    Stopped,
    Disabled,
    HardwareUnavailable,
    InvalidFeedback,
    SynchronizationLost
}
