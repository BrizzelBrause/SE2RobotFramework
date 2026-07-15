using SE2RobotFramework.Hardware;

namespace SE2RobotFramework.Mechanisms.Solar;

public class SolarArrayHardwareFactory
{
    public SolarArrayHardware Create(
        SolarArrayType type,
        IAxisHardware baseRotor,
        IEnumerable<IAxisHardware> elevationActuators,
        double maximumElevationSynchronizationError = double.PositiveInfinity)
    {
        ArgumentNullException.ThrowIfNull(baseRotor);
        ArgumentNullException.ThrowIfNull(elevationActuators);

        IAxisHardware[] actuators = elevationActuators.ToArray();

        return type switch
        {
            SolarArrayType.BaseRotorWithHinge => CreateSingle(
                type,
                baseRotor,
                actuators),
            SolarArrayType.BaseRotorWithRotor => CreateSingle(
                type,
                baseRotor,
                actuators),
            SolarArrayType.BaseRotorWithDualRotors => CreateDualRotors(
                type,
                baseRotor,
                actuators,
                maximumElevationSynchronizationError),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    private static SolarArrayHardware CreateSingle(
        SolarArrayType type,
        IAxisHardware baseRotor,
        IReadOnlyList<IAxisHardware> actuators)
    {
        ValidateActuatorCount(actuators, 1);

        return new SolarArrayHardware(type, baseRotor, actuators[0]);
    }

    private static SolarArrayHardware CreateDualRotors(
        SolarArrayType type,
        IAxisHardware baseRotor,
        IReadOnlyList<IAxisHardware> actuators,
        double maximumSynchronizationError)
    {
        ValidateActuatorCount(actuators, 2);

        ParallelAxisHardware elevation = new(new IAxisHardware[]
        {
            new TransformedAxisHardware(actuators[0]),
            new TransformedAxisHardware(actuators[1], scale: -1.0)
        },
        maximumPositionDeviation: maximumSynchronizationError,
        positionPeriod: 360.0);

        return new SolarArrayHardware(type, baseRotor, elevation);
    }

    private static void ValidateActuatorCount(
        IReadOnlyCollection<IAxisHardware> actuators,
        int expectedCount)
    {
        if (actuators.Count != expectedCount)
        {
            throw new ArgumentException(
                $"This solar array type requires exactly {expectedCount} elevation actuator(s).",
                nameof(actuators));
        }

        if (actuators.Any(actuator => actuator is null))
        {
            throw new ArgumentException(
                "Elevation actuators cannot contain null values.",
                nameof(actuators));
        }
    }
}
