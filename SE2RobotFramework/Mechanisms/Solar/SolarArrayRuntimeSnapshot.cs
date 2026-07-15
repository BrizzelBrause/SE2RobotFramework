using SE2RobotFramework.Controllers;

namespace SE2RobotFramework.Mechanisms.Solar;

public readonly record struct SolarArrayRuntimeSnapshot(
    SolarTrackingServiceStatus Status,
    MechanismRuntimeState MechanismState,
    SolarOrientation? LastOrientation);
