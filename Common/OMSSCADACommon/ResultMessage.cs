using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSSCADACommon
{
    public enum ResultMessage
    {
        OK = 0,
        ID_NOT_SET,
        INVALID_ID,
        INVALID_DIG_COMM,
        INVALID_VALUE,
        INTERNAL_SERVER_ERROR
    }
}
