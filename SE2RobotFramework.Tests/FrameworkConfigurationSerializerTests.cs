using System.Numerics;
using System.Text.Json;
using SE2RobotFramework.Configuration;
using SE2RobotFramework.Mechanisms.Solar;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class FrameworkConfigurationSerializerTests
{
    [Fact]
    public void SolarConfiguration_RoundTrip_PreservesSettingsAndUsesReadableEnums()
    {
        SolarArrayConfiguration original = new()
        {
            Type = SolarArrayType.BaseRotorWithDualRotors,
            AzimuthAxis = CreateAxis("Solar.Azimuth", AxisType.Rotational, 5.0),
            ElevationAxis = CreateAxis("Solar.Elevation", AxisType.Rotational, 3.0),
            TrackingFrame = new SolarTrackingFrame
            {
                ForwardAtZero = Vector3.UnitY,
                Up = Vector3.UnitZ,
                AzimuthOffsetDegrees = 12.0,
                ElevationOffsetDegrees = -4.0
            },
            MaximumElevationSynchronizationError = 2.0
        };
        FrameworkConfigurationSerializer serializer = new();

        string json = serializer.SerializeSolarArray(original);
        SolarArrayConfiguration restored = serializer.DeserializeSolarArray(json);

        Assert.Contains("\"schemaVersion\": 1", json);
        Assert.Contains("\"baseRotorWithDualRotors\"", json);
        Assert.Contains("\"linear\"", json);
        Assert.Equal(original.Type, restored.Type);
        Assert.Equal(Vector3.UnitY, restored.TrackingFrame.ForwardAtZero.ToVector3());
        Assert.Equal(2.0, restored.MaximumElevationSynchronizationError);
    }

    [Fact]
    public void DrillArmConfiguration_RoundTrip_PreservesPistonLayouts()
    {
        DrillArmConfiguration original = CreateDrillArmConfiguration();
        FrameworkConfigurationSerializer serializer = new();

        string json = serializer.SerializeDrillArm(original);
        DrillArmConfiguration restored = serializer.DeserializeDrillArm(json);

        Assert.Equal(4, restored.UpperArmPistons.SeriesCount);
        Assert.Equal(5, restored.UpperArmPistons.ParallelCount);
        Assert.Equal(3, restored.ForearmPistons.SeriesCount);
        Assert.Equal("DrillArm.Tool", restored.ToolExtension.Name);
    }

    [Fact]
    public void Deserialize_WithUnsupportedSchemaVersion_Throws()
    {
        const string json = """
            {
              "schemaVersion": 99,
              "configuration": {}
            }
            """;

        Assert.Throws<NotSupportedException>(() =>
            new FrameworkConfigurationSerializer().DeserializeSolarArray(json));
    }

    [Fact]
    public void Deserialize_WithMissingPayload_Throws()
    {
        const string json = """
            {
              "schemaVersion": 1
            }
            """;

        Assert.Throws<JsonException>(() =>
            new FrameworkConfigurationSerializer().DeserializeSolarArray(json));
    }

    private static DrillArmConfiguration CreateDrillArmConfiguration()
    {
        return new DrillArmConfiguration
        {
            BaseRotation = CreateAxis("DrillArm.Base", AxisType.Rotational, 5.0),
            Shoulder = CreateAxis("DrillArm.Shoulder", AxisType.Rotational, 5.0),
            UpperArmExtension = CreateAxis("DrillArm.UpperArm", AxisType.Linear, 4.0),
            UpperArmPistons = new PistonBankConfiguration
            {
                SeriesCount = 4,
                ParallelCount = 5
            },
            Elbow = CreateAxis("DrillArm.Elbow", AxisType.Rotational, 3.0),
            ForearmExtension = CreateAxis("DrillArm.Forearm", AxisType.Linear, 4.0),
            ForearmPistons = new PistonBankConfiguration
            {
                SeriesCount = 3,
                ParallelCount = 5
            },
            ForearmHinge = CreateAxis("DrillArm.ForearmHinge", AxisType.Rotational, 3.0),
            WristRotation = CreateAxis("DrillArm.WristRotation", AxisType.Rotational, 2.0),
            WristHinge = CreateAxis("DrillArm.WristHinge", AxisType.Rotational, 2.0),
            ToolExtension = CreateAxis("DrillArm.Tool", AxisType.Linear, 1.0),
            ToolPistons = new PistonBankConfiguration
            {
                SeriesCount = 1,
                ParallelCount = 1
            }
        };
    }

    private static AxisConfiguration CreateAxis(
        string name,
        AxisType type,
        double maximumSpeed)
    {
        return new AxisConfiguration
        {
            Name = name,
            AxisType = type,
            MotionProfileType = MotionProfileType.Linear,
            MotionLimits = new MotionLimitsConfiguration
            {
                MaximumSpeed = maximumSpeed
            },
            Tolerance = 0.1
        };
    }
}
