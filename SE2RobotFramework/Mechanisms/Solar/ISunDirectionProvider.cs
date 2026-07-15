using System.Numerics;

namespace SE2RobotFramework.Mechanisms.Solar;

public interface ISunDirectionProvider
{
    bool TryGetSunDirection(out Vector3 sunDirection);
}
