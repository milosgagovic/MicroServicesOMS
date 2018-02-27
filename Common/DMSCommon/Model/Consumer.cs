using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DMSCommon.Model
{
    [DataContract]
    public class Consumer : Branch
    {
        public Consumer() { }
        public Consumer(long gid, long end2) : base(gid)
        {
            End2 = end2;
        }
        public Consumer (long gid,string mrid):base(gid,mrid)
        {
            End2 = 0;
        }
        public Consumer(long gid, string mrid, long node) : base(gid, mrid)
        {
            End1 = node;
            End2 = 0;
        }
    }
}
