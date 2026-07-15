namespace SE2RobotFramework.Mechanisms.DrillArm;

public readonly record struct DrillArmManualInput(
    DrillArmMouseInput Mouse,
    DrillArmKeyboardInput Keyboard,
    bool DrillHeadEnabled = false);
