using SE2RobotFramework.Controllers;

namespace SE2RobotFramework.Mechanisms.DrillArm;

public readonly record struct DrillArmManualInputResult(
    DrillArmTargets Targets,
    DrillArmMouseControlResult? MouseResult,
    DrillArmControlStatus Status,
    bool? IsDrillHeadCommandAccepted,
    SwitchableControllerStatus? DrillHeadStatus);
