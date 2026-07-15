using System.Numerics;

namespace SE2RobotFramework.Mechanisms.Solar;

public class SolarOrientationSolver
{
    private const double RadiansToDegrees = 180.0 / Math.PI;

    public SolarOrientation Calculate(
        Vector3 sunDirection,
        SolarTrackingFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);
        frame.Validate();
        ValidateSunDirection(sunDirection);

        Vector3 direction = Vector3.Normalize(sunDirection);
        Vector3 forward = Vector3.Normalize(frame.ForwardAtZero);
        Vector3 up = Vector3.Normalize(frame.Up);
        Vector3 right = Vector3.Normalize(Vector3.Cross(up, forward));

        double forwardComponent = Vector3.Dot(direction, forward);
        double rightComponent = Vector3.Dot(direction, right);
        double upComponent = Vector3.Dot(direction, up);
        double horizontalLength = Math.Sqrt(
            forwardComponent * forwardComponent +
            rightComponent * rightComponent);

        double azimuth = Math.Atan2(rightComponent, forwardComponent) *
            RadiansToDegrees + frame.AzimuthOffsetDegrees;
        double elevation = Math.Atan2(upComponent, horizontalLength) *
            RadiansToDegrees + frame.ElevationOffsetDegrees;

        return new SolarOrientation(
            NormalizeDegrees(azimuth),
            elevation);
    }

    private static double NormalizeDegrees(double angle)
    {
        angle %= 360.0;
        return angle < 0.0 ? angle + 360.0 : angle;
    }

    private static void ValidateSunDirection(Vector3 direction)
    {
        if (!float.IsFinite(direction.X) ||
            !float.IsFinite(direction.Y) ||
            !float.IsFinite(direction.Z) ||
            direction.LengthSquared() <= 0.000001f)
        {
            throw new ArgumentException(
                "A finite, non-zero sun direction is required.",
                nameof(direction));
        }
    }
}
