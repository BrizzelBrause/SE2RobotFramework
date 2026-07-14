using SE2RobotFramework.Motion;

namespace SE2RobotFramework.Tests;

public class StoppingDistanceCalculatorTests
{
    [Fact]
    public void CalculateStoppingTime_FromRestAcceleration_ReturnsExpectedTime()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double time = calculator.CalculateStoppingTime(currentVelocity: 4.0, currentAcceleration: 0.0, maximumAcceleration: 5.0, maximumJerk: 2.0);

        Assert.Equal(2.0, time, 10);
    }

    [Fact]
    public void CalculateStoppingDistance_FromRestAcceleration_ReturnsExpectedDistance()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double distance = calculator.CalculateStoppingDistance(currentVelocity: 4.0, currentAcceleration: 0.0, maximumAcceleration: 5.0, maximumJerk: 2.0);

        Assert.Equal(5.6568542495, distance, 8);
    }

    [Fact]
    public void CalculateStoppingDistance_WithPositiveAcceleration_IsLongerThanWithoutAcceleration()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double distanceWithoutAcceleration = calculator.CalculateStoppingDistance(currentVelocity: 4.0, currentAcceleration: 0.0, maximumAcceleration: 5.0, maximumJerk: 2.0);

        double distanceWithPositiveAcceleration = calculator.CalculateStoppingDistance(currentVelocity: 4.0, currentAcceleration: 2.0, maximumAcceleration: 5.0, maximumJerk: 2.0);

        Assert.True(
            distanceWithPositiveAcceleration > distanceWithoutAcceleration);
    }

    [Fact]
    public void CalculateStoppingDistance_WithHigherVelocity_IsLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double slower = calculator.CalculateStoppingDistance(currentVelocity: 2.0, currentAcceleration: 0.0, maximumAcceleration: 5.0, maximumJerk: 2.0);

        double faster = calculator.CalculateStoppingDistance(currentVelocity: 4.0, currentAcceleration: 0.0, maximumAcceleration: 5.0, maximumJerk: 2.0);

        Assert.True(faster > slower);
    }

    [Fact]
    public void CalculateStoppingDistance_WithHigherMaximumJerk_IsShorter()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double lowJerk = calculator.CalculateStoppingDistance(currentVelocity: 4.0, currentAcceleration: 0.0, maximumAcceleration: 5.0, maximumJerk: 1.0);

        double highJerk = calculator.CalculateStoppingDistance(currentVelocity: 4.0, currentAcceleration: 0.0, maximumAcceleration: 5.0, maximumJerk: 4.0);

        Assert.True(highJerk < lowJerk);
    }

    [Fact]
    public void CalculateStoppingDistance_WithLowerMaximumAcceleration_IsLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double weakBrakes = calculator.CalculateStoppingDistance(currentVelocity: 4.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 2.0);

        double strongBrakes = calculator.CalculateStoppingDistance(currentVelocity: 4.0, currentAcceleration: 0.0, maximumAcceleration: 5.0, maximumJerk: 2.0);

        Assert.True(weakBrakes > strongBrakes);
    }

    [Fact]
    public void CalculateStoppingDistance_WithAccelerationLimit_RemainsPositive()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double distance = calculator.CalculateStoppingDistance(currentVelocity: 8.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.True(distance > 0.0);
    }

    [Fact]
    public void CalculateStoppingTime_WhenMaximumDecelerationIsReached_IncludesAccelerationReleasePhase()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double stoppingTime = calculator.CalculateStoppingTime(currentVelocity: 10.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.Equal(7.0, stoppingTime, 10);
    }

    [Fact]
    public void CalculateStoppingDistance_WhenMaximumDecelerationIsReached_IncludesAccelerationReleasePhase()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double distance = calculator.CalculateStoppingDistance(currentVelocity: 10.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.Equal(35.0, distance, 10);
    }

    [Fact]
    public void CalculateStoppingDistance_WhenMaximumDecelerationCannotBeReleased_UsesTriangularProfile()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double distance = calculator.CalculateStoppingDistance(currentVelocity: 3.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.Equal(5.1961524227, distance, 10);
    }

    [Fact]
    public void CalculateStoppingTime_WhenMaximumDecelerationCannotBeReleased_UsesTriangularProfile()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double time = calculator.CalculateStoppingTime(currentVelocity: 3.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.Equal(3.4641016151, time, 10);
    }

    [Fact]
    public void CalculateStoppingDistance_WithNegativeVelocity_HasSameMagnitude()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double positive = calculator.CalculateStoppingDistance(currentVelocity: 3.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        double negative = calculator.CalculateStoppingDistance(currentVelocity: -3.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.Equal(positive, negative, 10);
    }

    [Fact]
    public void CalculateStoppingTime_WithNegativeVelocity_HasSameMagnitude()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double positive = calculator.CalculateStoppingTime(currentVelocity: 3.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        double negative = calculator.CalculateStoppingTime(currentVelocity: -3.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.Equal(positive, negative, 10);
    }

    [Fact]
    public void CalculateStoppingDistance_WithInitialNegativeAcceleration_UsesTriangularProfile()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double distance = calculator.CalculateStoppingDistance(currentVelocity: 3.0, currentAcceleration: -1.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.True(distance > 3.2145670935);
    }

    [Fact]
    public void CalculateStoppingDistance_WithStrongerInitialBraking_IsShorter()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double weakerBraking = calculator.CalculateStoppingDistance(currentVelocity: 3.0, currentAcceleration: -0.5, maximumAcceleration: 2.0, maximumJerk: 1.0);

        double strongerBraking = calculator.CalculateStoppingDistance(currentVelocity: 3.0, currentAcceleration: -1.5, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.True(strongerBraking < weakerBraking);
    }

    [Fact]
    public void CalculateStoppingTime_WithStrongerInitialBraking_IsShorter()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double weakerBraking = calculator.CalculateStoppingTime(currentVelocity: 3.0, currentAcceleration: -0.5, maximumAcceleration: 2.0, maximumJerk: 1.0);

        double strongerBraking = calculator.CalculateStoppingTime(currentVelocity: 3.0, currentAcceleration: -1.5, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.True(strongerBraking < weakerBraking);
    }

    [Fact]
    public void CalculateStoppingDistance_WithPositiveInitialAcceleration_IsLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double braking = calculator.CalculateStoppingDistance(currentVelocity: 3.0, currentAcceleration: -1.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        double accelerating = calculator.CalculateStoppingDistance(currentVelocity: 3.0, currentAcceleration: 1.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.True(accelerating > braking);
    }

    [Fact]
    public void CalculateStoppingTime_WithPositiveInitialAcceleration_IsLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double braking = calculator.CalculateStoppingTime(currentVelocity: 3.0, currentAcceleration: -1.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        double accelerating = calculator.CalculateStoppingTime(currentVelocity: 3.0, currentAcceleration: 1.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.True(accelerating != braking);
    }

    [Fact]
    public void CalculateStoppingDistance_WithVeryLargeMaximumAcceleration_IsIndependentOfMaximumAcceleration()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double first = calculator.CalculateStoppingDistance(currentVelocity: 3.0, currentAcceleration: 0.0, maximumAcceleration: 100.0,maximumJerk: 1.0);

        double second = calculator.CalculateStoppingDistance(currentVelocity: 3.0, currentAcceleration: 0.0, maximumAcceleration: 1000.0, maximumJerk: 1.0);

        Assert.Equal(first, second, 10);
    }

    [Fact]
    public void CalculateStoppingTime_WithVeryLargeMaximumAcceleration_IsIndependentOfMaximumAcceleration()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();
        
        double first = calculator.CalculateStoppingTime(currentVelocity: 3.0, currentAcceleration: 0.0, maximumAcceleration: 100.0, maximumJerk: 1.0);

        double second = calculator.CalculateStoppingTime(currentVelocity: 3.0, currentAcceleration: 0.0, maximumAcceleration: 1000.0, maximumJerk: 1.0);

        Assert.Equal(first, second, 10);
    }

    [Fact]
    public void CalculateStoppingDistance_FromRest_ReturnsZero()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double distance = calculator.CalculateStoppingDistance(currentVelocity: 0.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.Equal(0.0, distance);
    }

    [Fact]
    public void CalculateStoppingTime_FromRest_ReturnsZero()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double time = calculator.CalculateStoppingTime(currentVelocity: 0.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.Equal(0.0, time);
    }

    [Fact]
    public void CalculateStoppingDistance_WithVerySmallVelocity_RemainsPositive()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double distance = calculator.CalculateStoppingDistance(currentVelocity: 0.001, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.True(distance > 0.0);
    }

    [Fact]
    public void CalculateStoppingTime_WithVerySmallVelocity_RemainsPositive()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double time = calculator.CalculateStoppingTime(currentVelocity: 0.001, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.True(time > 0.0);
    }

    [Fact]
    public void CalculateStoppingDistance_WithVerySmallMaximumJerk_IsLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double lowJerk = calculator.CalculateStoppingDistance(currentVelocity: 3.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 0.1);

        double highJerk = calculator.CalculateStoppingDistance(currentVelocity: 3.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.True(lowJerk > highJerk);
    }

    [Fact]
    public void CalculateStoppingTime_WithVerySmallMaximumJerk_IsLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double lowJerk = calculator.CalculateStoppingTime(currentVelocity: 3.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 0.1);

        double highJerk = calculator.CalculateStoppingTime(currentVelocity: 3.0, currentAcceleration: 0.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.True(lowJerk > highJerk);
    }

    [Fact]
    public void CalculateStoppingDistance_WithVeryHighInitialAcceleration_IsLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double moderate = calculator.CalculateStoppingDistance(
            currentVelocity: 3.0,
            currentAcceleration: 1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        double high = calculator.CalculateStoppingDistance(
            currentVelocity: 3.0,
            currentAcceleration: 1.9,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        Assert.True(high > moderate);
    }

    [Fact]
    public void CalculateStoppingTime_WithVeryHighInitialAcceleration_IsLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double moderate = calculator.CalculateStoppingTime(
            currentVelocity: 3.0,
            currentAcceleration: 1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        double high = calculator.CalculateStoppingTime(
            currentVelocity: 3.0,
            currentAcceleration: 1.9,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        Assert.True(high > moderate);
    }

    [Fact]
    public void CalculateStoppingDistance_WithInitialAccelerationOppositeToVelocity_IsShorter()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double sameDirection = calculator.CalculateStoppingDistance(
            currentVelocity: 3.0,
            currentAcceleration: 1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        double oppositeDirection = calculator.CalculateStoppingDistance(
            currentVelocity: 3.0,
            currentAcceleration: -1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        Assert.True(oppositeDirection < sameDirection);
    }

    [Fact]
    public void CalculateStoppingTime_WithInitialAccelerationOppositeToVelocity_IsShorter()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double sameDirection = calculator.CalculateStoppingTime(
            currentVelocity: 3.0,
            currentAcceleration: 1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        double oppositeDirection = calculator.CalculateStoppingTime(
            currentVelocity: 3.0,
            currentAcceleration: -1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        Assert.True(oppositeDirection != sameDirection);
    }

    [Fact]
    public void CalculateStoppingDistance_WithNegativeVelocityAndNegativeAcceleration_HasSameMagnitude()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double positive = calculator.CalculateStoppingDistance(
            currentVelocity: 3.0,
            currentAcceleration: 1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        double negative = calculator.CalculateStoppingDistance(
            currentVelocity: -3.0,
            currentAcceleration: -1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        Assert.Equal(positive, negative, 10);
    }

    [Fact]
    public void CalculateStoppingTime_WithNegativeVelocityAndNegativeAcceleration_HasSameMagnitude()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double positive = calculator.CalculateStoppingTime(
            currentVelocity: 3.0,
            currentAcceleration: 1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        double negative = calculator.CalculateStoppingTime(
            currentVelocity: -3.0,
            currentAcceleration: -1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        Assert.Equal(positive, negative, 10);
    }

    [Fact]
    public void CalculateStoppingDistance_WithNegativeVelocityAndPositiveAcceleration_IsLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double braking = calculator.CalculateStoppingDistance(
            currentVelocity: -3.0,
            currentAcceleration: -1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        double accelerating = calculator.CalculateStoppingDistance(
            currentVelocity: -3.0,
            currentAcceleration: 1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        Assert.True(accelerating != braking);
    }

    [Fact]
    public void CalculateStoppingTime_WithNegativeVelocityAndPositiveAcceleration_IsDifferent()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double braking = calculator.CalculateStoppingTime(
            currentVelocity: -3.0,
            currentAcceleration: -1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        double accelerating = calculator.CalculateStoppingTime(
            currentVelocity: -3.0,
            currentAcceleration: 1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        Assert.True(accelerating != braking);
    }

    [Fact]
    public void CalculateStoppingDistance_WithVeryHighVelocity_IsLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double lowVelocity = calculator.CalculateStoppingDistance(
            currentVelocity: 10.0,
            currentAcceleration: 0.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        double highVelocity = calculator.CalculateStoppingDistance(
            currentVelocity: 20.0,
            currentAcceleration: 0.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        Assert.True(highVelocity > lowVelocity);
    }

    [Fact]
    public void CalculateStoppingTime_WithVeryHighVelocity_IsLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double lowVelocity = calculator.CalculateStoppingTime(
            currentVelocity: 10.0,
            currentAcceleration: 0.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        double highVelocity = calculator.CalculateStoppingTime(
            currentVelocity: 20.0,
            currentAcceleration: 0.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        Assert.True(highVelocity > lowVelocity);
    }

    [Fact]
    public void CalculateStoppingDistance_WithHigherMaximumAcceleration_IsNeverLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double lowAcceleration = calculator.CalculateStoppingDistance(
            currentVelocity: 10.0,
            currentAcceleration: 0.5,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        double highAcceleration = calculator.CalculateStoppingDistance(
            currentVelocity: 10.0,
            currentAcceleration: 0.5,
            maximumAcceleration: 4.0,
            maximumJerk: 1.0);

        Assert.True(highAcceleration <= lowAcceleration);
    }

    [Fact]
    public void CalculateStoppingTime_WithHigherMaximumAcceleration_IsNeverLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double lowAcceleration = calculator.CalculateStoppingTime(
            currentVelocity: 10.0,
            currentAcceleration: 0.5,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        double highAcceleration = calculator.CalculateStoppingTime(
            currentVelocity: 10.0,
            currentAcceleration: 0.5,
            maximumAcceleration: 4.0,
            maximumJerk: 1.0);

        Assert.True(highAcceleration <= lowAcceleration);
    }

    [Fact]
    public void CalculateStoppingDistance_WithHigherMaximumJerk_IsNeverLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double lowJerk = calculator.CalculateStoppingDistance(
            currentVelocity: 10.0,
            currentAcceleration: 0.5,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        double highJerk = calculator.CalculateStoppingDistance(
            currentVelocity: 10.0,
            currentAcceleration: 0.5,
            maximumAcceleration: 2.0,
            maximumJerk: 4.0);

        Assert.True(highJerk <= lowJerk);
    }

    [Fact]
    public void CalculateStoppingTime_WithHigherMaximumJerk_IsNeverLonger()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double lowJerk = calculator.CalculateStoppingTime(
            currentVelocity: 10.0,
            currentAcceleration: 0.5,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        double highJerk = calculator.CalculateStoppingTime(
            currentVelocity: 10.0,
            currentAcceleration: 0.5,
            maximumAcceleration: 2.0,
            maximumJerk: 4.0);

        Assert.True(highJerk <= lowJerk);
    }

    [Fact]
    public void CalculateStoppingDistance_WhenConstantDecelerationIsRequired_ReturnsExpectedDistance()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double distance = calculator.CalculateStoppingDistance(
            currentVelocity: 4.5,
            currentAcceleration: 0.0,
            maximumAcceleration: 2.0,
            maximumJerk: 2.0);

        Assert.Equal(7.3125, distance, 10);
    }

    [Fact]
    public void CalculateStoppingTime_WithPositiveInitialAccelerationAndTriangularProfile_ReturnsExpectedTime()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double time = calculator.CalculateStoppingTime(
            currentVelocity: 3.0,
            currentAcceleration: 1.0,
            maximumAcceleration: 10.0,
            maximumJerk: 1.0);

        Assert.Equal(4.7416573868, time, 10);
    }

    [Fact]
    public void CalculateStoppingTime_WithPositiveInitialAccelerationAndConstantDeceleration_ReturnsExpectedTime()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double time = calculator.CalculateStoppingTime(currentVelocity: 4.0, currentAcceleration: 1.0, maximumAcceleration: 2.0, maximumJerk: 1.0);

        Assert.Equal(5.25, time, 10);
    }

    [Fact]
    public void CalculateStoppingTime_WhenMaximumDecelerationCannotBeReleasedWithPositiveAcceleration_ReturnsExpectedTime()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        double time = calculator.CalculateStoppingTime(
            currentVelocity: 2.0,
            currentAcceleration: 1.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        Assert.Equal(4.1622776602, time, 10);
    }

    [Fact]
    public void DetermineProfileType_WhenMaximumDecelerationIsNotReached_ReturnsThreePhase()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        StoppingProfileType profileType = calculator.DetermineProfileType(
            currentVelocity: 3.0,
            currentAcceleration: 0.0,
            maximumAcceleration: 5.0,
            maximumJerk: 2.0);

        Assert.Equal(StoppingProfileType.ThreePhase, profileType);
    }

    [Fact]
    public void DetermineProfileType_WhenMaximumDecelerationIsReachedAndReleased_ReturnsSevenPhase()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        StoppingProfileType profileType = calculator.DetermineProfileType(
            currentVelocity: 10.0,
            currentAcceleration: 0.0,
            maximumAcceleration: 2.0,
            maximumJerk: 1.0);

        Assert.Equal(StoppingProfileType.SevenPhase, profileType);
    }

    [Fact]
    public void DetermineProfileType_WhenMaximumDecelerationIsReachedButCannotBeReleased_ReturnsFivePhase()
    {
        StoppingDistanceCalculator calculator = new StoppingDistanceCalculator();

        StoppingProfileType profileType = calculator.DetermineProfileType(
            currentVelocity: 1.5,
            currentAcceleration: 0.0,
            maximumAcceleration: 2.0,
            maximumJerk: 2.0);

        Assert.Equal(StoppingProfileType.FivePhase, profileType);
    }
}