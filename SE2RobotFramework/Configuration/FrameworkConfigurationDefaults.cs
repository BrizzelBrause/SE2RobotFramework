using SE2RobotFramework.Mechanisms.DrillArm;
using SE2RobotFramework.Mechanisms.Solar;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Configuration;

public static class FrameworkConfigurationDefaults
{
    public static SolarArrayConfiguration CreateSolarArray(
        SolarArrayType type,
        MotionProfileType motionProfileType = MotionProfileType.SCurve)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type));
        }

        SolarArrayConfiguration configuration = new()
        {
            Type = type,
            AzimuthAxis = CreateRotationalAxis(
                "Solar.Azimuth",
                motionProfileType,
                wrapAround: true),
            ElevationAxis = CreateRotationalAxis(
                "Solar.Elevation",
                motionProfileType,
                minimumPosition: type == SolarArrayType.BaseRotorWithHinge
                    ? 0.0
                    : null,
                maximumPosition: type == SolarArrayType.BaseRotorWithHinge
                    ? 180.0
                    : null,
                wrapAround: type != SolarArrayType.BaseRotorWithHinge)
        };
        configuration.Validate();
        return configuration;
    }

    public static DrillArmConfiguration CreateDrillArm(
        MotionProfileType motionProfileType = MotionProfileType.SCurve,
        int upperArmSeriesCount = 4,
        int upperArmParallelCount = 5,
        int forearmSeriesCount = 3,
        int forearmParallelCount = 5,
        int toolSeriesCount = 1,
        int toolParallelCount = 1)
    {
        PistonBankConfiguration upperArmPistons = CreatePistonBank(
            upperArmSeriesCount,
            upperArmParallelCount);
        PistonBankConfiguration forearmPistons = CreatePistonBank(
            forearmSeriesCount,
            forearmParallelCount);
        PistonBankConfiguration toolPistons = CreatePistonBank(
            toolSeriesCount,
            toolParallelCount);

        DrillArmConfiguration configuration = new()
        {
            BaseRotation = CreateRotationalAxis(
                "DrillArm.Base",
                motionProfileType,
                0.0,
                100.0),
            Shoulder = CreateRotationalAxis(
                "DrillArm.Shoulder",
                motionProfileType,
                90.0,
                180.0),
            UpperArmExtension = CreateLinearAxis(
                "DrillArm.UpperArm",
                motionProfileType,
                upperArmSeriesCount * DrillArmPositionLimits.PistonStrokeMeters),
            UpperArmPistons = upperArmPistons,
            Elbow = CreateRotationalAxis(
                "DrillArm.Elbow",
                motionProfileType,
                0.0,
                90.0),
            ForearmExtension = CreateLinearAxis(
                "DrillArm.Forearm",
                motionProfileType,
                forearmSeriesCount * DrillArmPositionLimits.PistonStrokeMeters),
            ForearmPistons = forearmPistons,
            ForearmHinge = CreateRotationalAxis(
                "DrillArm.ForearmHinge",
                motionProfileType,
                0.0,
                180.0),
            WristRotation = CreateRotationalAxis(
                "DrillArm.WristRotation",
                motionProfileType,
                wrapAround: true),
            WristHinge = CreateRotationalAxis(
                "DrillArm.WristHinge",
                motionProfileType,
                0.0,
                180.0),
            ToolExtension = CreateLinearAxis(
                "DrillArm.Tool",
                motionProfileType,
                toolSeriesCount * DrillArmPositionLimits.PistonStrokeMeters),
            ToolPistons = toolPistons
        };
        configuration.Validate();
        return configuration;
    }

    private static AxisConfiguration CreateRotationalAxis(
        string name,
        MotionProfileType motionProfileType,
        double? minimumPosition = null,
        double? maximumPosition = null,
        bool wrapAround = false)
    {
        return new AxisConfiguration
        {
            Name = name,
            AxisType = AxisType.Rotational,
            MotionProfileType = motionProfileType,
            MotionLimits = CreateMotionLimits(
                maximumSpeed: 5.0,
                maximumAcceleration: 2.0,
                maximumJerk: 5.0),
            MinimumPosition = minimumPosition,
            MaximumPosition = maximumPosition,
            Tolerance = 0.1,
            WrapAround = wrapAround
        };
    }

    private static AxisConfiguration CreateLinearAxis(
        string name,
        MotionProfileType motionProfileType,
        double maximumPosition)
    {
        return new AxisConfiguration
        {
            Name = name,
            AxisType = AxisType.Linear,
            MotionProfileType = motionProfileType,
            MotionLimits = CreateMotionLimits(
                maximumSpeed: 1.0,
                maximumAcceleration: 1.0,
                maximumJerk: 2.0),
            MinimumPosition = 0.0,
            MaximumPosition = maximumPosition,
            Tolerance = 0.01
        };
    }

    private static MotionLimitsConfiguration CreateMotionLimits(
        double maximumSpeed,
        double maximumAcceleration,
        double maximumJerk)
    {
        return new MotionLimitsConfiguration
        {
            MaximumSpeed = maximumSpeed,
            MaximumAcceleration = maximumAcceleration,
            MaximumJerk = maximumJerk
        };
    }

    private static PistonBankConfiguration CreatePistonBank(
        int seriesCount,
        int parallelCount)
    {
        PistonBankConfiguration configuration = new()
        {
            SeriesCount = seriesCount,
            ParallelCount = parallelCount
        };
        configuration.Validate();
        return configuration;
    }
}
