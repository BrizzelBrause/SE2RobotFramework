namespace SE2RobotFramework.Configuration;

using SE2RobotFramework.Mechanisms.DrillArm;

public class DrillArmConfiguration
{
    public AxisConfiguration BaseRotation { get; init; } = new();

    public AxisConfiguration Shoulder { get; init; } = new();

    public AxisConfiguration UpperArmExtension { get; init; } = new();

    public PistonBankConfiguration UpperArmPistons { get; init; } = new();

    public AxisConfiguration Elbow { get; init; } = new();

    public AxisConfiguration ForearmExtension { get; init; } = new();

    public PistonBankConfiguration ForearmPistons { get; init; } = new();

    public AxisConfiguration ForearmHinge { get; init; } = new();

    public AxisConfiguration WristRotation { get; init; } = new();

    public AxisConfiguration WristHinge { get; init; } = new();

    public AxisConfiguration ToolExtension { get; init; } = new();

    public PistonBankConfiguration ToolPistons { get; init; } = new();

    public DrillArmMouseControlConfiguration MouseControl { get; init; } = new();

    public void Validate()
    {
        ValidateAxis(BaseRotation, AxisType.Rotational, nameof(BaseRotation));
        ValidateAxis(Shoulder, AxisType.Rotational, nameof(Shoulder));
        ValidateAxis(UpperArmExtension, AxisType.Linear, nameof(UpperArmExtension));
        ValidatePistonBank(UpperArmPistons, nameof(UpperArmPistons));
        ValidateAxis(Elbow, AxisType.Rotational, nameof(Elbow));
        ValidateAxis(ForearmExtension, AxisType.Linear, nameof(ForearmExtension));
        ValidatePistonBank(ForearmPistons, nameof(ForearmPistons));
        ValidateAxis(ForearmHinge, AxisType.Rotational, nameof(ForearmHinge));
        ValidateAxis(WristRotation, AxisType.Rotational, nameof(WristRotation));
        ValidateAxis(WristHinge, AxisType.Rotational, nameof(WristHinge));
        ValidateAxis(ToolExtension, AxisType.Linear, nameof(ToolExtension));
        ValidatePistonBank(ToolPistons, nameof(ToolPistons));
        ArgumentNullException.ThrowIfNull(MouseControl);
        MouseControl.Validate();
    }

    private static void ValidateAxis(
        AxisConfiguration configuration,
        AxisType expectedType,
        string parameterName)
    {
        ArgumentNullException.ThrowIfNull(configuration, parameterName);
        configuration.Validate();

        if (configuration.AxisType != expectedType)
        {
            throw new ArgumentException(
                $"This axis must be {expectedType}.",
                parameterName);
        }
    }

    private static void ValidatePistonBank(
        PistonBankConfiguration configuration,
        string parameterName)
    {
        ArgumentNullException.ThrowIfNull(configuration, parameterName);
        configuration.Validate();
    }
}
