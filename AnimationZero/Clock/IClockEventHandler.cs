using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationZero.Clock
{
    public interface IClockEventHandler
    {
        bool ClockTicked(int clockCount);
        void Kill();
    }
}
