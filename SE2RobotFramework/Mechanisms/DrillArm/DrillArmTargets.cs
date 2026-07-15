namespace SE2RobotFramework.Mechanisms.DrillArm;

public readonly record struct DrillArmTargets(
    double BaseRotation,
    double Shoulder,
    double UpperArmExtension,
    double Elbow,
    double ForearmExtension,
    double WristRotation,
    double WristHinge,
    double ToolExtension);
