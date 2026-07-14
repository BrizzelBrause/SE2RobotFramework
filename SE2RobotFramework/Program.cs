using SE2RobotFramework.Controllers;
using SE2RobotFramework.Core;
using SE2RobotFramework.Hardware;
using SE2RobotFramework.Motion;

RotationalAxis axis = new RotationalAxis
{
    Name = "Solar.Azimuth",
    MovementSpeed = 5.0,
    MaximumAcceleration = 1.0,
    Tolerance = 0.5,
    WrapAround = true
};

axis.SetTargetPosition(90.0);

FakeAxisHardware hardware = new FakeAxisHardware
{
    MaximumAcceleration = 1.0
};

hardware.SetPosition(0.0);

TrapezoidalMotionProfile profile = new TrapezoidalMotionProfile();

MotionRequestFactory requestFactory = new MotionRequestFactory();

MotionController controller = new MotionController(axis, hardware, profile, requestFactory);

double deltaTime = 0.1;
double simulationTime = 30.0;

int steps = (int)(simulationTime / deltaTime);

for (int step = 0; step < steps; step++)
{
    controller.Update(deltaTime);

    double time = step * deltaTime;

    if (step % 10 == 0)
    {
        Console.WriteLine(
            $"Zeit: {time,4:0.0}s | " +
            $"Position: {axis.CurrentPosition,5:0.0}° | " +
            $"Fehler: {axis.GetError(),5:0.0}° | " +
            $"Ist-v: {hardware.GetVelocity(),5:0.00}°/s | " +
            $"Soll-v: {hardware.CommandedVelocity,5:0.00}°/s");
    }

    hardware.Simulate(deltaTime);
}

double finalError = Math.Abs(axis.GetError());

if (finalError <= axis.Tolerance)
{
    Console.WriteLine("TEST BESTANDEN");
}
else
{
    Console.WriteLine(
        $"TEST FEHLGESCHLAGEN: Restfehler = {finalError:0.000}°");
}