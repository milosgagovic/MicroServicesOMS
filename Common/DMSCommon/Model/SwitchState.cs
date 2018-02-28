using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DMSCommon.Model
{
    [DataContract]
    public enum SwitchState
    {
        Closed = 0,
        Open = 1
    }
}
