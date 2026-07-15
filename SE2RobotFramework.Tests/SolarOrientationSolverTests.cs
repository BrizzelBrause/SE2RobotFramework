using System.Numerics;
using System.Text.Json;
using SE2RobotFramework.Configuration;
using SE2RobotFramework.Mechanisms.Solar;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class SolarOrientationSolverTests
{
    [Theory]
    [InlineData(1.0f, 0.0f, 0.0f, 0.0, 0.0)]
    [InlineData(0.0f, 1.0f, 0.0f, 90.0, 0.0)]
    [InlineData(-1.0f, 0.0f, 0.0f, 180.0, 0.0)]
    [InlineData(0.0f, -1.0f, 0.0f, 270.0, 0.0)]
    [InlineData(0.0f, 0.0f, 1.0f, 0.0, 90.0)]
    public void Calculate_ForCardinalDirections_ReturnsExpectedAngles(
        float x,
        float y,
        float z,
        double expectedAzimuth,
        double expectedElevation)
    {
        SolarOrientation orientation = new SolarOrientationSolver().Calculate(
            new Vector3(x, y, z),
            new SolarTrackingFrame());

        Assert.Equal(expectedAzimuth, orientation.AzimuthDegrees, 6);
        Assert.Equal(expectedElevation, orientation.ElevationDegrees, 6);
    }

    [Fact]
    public void Calculate_WithCalibrationOffsets_AppliesOffsets()
    {
        SolarTrackingFrame frame = new()
        {
            AzimuthOffsetDegrees = 15.0,
            ElevationOffsetDegrees = -5.0
        };

        SolarOrientation orientation = new SolarOrientationSolver().Calculate(
            Vector3.UnitX,
            frame);

        Assert.Equal(15.0, orientation.AzimuthDegrees, 6);
        Assert.Equal(-5.0, orientation.ElevationDegrees, 6);
    }

    [Fact]
    public void Calculate_WithRotatedFrame_UsesConfiguredZeroDirection()
    {
        SolarTrackingFrame frame = new()
        {
            ForwardAtZero = Vector3.UnitY,
            Up = Vector3.UnitZ
        };

        SolarOrientation orientation = new SolarOrientationSolver().Calculate(
            Vector3.UnitY,
            frame);

        Assert.Equal(0.0, orientation.AzimuthDegrees, 6);
        Assert.Equal(0.0, orientation.ElevationDegrees, 6);
    }

    [Fact]
    public void Calculate_WithZeroSunDirection_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new SolarOrientationSolver().Calculate(
                Vector3.Zero,
                new SolarTrackingFrame()));
    }

    [Fact]
    public void TrackingFrame_CanRoundTripThroughSolarConfigurationJson()
    {
        SolarArrayConfiguration configuration = new()
        {
            Type = SolarArrayType.BaseRotorWithRotor,
            AzimuthAxis = CreateAxisConfiguration("Solar.Azimuth"),
            ElevationAxis = CreateAxisConfiguration("Solar.Elevation"),
            TrackingFrame = new SolarTrackingFrame
            {
                ForwardAtZero = Vector3.UnitY,
                Up = Vector3.UnitZ,
                AzimuthOffsetDegrees = 12.0,
                ElevationOffsetDegrees = -3.0
            }
        };

        string json = JsonSerializer.Serialize(configuration);
        SolarArrayConfiguration? restored =
            JsonSerializer.Deserialize<SolarArrayConfiguration>(json);

        Assert.NotNull(restored);
        Assert.Equal(Vector3.UnitY, restored.TrackingFrame.ForwardAtZero.ToVector3());
        Assert.Equal(Vector3.UnitZ, restored.TrackingFrame.Up.ToVector3());
        Assert.Equal(12.0, restored.TrackingFrame.AzimuthOffsetDegrees);
        Assert.Equal(-3.0, restored.TrackingFrame.ElevationOffsetDegrees);
    }

    private static AxisConfiguration CreateAxisConfiguration(string name)
    {
        return new AxisConfiguration
        {
            Name = name,
            AxisType = AxisType.Rotational,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimitsConfiguration
            {
                MaximumSpeed = 5.0
            },
            Tolerance = 0.1
        };
    }
}
