using SE2RobotFramework.Hardware;

namespace SE2RobotFramework.Tests;

public class SeriesAxisHardwareTests
{
    [Fact]
    public void PositionAndVelocity_AreCombinedAcrossPistonChain()
    {
        FakeAxisHardware first = new();
        first.SetPosition(2.0);
        FakeAxisHardware second = new();
        second.SetPosition(3.0);
        SeriesAxisHardware hardware = new(new IAxisHardware[] { first, second });

        hardware.SetVelocity(4.0);

        Assert.Equal(5.0, hardware.GetPosition());
        Assert.Equal(2.0, first.CommandedVelocity);
        Assert.Equal(2.0, second.CommandedVelocity);
    }

    [Fact]
    public void Stop_StopsEveryPiston()
    {
        FakeAxisHardware first = new();
        FakeAxisHardware second = new();
        SeriesAxisHardware hardware = new(new IAxisHardware[] { first, second });
        hardware.SetVelocity(4.0);

        hardware.Stop();

        Assert.Equal(0.0, first.CommandedVelocity);
        Assert.Equal(0.0, second.CommandedVelocity);
    }
}
