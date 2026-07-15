using SE2RobotFramework.Hardware;

namespace SE2RobotFramework.Tests;

public class TransformedAxisHardwareTests
{
    [Fact]
    public void MirroredHardware_TransformsPositionAndVelocityInBothDirections()
    {
        FakeAxisHardware physicalHardware = new();
        physicalHardware.SetPosition(-30.0);
        TransformedAxisHardware hardware = new(physicalHardware, scale: -1.0);

        hardware.SetVelocity(5.0);

        Assert.Equal(30.0, hardware.GetPosition());
        Assert.Equal(-5.0, physicalHardware.CommandedVelocity);
    }

    [Fact]
    public void PositionOffset_MapsPhysicalZeroToLogicalZero()
    {
        FakeAxisHardware physicalHardware = new();
        physicalHardware.SetPosition(90.0);
        TransformedAxisHardware hardware = new(
            physicalHardware,
            positionOffset: 90.0);

        Assert.Equal(0.0, hardware.GetPosition());
    }
}
