using System;
using System.Collections.Generic;
using System.Text;

namespace SE2RobotFramework.Hardware;

public class FakeAxisHardware : IAxisHardware
{
    public double Position { get; private set; }

    public double Velocity { get; private set; }

    public double CommandedVelocity { get; private set; }

    public double MaximumAcceleration { get; set; } = 1.0;

    public bool IsAvailable { get; set; } = true;

    public double GetPosition()
    {
        return Position;
    }

    public double GetVelocity()
    {
        return Velocity;
    }

    public void SetVelocity(double velocity)
    {
        if (!CanExecuteCommand())
        {
            return;
        }

        CommandedVelocity = velocity;
    }

    public void SetPosition(double position)
    {
        Position = position;
    }

    public void Stop()
    {
        CommandedVelocity = 0.0;
    }

    public bool CanExecuteCommand()
    {
        return IsAvailable;
    }

    public void Simulate(double deltaTime)
    {
        UpdateVelocity(deltaTime);

        Position += Velocity * deltaTime;
    }

    private void UpdateVelocity(double deltaTime)
    {
        double velocityError = CommandedVelocity - Velocity;

        double maximumVelocityChange =
            MaximumAcceleration * deltaTime;

        if (Math.Abs(velocityError) <= maximumVelocityChange)
        {
            Velocity = CommandedVelocity;
            return;
        }

        int direction = Math.Sign(velocityError);

        Velocity += direction * maximumVelocityChange;
    }
}