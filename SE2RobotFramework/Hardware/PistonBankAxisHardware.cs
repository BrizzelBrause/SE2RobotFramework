namespace SE2RobotFramework.Hardware;

public class PistonBankAxisHardware : IAxisHardware
{
    public const int MaximumPistonsPerRow = 6;

    private readonly ParallelAxisHardware _parallelRows;

    public PistonBankAxisHardware(
        IEnumerable<IEnumerable<IAxisHardware>> rows,
        int feedbackRowIndex = 0,
        double maximumRowPositionDeviation = double.PositiveInfinity)
    {
        ArgumentNullException.ThrowIfNull(rows);

        IAxisHardware[][] rowMembers = rows
            .Select(row => row?.ToArray() ??
                throw new ArgumentException(
                    "Piston rows cannot contain null values.",
                    nameof(rows)))
            .ToArray();

        if (rowMembers.Length == 0)
        {
            throw new ArgumentException(
                "At least one parallel piston row is required.",
                nameof(rows));
        }

        SeriesCount = rowMembers[0].Length;

        if (SeriesCount == 0 || SeriesCount > MaximumPistonsPerRow)
        {
            throw new ArgumentException(
                $"Each piston row must contain between 1 and {MaximumPistonsPerRow} pistons.",
                nameof(rows));
        }

        if (rowMembers.Any(row => row.Length != SeriesCount))
        {
            throw new ArgumentException(
                "All parallel piston rows must contain the same number of pistons.",
                nameof(rows));
        }

        ParallelCount = rowMembers.Length;

        _parallelRows = new ParallelAxisHardware(
            rowMembers.Select(row => new SeriesAxisHardware(row)),
            feedbackRowIndex,
            maximumRowPositionDeviation);
    }

    public int SeriesCount { get; }

    public int ParallelCount { get; }

    public double MaximumRowPositionDeviation =>
        _parallelRows.MaximumPositionDeviation;

    public double CurrentRowPositionDeviation =>
        _parallelRows.CurrentPositionDeviation;

    public bool IsSynchronized => _parallelRows.IsSynchronized;

    public double GetPosition()
    {
        return _parallelRows.GetPosition();
    }

    public double GetVelocity()
    {
        return _parallelRows.GetVelocity();
    }

    public void SetVelocity(double velocity)
    {
        _parallelRows.SetVelocity(velocity);
    }

    public void Stop()
    {
        _parallelRows.Stop();
    }

    public bool CanExecuteCommand()
    {
        return _parallelRows.CanExecuteCommand();
    }
}
