using SE2RobotFramework.Hardware;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Controllers;

public readonly record struct AxisRuntimeState(
    string Name,
    double CurrentPosition,
    double TargetPosition,
    double Error,
    int Direction,
    bool Enabled,
    MotionProfileType MotionProfileType,
    MotionControllerStatus ControllerStatus,
    AxisHardwareStatus HardwareStatus);
