namespace SE2RobotFramework.Hardware;

public class SeriesAxisHardware : IAxisHardware, IAxisHardwareDiagnostics
{
    private readonly IReadOnlyList<IAxisHardware> _members;

    public SeriesAxisHardware(IEnumerable<IAxisHardware> members)
    {
        ArgumentNullException.ThrowIfNull(members);

        _members = members.ToArray();

        if (_members.Count == 0)
        {
            throw new ArgumentException(
                "At least one hardware member is required.",
                nameof(members));
        }

        if (_members.Any(member => member is null))
        {
            throw new ArgumentException(
                "Hardware members cannot contain null values.",
                nameof(members));
        }
    }

    public double GetPosition()
    {
        return _members.Sum(member => member.GetPosition());
    }

    public double GetVelocity()
    {
        return _members.Sum(member => member.GetVelocity());
    }

    public void SetVelocity(double velocity)
    {
        double memberVelocity = velocity / _members.Count;

        foreach (IAxisHardware member in _members)
        {
            member.SetVelocity(memberVelocity);
        }
    }

    public void Stop()
    {
        foreach (IAxisHardware member in _members)
        {
            member.Stop();
        }
    }

    public bool CanExecuteCommand()
    {
        return GetStatus() == AxisHardwareStatus.Ready;
    }

    public AxisHardwareStatus GetStatus()
    {
        AxisHardwareStatus[] statuses = _members
            .Select(AxisHardwareDiagnostics.GetStatus)
            .ToArray();

        if (statuses.Contains(AxisHardwareStatus.InvalidFeedback))
        {
            return AxisHardwareStatus.InvalidFeedback;
        }

        return statuses.All(status => status == AxisHardwareStatus.Ready)
            ? AxisHardwareStatus.Ready
            : AxisHardwareStatus.Unavailable;
    }
}
