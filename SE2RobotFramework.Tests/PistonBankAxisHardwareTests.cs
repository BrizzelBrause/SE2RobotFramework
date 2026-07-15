using SE2RobotFramework.Hardware;

namespace SE2RobotFramework.Tests;

public class PistonBankAxisHardwareTests
{
    [Theory]
    [InlineData(5, 4)]
    [InlineData(5, 3)]
    [InlineData(8, 1)]
    [InlineData(2, 6)]
    public void Constructor_WithValidLayout_ExposesConfiguredDimensions(
        int parallelCount,
        int seriesCount)
    {
        FakeAxisHardware[][] pistons = CreatePistons(parallelCount, seriesCount);

        PistonBankAxisHardware hardware = new(pistons);

        Assert.Equal(parallelCount, hardware.ParallelCount);
        Assert.Equal(seriesCount, hardware.SeriesCount);
    }

    [Fact]
    public void SetVelocity_ForFiveRowsOfFour_DistributesVelocityAcrossEachRow()
    {
        FakeAxisHardware[][] pistons = CreatePistons(5, 4);
        PistonBankAxisHardware hardware = new(pistons);

        hardware.SetVelocity(8.0);

        Assert.All(
            pistons.SelectMany(row => row),
            piston => Assert.Equal(2.0, piston.CommandedVelocity));
    }

    [Fact]
    public void GetPosition_UsesTotalExtensionOfFeedbackRow()
    {
        FakeAxisHardware[][] pistons = CreatePistons(5, 4);
        pistons[0][0].SetPosition(1.0);
        pistons[0][1].SetPosition(2.0);
        pistons[0][2].SetPosition(3.0);
        pistons[0][3].SetPosition(4.0);
        PistonBankAxisHardware hardware = new(pistons);

        Assert.Equal(10.0, hardware.GetPosition());
    }

    [Fact]
    public void Constructor_WithMoreThanSixPistonsPerRow_Throws()
    {
        FakeAxisHardware[][] pistons = CreatePistons(1, 7);

        Assert.Throws<ArgumentException>(() => new PistonBankAxisHardware(pistons));
    }

    [Fact]
    public void Constructor_WithDifferentRowLengths_Throws()
    {
        FakeAxisHardware[][] pistons =
        {
            new FakeAxisHardware[3],
            new FakeAxisHardware[4]
        };

        for (int index = 0; index < pistons.Length; index++)
        {
            for (int memberIndex = 0; memberIndex < pistons[index].Length; memberIndex++)
            {
                pistons[index][memberIndex] = new FakeAxisHardware();
            }
        }

        Assert.Throws<ArgumentException>(() => new PistonBankAxisHardware(pistons));
    }

    [Fact]
    public void CanExecuteCommand_WhenParallelRowsAreOutOfSync_ReturnsFalse()
    {
        FakeAxisHardware[][] pistons = CreatePistons(2, 3);
        pistons[1][0].SetPosition(2.0);
        PistonBankAxisHardware hardware = new(
            pistons,
            maximumRowPositionDeviation: 1.0);

        Assert.Equal(2.0, hardware.CurrentRowPositionDeviation);
        Assert.False(hardware.IsSynchronized);
        Assert.False(hardware.CanExecuteCommand());
    }

    private static FakeAxisHardware[][] CreatePistons(
        int parallelCount,
        int seriesCount)
    {
        return Enumerable.Range(0, parallelCount)
            .Select(_ => Enumerable.Range(0, seriesCount)
                .Select(_ => new FakeAxisHardware())
                .ToArray())
            .ToArray();
    }
}
