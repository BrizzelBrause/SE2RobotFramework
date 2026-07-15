using SE2RobotFramework.Hardware;

namespace SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmHardware
{
    public required IAxisHardware BaseRotation { get; init; }

    public required IAxisHardware Shoulder { get; init; }

    public required IAxisHardware UpperArmExtension { get; init; }

    public required IAxisHardware Elbow { get; init; }

    public required IAxisHardware ForearmExtension { get; init; }

    public required IAxisHardware WristRotation { get; init; }

    public required IAxisHardware WristHinge { get; init; }

    public required IAxisHardware ToolExtension { get; init; }
}
