using SE2RobotFramework.Controllers;

namespace SE2RobotFramework.Mechanisms.DrillArm;

public readonly record struct DrillArmRuntimeSnapshot(
    DrillArmControlStatus Status,
    MechanismRuntimeState MechanismState,
    DrillArmTargets? ActiveTargets,
    bool IsForearmOrientationHoldEnabled,
    bool IsForearmCompensationLimitLatched,
    double ForearmOrientationErrorDegrees);
