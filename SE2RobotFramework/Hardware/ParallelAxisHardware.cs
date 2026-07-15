namespace SE2RobotFramework.Hardware;

public class ParallelAxisHardware : IAxisHardware
{
    private readonly IReadOnlyList<IAxisHardware> _members;
    private readonly IAxisHardware _feedbackSource;

    public ParallelAxisHardware(
        IEnumerable<IAxisHardware> members,
        int feedbackSourceIndex = 0,
        double maximumPositionDeviation = double.PositiveInfinity,
        double? positionPeriod = null)
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

        if (double.IsNaN(maximumPositionDeviation) || maximumPositionDeviation < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumPositionDeviation));
        }

        if (positionPeriod.HasValue &&
            (!double.IsFinite(positionPeriod.Value) || positionPeriod.Value <= 0.0))
        {
            throw new ArgumentOutOfRangeException(nameof(positionPeriod));
        }

        _feedbackSource = _members[feedbackSourceIndex];
        MaximumPositionDeviation = maximumPositionDeviation;
        PositionPeriod = positionPeriod;
    }

    public double MaximumPositionDeviation { get; }

    public double? PositionPeriod { get; }

    public double CurrentPositionDeviation
    {
        get
        {
            double referencePosition = _feedbackSource.GetPosition();

            return _members.Max(member => CalculatePositionDifference(
                member.GetPosition(),
                referencePosition));
        }
    }

    public bool IsSynchronized =>
        CurrentPositionDeviation <= MaximumPositionDeviation;

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
        if (!CanExecuteCommand())
        {
            Stop();
            return;
        }

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
        return
            _members.All(member => member.CanExecuteCommand()) &&
            IsSynchronized;
    }

    private double CalculatePositionDifference(double first, double second)
    {
        if (!double.IsFinite(first) || !double.IsFinite(second))
        {
            return double.PositiveInfinity;
        }

        double difference = Math.Abs(first - second);

        if (!PositionPeriod.HasValue)
        {
            return difference;
        }

        difference %= PositionPeriod.Value;

        return Math.Min(difference, PositionPeriod.Value - difference);
    }
}
