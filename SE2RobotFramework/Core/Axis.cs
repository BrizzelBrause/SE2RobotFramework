using SE2RobotFramework.Motion;


namespace SE2RobotFramework.Core;


public class Axis
{
    public string Name { get; set; } = string.Empty;

    public double CurrentPosition { get; protected set; }

    public double TargetPosition { get; private set; }

    public MotionLimits MotionLimits { get; set; } = new();

    public MotionProfileType MotionProfileType { get; set; } = MotionProfileType.SCurve;

    public double MinimumPosition { get; set; } = double.NegativeInfinity;

    public double MaximumPosition { get; set; } = double.PositiveInfinity;

    public double Tolerance { get; set; }

    public bool Enabled { get; set; } = true;

    public virtual void UpdatePosition(double position)
    {
        CurrentPosition = position;
    }

    public void SetTargetPosition(double targetPosition)
    {
        if (!double.IsFinite(targetPosition))
        {
            throw new ArgumentOutOfRangeException(nameof(targetPosition));
        }

        TargetPosition = Math.Clamp(
            targetPosition,
            MinimumPosition,
            MaximumPosition);
    }

    public virtual double GetError()
    {
        return TargetPosition - CurrentPosition;
    }

    public bool IsAtTarget()
    {
        return Math.Abs(GetError()) <= Tolerance;
    }

    public int GetDirection()
    {
        return Math.Sign(GetError());
    }
}
