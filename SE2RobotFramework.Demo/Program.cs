using SE2RobotFramework.Controllers;
using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Motion;

RotationalAxis axis = new()
{
    Name = "Solar.Azimuth",
    MotionLimits = new MotionLimits
    {
        MaximumSpeed = 5.0,
        MaximumAcceleration = 1.0
    },
    MotionProfileType = MotionProfileType.Trapezoidal,
    Tolerance = 0.5,
    WrapAround = true
};

axis.SetTargetPosition(90.0);

FakeAxisHardware hardware = new()
{
    MaximumAcceleration = 1.0
};
hardware.SetPosition(0.0);

MotionController controller = new(
    axis,
    hardware,
    new MotionProfileFactory(),
    new MotionRequestFactory());

const double deltaTime = 0.1;
const double simulationTime = 30.0;
int steps = (int)(simulationTime / deltaTime);

for (int step = 0; step < steps; step++)
{
    controller.Update(deltaTime);

    if (step % 10 == 0)
    {
        double time = step * deltaTime;
        Console.WriteLine(
            $"Time: {time,4:0.0}s | " +
            $"Position: {axis.CurrentPosition,5:0.0} deg | " +
            $"Error: {axis.GetError(),5:0.0} deg | " +
            $"Velocity: {hardware.GetVelocity(),5:0.00} deg/s | " +
            $"Command: {hardware.CommandedVelocity,5:0.00} deg/s");
    }

    hardware.Simulate(deltaTime);
}

controller.Update(deltaTime);

Console.WriteLine(axis.IsAtTarget()
    ? "Simulation completed successfully."
    : $"Simulation stopped with an error of {Math.Abs(axis.GetError()):0.000} deg.");
