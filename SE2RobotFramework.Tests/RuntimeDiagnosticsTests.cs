using SE2RobotFramework.Controllers;
using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class RuntimeDiagnosticsTests
{
    [Fact]
    public void HardwareDiagnostics_WithInvalidFeedback_ReturnsInvalidFeedback()
    {
        FakeAxisHardware hardware = new();
        hardware.SetPosition(double.NaN);

        AxisHardwareStatus status = AxisHardwareDiagnostics.GetStatus(hardware);

        Assert.Equal(AxisHardwareStatus.InvalidFeedback, status);
        Assert.False(hardware.CanExecuteCommand());
    }

    [Fact]
    public void ParallelDiagnostics_WithPositionMismatch_ReturnsSynchronizationLost()
    {
        FakeAxisHardware first = new();
        FakeAxisHardware second = new();
        second.SetPosition(5.0);
        ParallelAxisHardware hardware = new(
            new IAxisHardware[] { first, second },
            maximumPositionDeviation: 1.0);

        AxisHardwareStatus status = hardware.GetStatus();

        Assert.Equal(AxisHardwareStatus.SynchronizationLost, status);
    }

    [Fact]
    public void Controller_WithUnavailableHardware_ReportsHardwareUnavailable()
    {
        FakeAxisHardware hardware = new() { IsAvailable = false };
        MotionController controller = CreateController(hardware);

        controller.Update(0.1);

        Assert.Equal(
            MotionControllerStatus.HardwareUnavailable,
            controller.Status);
    }

    [Fact]
    public void Controller_WithSynchronizationLoss_ReportsSynchronizationLost()
    {
        FakeAxisHardware first = new();
        FakeAxisHardware second = new();
        second.SetPosition(5.0);
        ParallelAxisHardware hardware = new(
            new IAxisHardware[] { first, second },
            maximumPositionDeviation: 1.0);
        MotionController controller = CreateController(hardware);

        controller.Update(0.1);

        Assert.Equal(
            MotionControllerStatus.SynchronizationLost,
            controller.Status);
    }

    [Fact]
    public void Controller_TransitionsFromMovingToAtTarget()
    {
        FakeAxisHardware hardware = new();
        MotionController controller = CreateController(hardware);

        controller.Update(0.1);
        Assert.Equal(MotionControllerStatus.Moving, controller.Status);

        hardware.SetPosition(10.0);
        controller.Update(0.1);

        Assert.Equal(MotionControllerStatus.AtTarget, controller.Status);
    }

    [Fact]
    public void Controller_WithInvalidDeltaTime_Throws()
    {
        MotionController controller = CreateController(new FakeAxisHardware());

        Assert.Throws<ArgumentOutOfRangeException>(() => controller.Update(0.0));
    }

    private static MotionController CreateController(IAxisHardware hardware)
    {
        LinearAxis axis = new()
        {
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimits { MaximumSpeed = 2.0 },
            Tolerance = 0.1
        };
        axis.SetTargetPosition(10.0);

        return new MotionController(
            axis,
            hardware,
            new MotionProfileFactory(),
            new MotionRequestFactory());
    }
}
