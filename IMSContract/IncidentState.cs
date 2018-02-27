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
        PENDING,
        REPAIRED,
        FAILED_TO_REPAIR,
        NO_CREWS
    }
}
