using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.DrillArm;

namespace SE2RobotFramework.Configuration;

public class DrillArmConfigurationApplier
{
    private readonly AxisConfigurationApplier _axisApplier;

    public DrillArmConfigurationApplier()
        : this(new AxisConfigurationApplier())
    {
    }

    public DrillArmConfigurationApplier(AxisConfigurationApplier axisApplier)
    {
        _axisApplier = axisApplier ?? throw new ArgumentNullException(nameof(axisApplier));
    }

    public void Apply(
        DrillArmConfiguration configuration,
        DrillArmMechanism mechanism)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(mechanism);
        configuration.Validate();

        ValidatePistonLayout(
            configuration.UpperArmPistons,
            mechanism.Hardware.UpperArmExtension,
            nameof(configuration.UpperArmPistons));
        ValidatePistonLayout(
            configuration.ForearmPistons,
            mechanism.Hardware.ForearmExtension,
            nameof(configuration.ForearmPistons));
        ValidatePistonLayout(
            configuration.ToolPistons,
            mechanism.Hardware.ToolExtension,
            nameof(configuration.ToolPistons));

        _axisApplier.Apply(configuration.BaseRotation, mechanism.Axes.BaseRotation);
        _axisApplier.Apply(configuration.Shoulder, mechanism.Axes.Shoulder);
        _axisApplier.Apply(configuration.UpperArmExtension, mechanism.Axes.UpperArmExtension);
        _axisApplier.Apply(configuration.Elbow, mechanism.Axes.Elbow);
        _axisApplier.Apply(configuration.ForearmExtension, mechanism.Axes.ForearmExtension);
        _axisApplier.Apply(configuration.WristRotation, mechanism.Axes.WristRotation);
        _axisApplier.Apply(configuration.WristHinge, mechanism.Axes.WristHinge);
        _axisApplier.Apply(configuration.ToolExtension, mechanism.Axes.ToolExtension);
    }

    private static void ValidatePistonLayout(
        PistonBankConfiguration configuration,
        IAxisHardware hardware,
        string configurationName)
    {
        if (hardware is not PistonBankAxisHardware pistonBank)
        {
            return;
        }

        if (configuration.SeriesCount != pistonBank.SeriesCount ||
            configuration.ParallelCount != pistonBank.ParallelCount ||
            (configuration.MaximumRowSynchronizationError ?? double.PositiveInfinity) !=
                pistonBank.MaximumRowPositionDeviation)
        {
            throw new InvalidOperationException(
                $"Changing {configurationName} requires rebuilding its hardware binding.");
        }
    }
}
