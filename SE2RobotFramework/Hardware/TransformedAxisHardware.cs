namespace SE2RobotFramework.Hardware;

public class TransformedAxisHardware : IAxisHardware, IAxisHardwareDiagnostics
{
    private readonly IAxisHardware _hardware;

    public TransformedAxisHardware(
        IAxisHardware hardware,
        double scale = 1.0,
        double positionOffset = 0.0)
    {
        if (scale == 0.0 || !double.IsFinite(scale))
        {
            throw new ArgumentOutOfRangeException(nameof(scale));
        }

        if (!double.IsFinite(positionOffset))
        {
            throw new ArgumentOutOfRangeException(nameof(positionOffset));
        }

        _hardware = hardware ?? throw new ArgumentNullException(nameof(hardware));
        Scale = scale;
        PositionOffset = positionOffset;
    }

    public double Scale { get; }

    public double PositionOffset { get; }

    public double GetPosition()
    {
        return (_hardware.GetPosition() - PositionOffset) / Scale;
    }

    public double GetVelocity()
    {
        return _hardware.GetVelocity() / Scale;
    }

    public void SetVelocity(double velocity)
    {
        _hardware.SetVelocity(velocity * Scale);
    }

    public void Stop()
    {
        _hardware.Stop();
    }

    public bool CanExecuteCommand()
    {
        return GetStatus() == AxisHardwareStatus.Ready;
    }

    public AxisHardwareStatus GetStatus()
    {
        AxisHardwareStatus status = AxisHardwareDiagnostics.GetStatus(_hardware);

        if (status != AxisHardwareStatus.Ready)
        {
            return status;
        }

        return double.IsFinite(GetPosition()) && double.IsFinite(GetVelocity())
            ? AxisHardwareStatus.Ready
            : AxisHardwareStatus.InvalidFeedback;
    }
}
