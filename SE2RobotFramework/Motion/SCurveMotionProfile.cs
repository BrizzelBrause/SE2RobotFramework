using System.Diagnostics;

namespace SE2RobotFramework.Motion;

public class SCurveMotionProfile : IMotionProfile
{
    private const double MotionTolerance = 0.0001;

    private readonly MotionIntegrator _integrator = new MotionIntegrator();

    private readonly StoppingDistanceCalculator _stoppingDistanceCalculator = new StoppingDistanceCalculator();

    private StoppingProfileType DetermineStoppingProfile(MotionRequest request)
    {
        return _stoppingDistanceCalculator.DetermineProfileType(request.CurrentVelocity, request.CurrentAcceleration, request.MaximumAcceleration, request.MaximumJerk);
    }

    private static bool MustReleaseBrakingAcceleration(MotionRequest request, double accelerationInTravelDirection, double velocityLostDuringAccelerationRelease, StoppingProfileType profileType)
    {
        double releaseVelocityTolerance = IsMovingTowardTarget(request) && profileType == StoppingProfileType.FivePhase ? MotionTolerance : 0.0;

        return IsAccelerationReleaseRequired(-accelerationInTravelDirection, Math.Abs(request.CurrentVelocity), velocityLostDuringAccelerationRelease + releaseVelocityTolerance);
    }

    private static bool MustHoldMaximumBrakingAcceleration(double accelerationInTravelDirection, double maximumAcceleration)
    {
        return HasReachedMaximumAcceleration(-accelerationInTravelDirection, maximumAcceleration);
    }

    private static double CalculateJerkDuringBraking(MotionRequest request, double travelDirection,  double accelerationInTravelDirection, double velocityLostDuringAccelerationRelease, StoppingProfileType profileType)
    {
        return profileType == StoppingProfileType.SevenPhase ? CalculateSevenPhaseBrakingJerk(request, travelDirection, accelerationInTravelDirection, velocityLostDuringAccelerationRelease) : CalculateBrakingJerkForProfile(request, travelDirection, accelerationInTravelDirection, velocityLostDuringAccelerationRelease, profileType);
    }

    private static double CalculateBrakingJerkForProfile(MotionRequest request, double travelDirection, double accelerationInTravelDirection, double velocityLostDuringAccelerationRelease, StoppingProfileType profileType)
    {
        if (MustReleaseBrakingAcceleration(request, accelerationInTravelDirection, velocityLostDuringAccelerationRelease, profileType))
        {
            return CalculateJerkToZero(request);
        }

        return CalculateJerkInDirection(request.MaximumJerk, -travelDirection);
    }

    private static double CalculateSevenPhaseBrakingJerk(MotionRequest request, double travelDirection, double accelerationInTravelDirection, double velocityLostDuringAccelerationRelease)
    {
        if (MustHoldMaximumBrakingAcceleration(accelerationInTravelDirection, request.MaximumAcceleration))
        {
            return 0.0;
        }

        return CalculateBrakingJerkForProfile(request, travelDirection, accelerationInTravelDirection, velocityLostDuringAccelerationRelease, StoppingProfileType.SevenPhase);
    }

    private static double CalculateBrakingJerkAtStandstill(MotionRequest request)
    {
        return IsNearZero(request.CurrentAcceleration) ? 0.0 : CalculateJerkToZero(request);
    }

    private static double CalculateBrakingJerkWhileMoving(MotionRequest request, StoppingProfileType profileType)
    {
        double travelDirection = GetTravelDirection(request);

        return CalculateJerkDuringBraking(request, travelDirection, CalculateAccelerationInDirection(request, travelDirection), CalculateVelocityChangeDuringAccelerationRelease(request), profileType);
    }

    private static double CalculateBrakingJerk(MotionRequest request, StoppingProfileType profileType)
    {
        if (IsNearZero(request.CurrentVelocity))
        {
            return CalculateBrakingJerkAtStandstill(request);
        }

        return CalculateBrakingJerkWhileMoving(request, profileType);
    }

