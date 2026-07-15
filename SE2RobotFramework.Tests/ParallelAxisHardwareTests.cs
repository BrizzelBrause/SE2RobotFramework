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

    [Fact]
    public void CanExecuteCommand_WhenMembersExceedPositionDeviation_ReturnsFalse()
    {
        FakeAxisHardware first = new();
        FakeAxisHardware second = new();
        second.SetPosition(5.0);
        ParallelAxisHardware hardware = new(
            new IAxisHardware[] { first, second },
            maximumPositionDeviation: 2.0);

        hardware.SetVelocity(3.0);

        Assert.False(hardware.IsSynchronized);
        Assert.False(hardware.CanExecuteCommand());
        Assert.Equal(0.0, first.CommandedVelocity);
        Assert.Equal(0.0, second.CommandedVelocity);
    }

    [Fact]
    public void Synchronization_WithPeriod_UsesShortestCircularDifference()
    {
        FakeAxisHardware first = new();
        first.SetPosition(359.0);
        FakeAxisHardware second = new();
        second.SetPosition(1.0);
        ParallelAxisHardware hardware = new(
            new IAxisHardware[] { first, second },
            maximumPositionDeviation: 3.0,
            positionPeriod: 360.0);

        Assert.Equal(2.0, hardware.CurrentPositionDeviation);
        Assert.True(hardware.IsSynchronized);
    }
}
