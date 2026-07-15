using SE2RobotFramework.Core;

namespace SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmAxes
{
    public required RotationalAxis BaseRotation { get; init; }

    public required RotationalAxis Shoulder { get; init; }

    public required LinearAxis UpperArmExtension { get; init; }

    public required RotationalAxis Elbow { get; init; }

    public required LinearAxis ForearmExtension { get; init; }

    public required RotationalAxis WristRotation { get; init; }

    public required RotationalAxis WristHinge { get; init; }

    public required LinearAxis ToolExtension { get; init; }
}