    private static void ValidateRequest(MotionRequest request)
    {
        if (request.DeltaTime <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.DeltaTime));
        }

        if (request.MaximumJerk <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.MaximumJerk));
        }

        if (request.MaximumAcceleration <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.MaximumAcceleration));
        }
    }

    public MotionState Calculate(MotionRequest request)
    {
        ValidateRequest(request);

        MotionPhase phase = DeterminePhase(request);

        StoppingProfileType profileType = DetermineStoppingProfile(request);

        return _integrator.Integrate(request, CalculateDesiredJerk(request, phase, profileType));
    }

    private static double CalculateDesiredJerk(MotionRequest request, MotionPhase phase, StoppingProfileType profileType)
    {
        return phase switch
        {
            MotionPhase.Braking => CalculateBrakingJerk(request, profileType),

            MotionPhase.Accelerating => CalculateJerkDuringAcceleration(request),

            MotionPhase.Cruising => CalculateCruisingJerk(request),

            MotionPhase.Idle => 0.0,

            _ => throw new UnreachableException()
        };
    }

    private static double CalculateCruisingJerk(MotionRequest request)
    {
        if (IsNearZero(request.CurrentAcceleration))
        {
            return 0.0;
        }

        return CalculateJerkToZero(request);
    }

    private static double CalculateJerkToZero(MotionRequest request)
    {
        double jerkToZero = -request.CurrentAcceleration / request.DeltaTime;

        return Math.Clamp(jerkToZero, -request.MaximumJerk,  request.MaximumJerk);
    }

    private static double CalculateVelocityChangeDuringAccelerationRelease(MotionRequest request)
    {
        return
            request.CurrentAcceleration * request.CurrentAcceleration / (2.0 * request.MaximumJerk);
    }

    private static double CalculateAccelerationInDirection(MotionRequest request, double direction)
    {
        return
            request.CurrentAcceleration * direction;
    }

    private static bool IsAccelerationReleaseRequired(double accelerationInDirection, double remainingVelocity, double velocityChangeDuringRelease)
    {
        return
            accelerationInDirection > 0.0 && remainingVelocity <= velocityChangeDuringRelease;
    }

    private static bool HasReachedMaximumAcceleration(double accelerationInDirection, double maximumAcceleration)
    {
        return
            accelerationInDirection >= maximumAcceleration;
    }

    private static double CalculateJerkInDirection(double maximumJerk, double direction)
    {
        return maximumJerk * direction;
    }

    private static double GetTravelDirection(MotionRequest request)
    {
        return Math.Sign(request.CurrentVelocity);
    }

    private static bool IsNearZero(double value)
    {
        return Math.Abs(value) <= MotionTolerance;
    }

    private static bool IsMovingTowardTarget(MotionRequest request)
    {
        return
            Math.Sign(request.CurrentVelocity) == request.Direction;
    }

    private static bool HasResidualMotion(MotionRequest request)
    {
        return
            !IsNearZero(request.CurrentVelocity) || !IsNearZero(request.CurrentAcceleration);
    }

    private bool MustBrake(MotionRequest request)
    {
        if (!IsMovingTowardTarget(request) && !IsNearZero(request.CurrentVelocity))
        {
            return true;
        }

        double stoppingDistance = _stoppingDistanceCalculator.CalculateStoppingDistance(request.CurrentVelocity, request.CurrentAcceleration, request.MaximumAcceleration, request.MaximumJerk);

        return stoppingDistance >= request.RemainingDistance;
    }

    private static bool HasReachedMaximumSpeed(MotionRequest request)
    {
        return
            Math.Abs(request.CurrentVelocity) >= request.MaximumSpeed;
    }

    private static bool MustReleaseAcceleration(double accelerationInTargetDirection, double remainingVelocity, double velocityGainDuringAccelerationRelease)
    {
        return IsAccelerationReleaseRequired(accelerationInTargetDirection, remainingVelocity, velocityGainDuringAccelerationRelease);
    }

    private static bool MustHoldMaximumAcceleration(double accelerationInTargetDirection, double maximumAcceleration)
    {
        return HasReachedMaximumAcceleration(accelerationInTargetDirection, maximumAcceleration);
    }

    private static double CalculateJerkDuringAcceleration(MotionRequest request)
    {
        return CalculateJerkDuringAcceleration(request, CalculateAccelerationInDirection(request, request.Direction), request.MaximumSpeed - Math.Abs(request.CurrentVelocity), CalculateVelocityChangeDuringAccelerationRelease(request));
    }

    private static double CalculateJerkDuringAcceleration(MotionRequest request, double accelerationInTargetDirection, double remainingVelocity, double velocityGainDuringAccelerationRelease)
    {
        if (MustReleaseAcceleration(accelerationInTargetDirection, remainingVelocity, velocityGainDuringAccelerationRelease))
        {
            return CalculateJerkToZero(request);
        }

        if (MustHoldMaximumAcceleration(accelerationInTargetDirection, request.MaximumAcceleration))
        {
            return 0.0;
        }

        return CalculateJerkInDirection(request.MaximumJerk, request.Direction);
    }

    private MotionPhase DeterminePhase(MotionRequest request)
    {
        if (request.Direction == 0)
        {
            return HasResidualMotion(request) ? MotionPhase.Braking : MotionPhase.Idle;
        }

        if (MustBrake(request))
        {
            return MotionPhase.Braking;
        }

        if (HasReachedMaximumSpeed(request))
        {
            return MotionPhase.Cruising;
        }

        return MotionPhase.Accelerating;
    }
}