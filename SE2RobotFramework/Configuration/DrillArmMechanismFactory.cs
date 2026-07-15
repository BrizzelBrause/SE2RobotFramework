using SE2RobotFramework.Core;
using SE2RobotFramework.Mechanisms.DrillArm;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Configuration;

public class DrillArmMechanismFactory
{
    private readonly AxisFactory _axisFactory;
    private readonly DrillArmConfigurationApplier _configurationApplier;
    private readonly IMotionProfileFactory _profileFactory;
    private readonly MotionRequestFactory _requestFactory;

    public DrillArmMechanismFactory()
        : this(
            new AxisFactory(),
            new DrillArmConfigurationApplier(),
            new MotionProfileFactory(),
            new MotionRequestFactory())
    {
    }

    public DrillArmMechanismFactory(
        AxisFactory axisFactory,
        DrillArmConfigurationApplier configurationApplier,
        IMotionProfileFactory profileFactory,
        MotionRequestFactory requestFactory)
    {
        _axisFactory = axisFactory ?? throw new ArgumentNullException(nameof(axisFactory));
        _configurationApplier = configurationApplier ??
            throw new ArgumentNullException(nameof(configurationApplier));
        _profileFactory = profileFactory ?? throw new ArgumentNullException(nameof(profileFactory));
        _requestFactory = requestFactory ?? throw new ArgumentNullException(nameof(requestFactory));
    }

    public DrillArmMechanism Create(
        DrillArmConfiguration configuration,
        DrillArmHardware hardware)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(hardware);
        configuration.Validate();

        DrillArmAxes axes = new()
        {
            BaseRotation = CreateRotational(configuration.BaseRotation),
            Shoulder = CreateRotational(configuration.Shoulder),
            UpperArmExtension = CreateLinear(configuration.UpperArmExtension),
            Elbow = CreateRotational(configuration.Elbow),
            ForearmExtension = CreateLinear(configuration.ForearmExtension),
            ForearmHinge = CreateRotational(configuration.ForearmHinge),
            WristRotation = CreateRotational(configuration.WristRotation),
            WristHinge = CreateRotational(configuration.WristHinge),
            ToolExtension = CreateLinear(configuration.ToolExtension)
        };

        DrillArmMechanism mechanism = new(
            hardware,
            axes,
            _profileFactory,
            _requestFactory);

        _configurationApplier.Apply(configuration, mechanism);

        return mechanism;
    }

    private RotationalAxis CreateRotational(AxisConfiguration configuration)
    {
        return (RotationalAxis)_axisFactory.Create(configuration);
    }

    private LinearAxis CreateLinear(AxisConfiguration configuration)
    {
        return (LinearAxis)_axisFactory.Create(configuration);
    }
}
