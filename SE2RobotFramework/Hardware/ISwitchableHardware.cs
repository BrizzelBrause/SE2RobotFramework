namespace SE2RobotFramework.Hardware;

public interface ISwitchableHardware
{
    bool IsEnabled { get; }

    bool CanExecuteCommand();

    void SetEnabled(bool enabled);

    void Stop();
}
