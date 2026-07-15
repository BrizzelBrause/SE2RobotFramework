namespace SE2RobotFramework.Hardware;

public class FakeSwitchableHardware : ISwitchableHardware
{
    public bool IsEnabled { get; private set; }

    public bool IsAvailable { get; set; } = true;

    public bool CanExecuteCommand()
    {
        return IsAvailable;
    }

    public void SetEnabled(bool enabled)
    {
        IsEnabled = enabled && IsAvailable;
    }

    public void Stop()
    {
        IsEnabled = false;
    }
}
