using System.Numerics;

namespace SE2RobotFramework.Mechanisms.Solar;

public readonly record struct Vector3Value(double X, double Y, double Z)
{
    public Vector3 ToVector3()
    {
        return new Vector3((float)X, (float)Y, (float)Z);
    }

    public static implicit operator Vector3(Vector3Value value)
    {
        return value.ToVector3();
    }

    public static implicit operator Vector3Value(Vector3 value)
    {
        return new Vector3Value(value.X, value.Y, value.Z);
    }
}
