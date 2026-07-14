namespace SE2RobotFramework.Core;

public class RotationalAxis : Axis
{
    public bool WrapAround { get; set; }

    public double NormalizeAngle(double angle)
    {
        angle %= 360.0;

        if (angle < 0)
        {
            angle += 360.0;
        }

        return angle;
    }

    public override void UpdatePosition(double position)
    {
        CurrentPosition = WrapAround
            ? NormalizeAngle(position)
            : position;
    }

    public override double GetError()
    {
        if (!WrapAround)
        {
            return base.GetError();
        }

        double error = TargetPosition - CurrentPosition;

        error = (error + 180.0) % 360.0;

        if (error < 0)
        {
            error += 360.0;
        }

        return error - 180.0;
    }
}
