namespace SE2RobotFramework.Motion;

public class StoppingDistanceCalculator
{
    public double CalculateStoppingTime(double currentVelocity, double currentAcceleration, double maximumAcceleration, double maximumJerk)
    {
        double velocity = Math.Abs(currentVelocity);
        double jerk = Math.Abs(maximumJerk);
        double maximumDeceleration = Math.Abs(maximumAcceleration);

        if (velocity <= 0.0)
        {
            return 0.0;
        }

        if (jerk <= 0.0)
        {
            return double.PositiveInfinity;
        }

        double accelerationInTravelDirection = Math.Sign(currentVelocity) * currentAcceleration;

        StoppingProfileType profileType = DetermineProfileType(currentVelocity, currentAcceleration, maximumAcceleration, maximumJerk);

        double timeToMaximumDeceleration = CalculateTimeToMaximumDeceleration(accelerationInTravelDirection, maximumDeceleration, jerk);

        double velocityAtMaximumDeceleration = CalculateVelocityAtMaximumDeceleration(velocity, accelerationInTravelDirection, jerk, timeToMaximumDeceleration);

        if (profileType == StoppingProfileType.ThreePhase)
        {
            if (accelerationInTravelDirection > 0.0)
            {
                return (accelerationInTravelDirection + 2.0 * Math.Sqrt(0.5 * accelerationInTravelDirection * accelerationInTravelDirection + jerk * velocity)) / jerk;
            }

            return (accelerationInTravelDirection + Math.Sqrt(accelerationInTravelDirection * accelerationInTravelDirection + 2.0 * jerk * velocity)) / jerk;
        }

        double timeToReleaseAcceleration = profileType == StoppingProfileType.SevenPhase ? CalculateTimeToReleaseAcceleration(maximumDeceleration, jerk) : 0.0;

        double velocityLostDuringRelease = profileType == StoppingProfileType.SevenPhase ? CalculateVelocityLostDuringRelease(maximumDeceleration, timeToReleaseAcceleration) : 0.0;

        if (profileType == StoppingProfileType.FivePhase)
        {
            return (accelerationInTravelDirection + 2.0 * Math.Sqrt(0.5 * accelerationInTravelDirection * accelerationInTravelDirection + jerk * velocity)) / jerk;
        }

        double velocityBeforeRelease = CalculateVelocityBeforeRelease(velocityAtMaximumDeceleration, velocityLostDuringRelease);

        double constantDecelerationTime = CalculateConstantDecelerationTime(velocityBeforeRelease, maximumDeceleration);

        return
            timeToMaximumDeceleration + constantDecelerationTime + timeToReleaseAcceleration;
    }







    public double CalculateStoppingDistance(double currentVelocity, double currentAcceleration, double maximumAcceleration, double maximumJerk)
    {
        double time = CalculateStoppingTime(currentVelocity, currentAcceleration, maximumAcceleration, maximumJerk);

                if (double.IsPositiveInfinity(time))
        {
            return double.PositiveInfinity;
        }

        double direction = Math.Sign(currentVelocity);
        double velocity = Math.Abs(currentVelocity);

        double accelerationInTravelDirection = direction * currentAcceleration;

        StoppingProfileType profileType = DetermineProfileType(currentVelocity, currentAcceleration, maximumAcceleration, maximumJerk);

        double jerk = Math.Abs(maximumJerk);

        double maximumDeceleration = Math.Abs(maximumAcceleration);

        double timeToMaximumDeceleration = CalculateTimeToMaximumDeceleration(accelerationInTravelDirection, maximumDeceleration, jerk);

        double velocityAtMaximumDeceleration = CalculateVelocityAtMaximumDeceleration(velocity, accelerationInTravelDirection, jerk, timeToMaximumDeceleration);

        if (profileType == StoppingProfileType.ThreePhase || profileType == StoppingProfileType.FivePhase)
        {
            return CalculateTriangularStoppingDistance(velocity, accelerationInTravelDirection, jerk);
        }

        return CalculateFullStoppingDistance(velocity, accelerationInTravelDirection, jerk, maximumDeceleration, timeToMaximumDeceleration, velocityAtMaximumDeceleration);
    }

    public StoppingProfileType DetermineProfileType(double currentVelocity, double currentAcceleration, double maximumAcceleration, double maximumJerk)
    {
        double velocity = Math.Abs(currentVelocity);

        double jerk = Math.Abs(maximumJerk);

        double maximumDeceleration = Math.Abs(maximumAcceleration);

        double accelerationInTravelDirection = Math.Sign(currentVelocity) * currentAcceleration;

        double timeToMaximumDeceleration = CalculateTimeToMaximumDeceleration(accelerationInTravelDirection, maximumDeceleration, jerk);

        double velocityAtMaximumDeceleration = CalculateVelocityAtMaximumDeceleration(velocity, accelerationInTravelDirection, jerk, timeToMaximumDeceleration);

        if (velocityAtMaximumDeceleration <= 0.0)
        {
            return StoppingProfileType.ThreePhase;
        }

        double timeToReleaseAcceleration = CalculateTimeToReleaseAcceleration(maximumDeceleration, jerk);

        double velocityLostDuringRelease = CalculateVelocityLostDuringRelease(maximumDeceleration, timeToReleaseAcceleration);

        if (velocityAtMaximumDeceleration <= velocityLostDuringRelease)
        {
            return StoppingProfileType.FivePhase;
        }

        return StoppingProfileType.SevenPhase;
    }

