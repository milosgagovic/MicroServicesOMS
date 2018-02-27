using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DMSCommon.Model
{
    [DataContract]
    public class ACLine : Branch
    {
        private float _length;
        [DataMember]
        public float Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public ACLine() { }
        public ACLine(float length)
        {
            this.Length = length;
        }
        public ACLine(long gid) : base(gid)
        {

        }
        public ACLine(long gid, string mrid) : base(gid, mrid)
        {

        }


    }
}
