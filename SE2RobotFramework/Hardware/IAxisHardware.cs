using System;
using System.Collections.Generic;
using System.Text;

namespace SE2RobotFramework.Hardware
{
    public interface IAxisHardware
    {
        double GetPosition();

        double GetVelocity();

        void SetVelocity(double velocity);

        void Stop();

        bool CanExecuteCommand();
    }
}
