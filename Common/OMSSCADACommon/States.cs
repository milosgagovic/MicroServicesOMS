using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSSCADACommon
{
    public enum States
    {
        // ako je 0 na ulazu to je closed
        CLOSED = 0,
        OPENED,
        UNKNOWN
    }

    public enum CrewResponse
    {
        ShortCircuit = 0,
        GroundFault,
        Overload,
    }
}
