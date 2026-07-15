using SE2RobotFramework.Hardware;

namespace SE2RobotFramework.Mechanisms.Solar;

public class SolarArrayHardware
{
    public SolarArrayHardware(
        SolarArrayType type,
        IAxisHardware azimuth,
        IAxisHardware elevation)
    {
        Type = type;
        Azimuth = azimuth ?? throw new ArgumentNullException(nameof(azimuth));
        Elevation = elevation ?? throw new ArgumentNullException(nameof(elevation));
    }

    public SolarArrayType Type { get; }

    public IAxisHardware Azimuth { get; }

    public IAxisHardware Elevation { get; }
}