    private static double CalculateFullStoppingDistance(double velocity, double accelerationInTravelDirection, double jerk, double maximumDeceleration, double timeToMaximumDeceleration, double velocityAtMaximumDeceleration)
    {
        double distanceDuringJerkPhase = CalculateJerkRampDistance(velocity, accelerationInTravelDirection, -jerk, timeToMaximumDeceleration);

        double timeToReleaseAcceleration = CalculateTimeToReleaseAcceleration(maximumDeceleration, jerk);

        double velocityLostDuringRelease = CalculateVelocityLostDuringRelease(maximumDeceleration, timeToReleaseAcceleration);

        if (velocityAtMaximumDeceleration <= velocityLostDuringRelease)
        {
            return
                distanceDuringJerkPhase + CalculateFivePhaseStoppingDistance(velocityAtMaximumDeceleration, jerk);
        }

        double velocityBeforeRelease = CalculateVelocityBeforeRelease(velocityAtMaximumDeceleration, velocityLostDuringRelease);

        double constantDecelerationTime = CalculateConstantDecelerationTime(velocityBeforeRelease, maximumDeceleration);

        double distanceDuringConstantDeceleration = CalculateConstantAccelerationDistance(velocityAtMaximumDeceleration, -maximumDeceleration, constantDecelerationTime);

        double velocityAtReleaseStart = CalculateVelocityAtReleaseStart(velocityAtMaximumDeceleration, maximumDeceleration, constantDecelerationTime);

        double distanceDuringRelease = CalculateJerkRampDistance(velocityAtReleaseStart, -maximumDeceleration, jerk, timeToReleaseAcceleration);

        return
            distanceDuringJerkPhase + distanceDuringConstantDeceleration + distanceDuringRelease;
    }

    private static double CalculateTriangularStoppingDistance(double velocity, double accelerationInTravelDirection, double jerk)
    {
        double root = Math.Sqrt(0.5 * accelerationInTravelDirection * accelerationInTravelDirection + jerk * velocity);

        double firstRampTime = (accelerationInTravelDirection + root) / jerk;

        double secondRampTime = root / jerk;

        double velocityAfterFirstRamp = CalculateVelocityAfterJerkRamp(velocity, accelerationInTravelDirection, -jerk, firstRampTime);

        double accelerationAfterFirstRamp = CalculateAccelerationAfterJerkRamp(accelerationInTravelDirection, -jerk, firstRampTime);

        double distanceDuringFirstRamp = CalculateJerkRampDistance(velocity, accelerationInTravelDirection, -jerk, firstRampTime);

        double distanceDuringSecondRamp = CalculateJerkRampDistance(velocityAfterFirstRamp, accelerationAfterFirstRamp, jerk, secondRampTime);

        return
            distanceDuringFirstRamp + distanceDuringSecondRamp;
    }

    private static double CalculateFivePhaseStoppingDistance(double velocityAtMaximumDeceleration, double jerk)
    {
        double releaseTime = Math.Sqrt(2.0 * velocityAtMaximumDeceleration / jerk);

        double peakDeceleration = jerk * releaseTime;

        return CalculateJerkRampDistance(velocityAtMaximumDeceleration, -peakDeceleration, jerk, releaseTime);
    }

    private static double CalculateJerkRampDistance(double initialVelocity, double initialAcceleration, double jerk, double time)
    {
        return
            initialVelocity * time + 0.5 * initialAcceleration * time * time + jerk * time * time * time / 6.0;
    }

    private static double CalculateConstantAccelerationDistance(double initialVelocity, double acceleration, double time)
    {
        return
            initialVelocity * time + 0.5 * acceleration * time * time;
    }

    private static double CalculateVelocityAfterJerkRamp(double initialVelocity, double initialAcceleration, double jerk, double time)
    {
        return
            initialVelocity + initialAcceleration * time + 0.5 * jerk * time * time;
    }

    private static double CalculateAccelerationAfterJerkRamp(double initialAcceleration, double jerk, double time)
    {
        return
            initialAcceleration + jerk * time;
    }

    private static double CalculateVelocityAtMaximumDeceleration(double velocity, double accelerationInTravelDirection, double jerk, double timeToMaximumDeceleration)
    {
        return CalculateVelocityAfterJerkRamp(velocity, accelerationInTravelDirection, -jerk, timeToMaximumDeceleration);
    }

    private static double CalculateTimeToMaximumDeceleration(double accelerationInTravelDirection, double maximumDeceleration, double jerk)
    {
        return
            (accelerationInTravelDirection + maximumDeceleration) / jerk;
    }

    private static double CalculateTimeToReleaseAcceleration(double maximumDeceleration, double jerk)
    {
        return
            maximumDeceleration / jerk;
    }

    private static double CalculateVelocityLostDuringRelease(double maximumDeceleration, double timeToReleaseAcceleration)
    {
        return
            0.5 * maximumDeceleration * timeToReleaseAcceleration;
    }

    private static double CalculateConstantDecelerationTime(double velocityBeforeRelease, double maximumDeceleration)
    {
        return Math.Max(0.0, velocityBeforeRelease / maximumDeceleration);
    }

    private static double CalculateVelocityBeforeRelease(double velocityAtMaximumDeceleration, double velocityLostDuringRelease)
    {
        return
            velocityAtMaximumDeceleration - velocityLostDuringRelease;
    }

    private static double CalculateVelocityAtReleaseStart(double velocityAtMaximumDeceleration, double maximumDeceleration, double constantDecelerationTime)
    {
        return
            velocityAtMaximumDeceleration - maximumDeceleration * constantDecelerationTime;
    }
}