using SE2RobotFramework.Hardware;

namespace SE2RobotFramework.Configuration;

public class PistonBankHardwareFactory
{
    public PistonBankAxisHardware Create(
        PistonBankConfiguration configuration,
        IEnumerable<IEnumerable<IAxisHardware>> pistonRows)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(pistonRows);
        configuration.Validate();

        PistonBankAxisHardware hardware = new(pistonRows);

        if (hardware.SeriesCount != configuration.SeriesCount ||
            hardware.ParallelCount != configuration.ParallelCount)
        {
            throw new ArgumentException(
                "The physical piston layout does not match its configuration.",
                nameof(pistonRows));
        }

        return hardware;
    }
}
