using System.Numerics;

namespace SE2RobotFramework.Mechanisms.Solar;

public class SolarTrackingFrame
{
    private const float VectorTolerance = 0.000001f;

    public Vector3Value ForwardAtZero { get; init; } = new(1.0, 0.0, 0.0);

    public Vector3Value Up { get; init; } = new(0.0, 0.0, 1.0);

    public double AzimuthOffsetDegrees { get; init; }

    public double ElevationOffsetDegrees { get; init; }

    public void Validate()
    {
        ValidateVector(ForwardAtZero, nameof(ForwardAtZero));
        ValidateVector(Up, nameof(Up));

        Vector3 forward = Vector3.Normalize(ForwardAtZero);
        Vector3 up = Vector3.Normalize(Up);

        if (Math.Abs(Vector3.Dot(forward, up)) > VectorTolerance)
        {
            throw new ArgumentException(
                "The zero direction and up axis must be perpendicular.");
        }

        if (!double.IsFinite(AzimuthOffsetDegrees))
        {
            throw new ArgumentOutOfRangeException(nameof(AzimuthOffsetDegrees));
        }

        if (!double.IsFinite(ElevationOffsetDegrees))
        {
            throw new ArgumentOutOfRangeException(nameof(ElevationOffsetDegrees));
        }
    }

    private static void ValidateVector(Vector3Value value, string parameterName)
    {
        Vector3 vector = value;

        if (!double.IsFinite(value.X) ||
            !double.IsFinite(value.Y) ||
            !double.IsFinite(value.Z) ||
            vector.LengthSquared() <= VectorTolerance)
        {
            throw new ArgumentException(
                "A finite, non-zero vector is required.",
                parameterName);
        }
    }
}
