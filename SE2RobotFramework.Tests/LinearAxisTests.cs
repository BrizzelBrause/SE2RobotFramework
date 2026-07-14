using SE2RobotFramework.Core;

namespace SE2RobotFramework.Tests;

public class LinearAxisTests
{
    [Fact]
    public void GetError_TargetAboveCurrent_ReturnsPositiveDifference()
    {
        LinearAxis axis = new LinearAxis();

        axis.UpdatePosition(2.0);
        axis.SetTargetPosition(5.0);

        Assert.Equal(3.0, axis.GetError());
    }

    [Fact]
    public void GetError_TargetBelowCurrent_ReturnsNegativeDifference()
    {
        LinearAxis axis = new LinearAxis();

        axis.UpdatePosition(5.0);
        axis.SetTargetPosition(2.0);

        Assert.Equal(-3.0, axis.GetError());
    }

    [Fact]
    public void SetTargetPosition_AboveMaximum_ClampsToMaximum()
    {
        LinearAxis axis = new LinearAxis
        {
            MinimumPosition = 0.0,
            MaximumPosition = 10.0
        };

        axis.SetTargetPosition(15.0);

        Assert.Equal(10.0, axis.TargetPosition);
    }

    [Fact]
    public void SetTargetPosition_BelowMinimum_ClampsToMinimum()
    {
        LinearAxis axis = new LinearAxis
        {
            MinimumPosition = 0.0,
            MaximumPosition = 10.0
        };

        axis.SetTargetPosition(-3.0);

        Assert.Equal(0.0, axis.TargetPosition);
    }
}