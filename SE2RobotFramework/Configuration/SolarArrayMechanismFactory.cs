using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Mechanisms.Solar;
using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Configuration;

public class SolarArrayMechanismFactory
{
    private readonly AxisFactory _axisFactory;
    private readonly SolarArrayHardwareFactory _hardwareFactory;
    private readonly IMotionProfileFactory _profileFactory;
    private readonly MotionRequestFactory _requestFactory;

    public SolarArrayMechanismFactory()
        : this(
            new AxisFactory(),
            new SolarArrayHardwareFactory(),
            new MotionProfileFactory(),
            new MotionRequestFactory())
    {
    }

    public SolarArrayMechanismFactory(
        AxisFactory axisFactory,
        SolarArrayHardwareFactory hardwareFactory,
        IMotionProfileFactory profileFactory,
        MotionRequestFactory requestFactory)
    {
        _axisFactory = axisFactory ?? throw new ArgumentNullException(nameof(axisFactory));
        _hardwareFactory = hardwareFactory ?? throw new ArgumentNullException(nameof(hardwareFactory));
        _profileFactory = profileFactory ?? throw new ArgumentNullException(nameof(profileFactory));
        _requestFactory = requestFactory ?? throw new ArgumentNullException(nameof(requestFactory));
    }

    public SolarArrayMechanism Create(
        SolarArrayConfiguration configuration,
        IAxisHardware baseRotor,
        IEnumerable<IAxisHardware> elevationActuators)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        configuration.Validate();

        SolarArrayHardware hardware = _hardwareFactory.Create(
            configuration.Type,
            baseRotor,
            elevationActuators,
            configuration.MaximumElevationSynchronizationError ??
                double.PositiveInfinity);
        RotationalAxis azimuth = (RotationalAxis)_axisFactory.Create(
            configuration.AzimuthAxis);
        RotationalAxis elevation = (RotationalAxis)_axisFactory.Create(
            configuration.ElevationAxis);

        return new SolarArrayMechanism(
            hardware,
            azimuth,
            elevation,
            _profileFactory,
            _requestFactory);
    }
}
