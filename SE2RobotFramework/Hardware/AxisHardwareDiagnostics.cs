namespace SE2RobotFramework.Hardware;

public static class AxisHardwareDiagnostics
{
    public static AxisHardwareStatus GetStatus(IAxisHardware hardware)
    {
        ArgumentNullException.ThrowIfNull(hardware);

        if (hardware is IAxisHardwareDiagnostics diagnostics)
        {
            return diagnostics.GetStatus();
        }

        if (!double.IsFinite(hardware.GetPosition()) ||
            !double.IsFinite(hardware.GetVelocity()))
        {
            return AxisHardwareStatus.InvalidFeedback;
        }

        return hardware.CanExecuteCommand()
            ? AxisHardwareStatus.Ready
            : AxisHardwareStatus.Unavailable;
    }
}
