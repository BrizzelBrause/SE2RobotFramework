using SE2RobotFramework.Core;

namespace SE2RobotFramework.Tests;

public class RotationalAxisTests
{
    [Fact]
    public void GetError_From350To10_ReturnsPositive20()
    {
        RotationalAxis axis = new RotationalAxis
        {
            WrapAround = true
        };

        axis.UpdatePosition(350.0);
        axis.SetTargetPosition(10.0);

        double error = axis.GetError();

        Assert.Equal(20.0, error);
    }

    [Fact]
    public void GetError_From10To350_ReturnsNegative20()
    {
        RotationalAxis axis = new RotationalAxis
        {
            WrapAround = true
        };

        axis.UpdatePosition(10.0);
        axis.SetTargetPosition(350.0);

        double error = axis.GetError();

        Assert.Equal(-20.0, error);
    }

    [Fact]
    public void UpdatePosition_With370_NormalizesTo10()
    {
        RotationalAxis axis = new RotationalAxis
        {
            WrapAround = true
        };

        axis.UpdatePosition(370.0);

        Assert.Equal(10.0, axis.CurrentPosition);
    }

    [Fact]
    public void UpdatePosition_WithNegative10_NormalizesTo350()
    {
        RotationalAxis axis = new RotationalAxis
        {
            WrapAround = true
        };

        axis.UpdatePosition(-10.0);

        Assert.Equal(350.0, axis.CurrentPosition);
    }
}