using SE2RobotFramework.Hardware;

namespace SE2RobotFramework.Controllers;

public class SwitchableController
{
    private readonly ISwitchableHardware _hardware;

    public SwitchableController(ISwitchableHardware hardware)
    {
        _hardware = hardware ?? throw new ArgumentNullException(nameof(hardware));
    }

    public SwitchableControllerStatus Status { get; private set; } =
        SwitchableControllerStatus.Stopped;

    public bool SetEnabled(bool enabled)
    {
        if (!enabled)
        {
            _hardware.Stop();
            Status = SwitchableControllerStatus.Disabled;
            return true;
        }

        if (!_hardware.CanExecuteCommand())
        {
            _hardware.Stop();
            Status = SwitchableControllerStatus.HardwareUnavailable;
            return false;
        }

        _hardware.SetEnabled(true);
        Status = _hardware.IsEnabled
            ? SwitchableControllerStatus.Enabled
            : SwitchableControllerStatus.HardwareUnavailable;
        return Status == SwitchableControllerStatus.Enabled;
    }

    public void Stop()
    {
        _hardware.Stop();
        Status = SwitchableControllerStatus.Stopped;
    }
}
