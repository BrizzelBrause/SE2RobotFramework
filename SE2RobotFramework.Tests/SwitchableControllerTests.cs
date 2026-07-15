using SE2RobotFramework.Controllers;
using SE2RobotFramework.Hardware;

namespace SE2RobotFramework.Tests;

public class SwitchableControllerTests
{
    [Fact]
    public void SetEnabled_WithAvailableHardware_ControlsActuator()
    {
        FakeSwitchableHardware hardware = new();
        SwitchableController controller = new(hardware);

        bool enabled = controller.SetEnabled(true);

        Assert.True(enabled);
        Assert.True(hardware.IsEnabled);
        Assert.Equal(SwitchableControllerStatus.Enabled, controller.Status);

        controller.Stop();

        Assert.False(hardware.IsEnabled);
        Assert.Equal(SwitchableControllerStatus.Stopped, controller.Status);
    }

    [Fact]
    public void SetEnabled_WithUnavailableHardware_FailsSafe()
    {
        FakeSwitchableHardware hardware = new() { IsAvailable = false };
        SwitchableController controller = new(hardware);

        bool enabled = controller.SetEnabled(true);

        Assert.False(enabled);
        Assert.False(hardware.IsEnabled);
        Assert.Equal(
            SwitchableControllerStatus.HardwareUnavailable,
            controller.Status);
    }
}
