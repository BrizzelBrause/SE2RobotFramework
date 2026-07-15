using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.DrillArm;
using SE2RobotFramework.Motion;

internal static class DrillArmDemo
{
    private const double DeltaTime = 0.1;

    public static void Run()
    {
        DrillArmAxes axes = CreateAxes();
        DrillArmPositionLimits.Apply(axes);
        (DrillArmHardware hardware, FakeAxisHardware[] physicalAxes) =
            CreateHardware();
        SetInitialPose(axes, hardware);

        DrillArmMechanism mechanism = new(
            hardware,
            axes,
            new MotionProfileFactory(),
            new MotionRequestFactory());
        DrillArmControlService service = new(mechanism);
        DrillArmManualInputController controller = new(
            mechanism,
            service,
            new DrillArmMouseController(
                mechanism,
                service,
                new DrillArmMouseControlConfiguration()),
            new DrillArmKeyboardController(
                mechanism,
                service,
                new DrillArmKeyboardControlConfiguration()));

        for (int step = 0; step < 300; step++)
        {
            bool isInputPhase = step < 10;
            DrillArmMouseInput mouse = step == 0
                ? new DrillArmMouseInput(10.0, 5.0)
                : default;
            DrillArmKeyboardInput keyboard = isInputPhase
                ? new DrillArmKeyboardInput(1.0, 0.5, 0.0, 1.0)
                : default;

            controller.Process(
                new DrillArmManualInput(mouse, keyboard),
                DeltaTime);

            foreach (FakeAxisHardware physicalAxis in physicalAxes)
            {
                physicalAxis.Simulate(DeltaTime);
            }

            if (step % 50 == 0)
            {
                Console.WriteLine(
                    $"Step: {step,3} | Base: {axes.BaseRotation.CurrentPosition,5:0.0} deg | " +
                    $"Shoulder: {axes.Shoulder.CurrentPosition,5:0.0} deg | " +
                    $"Upper arm: {axes.UpperArmExtension.CurrentPosition,4:0.0} m | " +
                    $"Status: {service.Status}");
            }
        }

        if (service.Status == DrillArmControlStatus.AtTarget)
        {
            Console.WriteLine("Drill-arm simulation completed successfully.");
            return;
        }

        Console.WriteLine($"Drill-arm simulation ended with status {service.Status}.");
        if (service.LastRuntimeState is not null)
        {
            foreach (var state in service.LastRuntimeState.Axes
                .Where(state => Math.Abs(state.Error) > 0.1))
            {
                Console.WriteLine(
                    $"  {state.Name}: position {state.CurrentPosition:0.00}, " +
                    $"target {state.TargetPosition:0.00}, error {state.Error:0.00}");
            }
        }
    }

    private static DrillArmAxes CreateAxes()
    {
        return new DrillArmAxes
        {
            BaseRotation = CreateAxis<RotationalAxis>("DrillArm.Base"),
            Shoulder = CreateAxis<RotationalAxis>("DrillArm.Shoulder"),
            UpperArmExtension = CreateAxis<LinearAxis>("DrillArm.UpperArm"),
            Elbow = CreateAxis<RotationalAxis>("DrillArm.Elbow"),
            ForearmExtension = CreateAxis<LinearAxis>("DrillArm.Forearm"),
            ForearmHinge = CreateAxis<RotationalAxis>("DrillArm.ForearmHinge"),
            WristRotation = CreateAxis<RotationalAxis>("DrillArm.WristRotation"),
            WristHinge = CreateAxis<RotationalAxis>("DrillArm.WristHinge"),
            ToolExtension = CreateAxis<LinearAxis>("DrillArm.Tool")
        };
    }

    private static TAxis CreateAxis<TAxis>(string name)
        where TAxis : Axis, new()
    {
        bool isLinear = typeof(TAxis) == typeof(LinearAxis);
        return new TAxis
        {
            Name = name,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimits
            {
                MaximumSpeed = isLinear ? 1.0 : 10.0
            },
            Tolerance = isLinear ? 0.05 : 0.75
        };
    }

    private static (DrillArmHardware Hardware, FakeAxisHardware[] PhysicalAxes)
        CreateHardware()
    {
        FakeAxisHardware[] physicalAxes = Enumerable.Range(0, 9)
            .Select(_ => new FakeAxisHardware { MaximumAcceleration = 20.0 })
            .ToArray();

        return (new DrillArmHardware
        {
            BaseRotation = physicalAxes[0],
            Shoulder = physicalAxes[1],
            UpperArmExtension = physicalAxes[2],
            Elbow = physicalAxes[3],
            ForearmExtension = physicalAxes[4],
            ForearmHinge = physicalAxes[5],
            WristRotation = physicalAxes[6],
            WristHinge = physicalAxes[7],
            ToolExtension = physicalAxes[8]
        }, physicalAxes);
    }

    private static void SetInitialPose(
        DrillArmAxes axes,
        DrillArmHardware hardware)
    {
        SetPosition(axes.Shoulder, hardware.Shoulder, 90.0);
        SetPosition(axes.ForearmHinge, hardware.ForearmHinge, 90.0);
        axes.Shoulder.SetTargetPosition(90.0);
        axes.ForearmHinge.SetTargetPosition(90.0);
    }

    private static void SetPosition(Axis axis, IAxisHardware hardware, double position)
    {
        ((FakeAxisHardware)hardware).SetPosition(position);
        axis.UpdatePosition(position);
    }
}
