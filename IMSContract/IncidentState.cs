using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSContract
{
    public enum IncidentState
    {
        UNRESOLVED,
        INVESTIGATING,
        READY_FOR_REPAIR,
        REPAIRING,
        REPAIRED,
        FAILED_TO_REPAIR,
        NO_FREE_CREWS
    }
}
