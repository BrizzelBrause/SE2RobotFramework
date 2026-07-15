namespace SE2RobotFramework.Mechanisms.DrillArm;

public readonly record struct DrillArmMouseInput(
    double HorizontalDelta,
    double VerticalDelta)
{
    public void Validate()
    {
        if (!double.IsFinite(HorizontalDelta) || !double.IsFinite(VerticalDelta))
        {
            throw new ArgumentException("Mouse input deltas must be finite values.");
        }
    }
}
