using SE2RobotFramework.Hardware;

namespace SE2RobotFramework.Configuration;

public class PistonBankConfiguration
{
    public int SeriesCount { get; init; }

    public int ParallelCount { get; init; }

    public void Validate()
    {
        if (SeriesCount < 1 ||
            SeriesCount > PistonBankAxisHardware.MaximumPistonsPerRow)
        {
            throw new ArgumentOutOfRangeException(nameof(SeriesCount));
        }

        if (ParallelCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(ParallelCount));
        }
    }
}
