namespace SE2RobotFramework.Hardware;

public class ParallelAxisHardware : IAxisHardware
{
    private readonly IReadOnlyList<IAxisHardware> _members;
    private readonly IAxisHardware _feedbackSource;

    public ParallelAxisHardware(
        IEnumerable<IAxisHardware> members,
        int feedbackSourceIndex = 0)
    {
        ArgumentNullException.ThrowIfNull(members);

        _members = members.ToArray();

        if (_members.Count == 0)
        {
            throw new ArgumentException(
                "At least one hardware member is required.",
                nameof(members));
        }

        if (feedbackSourceIndex < 0 || feedbackSourceIndex >= _members.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(feedbackSourceIndex));
        }

        if (_members.Any(member => member is null))
        {
            throw new ArgumentException(
                "Hardware members cannot contain null values.",
                nameof(members));
        }

        _feedbackSource = _members[feedbackSourceIndex];
    }

    public double GetPosition()
    {
        return _feedbackSource.GetPosition();
    }

    public double GetVelocity()
    {
        return _feedbackSource.GetVelocity();
    }

    public void SetVelocity(double velocity)
    {
        foreach (IAxisHardware member in _members)
        {
            member.SetVelocity(velocity);
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
        return _members.All(member => member.CanExecuteCommand());
    }
}
