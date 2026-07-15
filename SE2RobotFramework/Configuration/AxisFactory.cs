using SE2RobotFramework.Core;

namespace SE2RobotFramework.Configuration;

public class AxisFactory
{
    private readonly AxisConfigurationApplier _configurationApplier;

    public AxisFactory()
        : this(new AxisConfigurationApplier())
    {
    }

    public AxisFactory(AxisConfigurationApplier configurationApplier)
    {
        _configurationApplier = configurationApplier ??
            throw new ArgumentNullException(nameof(configurationApplier));
    }

    public Axis Create(AxisConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        configuration.Validate();

        Axis axis = configuration.AxisType switch
        {
            AxisType.Linear => new LinearAxis(),
            AxisType.Rotational => new RotationalAxis(),
            _ => throw new ArgumentOutOfRangeException(nameof(configuration.AxisType))
        };

        _configurationApplier.Apply(configuration, axis);

        return axis;
    }
}
