using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DMSCommon.Model
{
    [DataContract]
    [KnownType(typeof(ACLine))]
    [KnownType(typeof(Switch))]
    [KnownType(typeof(Node))]
    [KnownType(typeof(Source))]
    [KnownType(typeof(Consumer))]
    [KnownType(typeof(Node))]
    public class Element
    {

        private long _elementGID;

        private bool _marker;

        private bool _underSCADA = false;

        private bool _incident;

        private string _mRID;

        [DataMember]
        public long ElementGID
        {
            get { return _elementGID; }
            set { _elementGID = value; }
        }

        [DataMember]
        public bool Marker
        {
            get { return _marker; }
            set { _marker = value; }
        }

        [DataMember]
        public bool UnderSCADA
        {
            get { return _underSCADA; }
            set { _underSCADA = value; }
        }

        [DataMember]
        public bool Incident
        {
            get { return _incident; }
            set { _incident = value; }
        }
        [DataMember]
        public string MRID
        {
            get { return _mRID; }
            set { _mRID = value; }
        }
        public Element() { }
        public Element(long gid)
        {
            ElementGID = gid;
            Marker = true;
        }

        public Element(long gid,string mrid)
        {
            ElementGID = gid;
            MRID = mrid;
            Marker = true;
        }

    }

}
