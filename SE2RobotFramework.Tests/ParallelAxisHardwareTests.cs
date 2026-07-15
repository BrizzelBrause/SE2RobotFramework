using SE2RobotFramework.Hardware;

namespace SE2RobotFramework.Tests;

public class ParallelAxisHardwareTests
{
    [Fact]
    public void SetVelocity_WithMirroredMember_CommandsOppositePhysicalDirections()
    {
        FakeAxisHardware first = new();
        FakeAxisHardware second = new();
        ParallelAxisHardware hardware = new(new IAxisHardware[]
        {
            new TransformedAxisHardware(first),
            new TransformedAxisHardware(second, scale: -1.0)
        });

        hardware.SetVelocity(3.0);

        Assert.Equal(3.0, first.CommandedVelocity);
        Assert.Equal(-3.0, second.CommandedVelocity);
    }

    [Fact]
    public void CanExecuteCommand_WhenOneMemberUnavailable_ReturnsFalse()
    {
        FakeAxisHardware first = new();
        FakeAxisHardware second = new() { IsAvailable = false };
        ParallelAxisHardware hardware = new(new IAxisHardware[] { first, second });

        Assert.False(hardware.CanExecuteCommand());
    }
}
