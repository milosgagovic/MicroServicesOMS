using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DMSCommon.Model
{
    [DataContract]
    public class Source : Branch
    {
        public Source() { }
        public Source(long gid, long end1,string mrid) : base(gid,mrid)
        {
            End1 = end1;
        }

        public Source(long gid, long end1, long end2, string mrid) : base(gid, mrid)
        {
            End1 = end1;
            End2 = end2;
        }
    }
}
