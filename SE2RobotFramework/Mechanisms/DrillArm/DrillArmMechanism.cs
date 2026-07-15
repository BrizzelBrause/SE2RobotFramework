using SE2RobotFramework.Controllers;
using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmMechanism
{
    private readonly IReadOnlyList<MotionController> _controllers;

    public DrillArmMechanism(
        DrillArmHardware hardware,
        DrillArmAxes axes,
        IMotionProfileFactory profileFactory,
        MotionRequestFactory requestFactory)
    {
        Hardware = hardware ?? throw new ArgumentNullException(nameof(hardware));
        Axes = axes ?? throw new ArgumentNullException(nameof(axes));
        ArgumentNullException.ThrowIfNull(profileFactory);
        ArgumentNullException.ThrowIfNull(requestFactory);

        _controllers = new[]
        {
            CreateController(Axes.BaseRotation, Hardware.BaseRotation),
            CreateController(Axes.Shoulder, Hardware.Shoulder),
            CreateController(Axes.UpperArmExtension, Hardware.UpperArmExtension),
            CreateController(Axes.Elbow, Hardware.Elbow),
            CreateController(Axes.ForearmExtension, Hardware.ForearmExtension),
            CreateController(Axes.WristRotation, Hardware.WristRotation),
            CreateController(Axes.WristHinge, Hardware.WristHinge),
            CreateController(Axes.ToolExtension, Hardware.ToolExtension)
        };

        MotionController CreateController(Axis axis, IAxisHardware axisHardware)
        {
            return new MotionController(
                axis,
                axisHardware,
                profileFactory,
                requestFactory);
        }
    }

    public DrillArmHardware Hardware { get; }

    public DrillArmAxes Axes { get; }

    public void SetTargets(DrillArmTargets targets)
    {
        Axes.BaseRotation.SetTargetPosition(targets.BaseRotation);
        Axes.Shoulder.SetTargetPosition(targets.Shoulder);
        Axes.UpperArmExtension.SetTargetPosition(targets.UpperArmExtension);
        Axes.Elbow.SetTargetPosition(targets.Elbow);
        Axes.ForearmExtension.SetTargetPosition(targets.ForearmExtension);
        Axes.WristRotation.SetTargetPosition(targets.WristRotation);
        Axes.WristHinge.SetTargetPosition(targets.WristHinge);
        Axes.ToolExtension.SetTargetPosition(targets.ToolExtension);
    }

    public void Update(double deltaTime)
    {
        foreach (MotionController controller in _controllers)
        {
            controller.Update(deltaTime);
        }
    }
}
