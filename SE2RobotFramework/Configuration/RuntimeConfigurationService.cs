using SE2RobotFramework.Mechanisms.DrillArm;
using SE2RobotFramework.Mechanisms.Solar;
using System.Text.Json;

namespace SE2RobotFramework.Configuration;

public class RuntimeConfigurationService
{
    private readonly FrameworkConfigurationSerializer _serializer;
    private readonly DrillArmRuntimeConfigurationApplier _drillArmApplier;
    private readonly SolarArrayRuntimeConfigurationApplier _solarArrayApplier;

    public RuntimeConfigurationService()
        : this(
            new FrameworkConfigurationSerializer(),
            new DrillArmRuntimeConfigurationApplier(),
            new SolarArrayRuntimeConfigurationApplier())
    {
    }

    public RuntimeConfigurationService(
        FrameworkConfigurationSerializer serializer,
        DrillArmRuntimeConfigurationApplier drillArmApplier,
        SolarArrayRuntimeConfigurationApplier solarArrayApplier)
    {
        _serializer = serializer ??
            throw new ArgumentNullException(nameof(serializer));
        _drillArmApplier = drillArmApplier ??
            throw new ArgumentNullException(nameof(drillArmApplier));
        _solarArrayApplier = solarArrayApplier ??
            throw new ArgumentNullException(nameof(solarArrayApplier));
    }

    public DrillArmConfiguration ApplyDrillArm(
        string json,
        DrillArmRuntime runtime)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        DrillArmConfiguration configuration =
            _serializer.DeserializeDrillArm(json);
        _drillArmApplier.Apply(configuration, runtime);
        return configuration;
    }

    public SolarArrayConfiguration ApplySolarArray(
        string json,
        SolarArrayRuntime runtime)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        SolarArrayConfiguration configuration =
            _serializer.DeserializeSolarArray(json);
        _solarArrayApplier.Apply(configuration, runtime);
        return configuration;
    }

    public RuntimeConfigurationApplyResult<DrillArmConfiguration>
        TryApplyDrillArm(string json, DrillArmRuntime runtime)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        return TryApply(
            json,
            _serializer.DeserializeDrillArm,
            configuration => _drillArmApplier.Apply(configuration, runtime));
    }

    public RuntimeConfigurationApplyResult<SolarArrayConfiguration>
        TryApplySolarArray(string json, SolarArrayRuntime runtime)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        return TryApply(
            json,
            _serializer.DeserializeSolarArray,
            configuration => _solarArrayApplier.Apply(configuration, runtime));
    }

    private static RuntimeConfigurationApplyResult<TConfiguration> TryApply<TConfiguration>(
        string json,
        Func<string, TConfiguration> deserialize,
        Action<TConfiguration> apply)
        where TConfiguration : class
    {
        TConfiguration configuration;
        try
        {
            configuration = deserialize(json);
        }
        catch (JsonException exception)
        {
            return Failure<TConfiguration>(
                RuntimeConfigurationError.InvalidDocument,
                exception.Message);
        }
        catch (NotSupportedException exception)
        {
            return Failure<TConfiguration>(
                RuntimeConfigurationError.UnsupportedSchemaVersion,
                exception.Message);
        }
        catch (ArgumentException exception)
        {
            return Failure<TConfiguration>(
                RuntimeConfigurationError.InvalidConfiguration,
                exception.Message);
        }

        try
        {
            apply(configuration);
        }
        catch (InvalidOperationException exception)
        {
            return Failure<TConfiguration>(
                RuntimeConfigurationError.RequiresHardwareRebuild,
                exception.Message);
        }
        catch (ArgumentException exception)
        {
            return Failure<TConfiguration>(
                RuntimeConfigurationError.InvalidConfiguration,
                exception.Message);
        }

        return new RuntimeConfigurationApplyResult<TConfiguration>(
            true,
            configuration,
            RuntimeConfigurationError.None,
            null);
    }

    private static RuntimeConfigurationApplyResult<TConfiguration>
        Failure<TConfiguration>(
            RuntimeConfigurationError error,
            string message)
        where TConfiguration : class
    {
        return new RuntimeConfigurationApplyResult<TConfiguration>(
            false,
            null,
            error,
            message);
    }
}
