using OMSSCADACommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DMSCommon.Model
{
    /// <summary>
    /// Class describes changes on SCADA, used in the 
    /// process oh their pushing towards the client.
    /// </summary>
    [Serializable]
    [DataContract]
    public class SCADAUpdateModel
    {
        private long gid;
        private bool isEnergized;
        private States state;
        private CrewResponse response;
        private bool isElementAdded;
        private bool canCommand;

        [DataMember]
        public long Gid
        {
            get { return gid; }
            set { gid = value; }
        }

        [DataMember]
        public bool IsEnergized
        {
            get { return isEnergized; }
            set { isEnergized = value; }
        }

        [DataMember]
        public bool CanCommand
        {
            get { return canCommand; }
            set { canCommand = value; }
        }

        [DataMember]
        public States State
        {
            get { return state; }
            set { state = value; }
        }
        [DataMember]
        public CrewResponse Response { get => response; set => response = value; }
        [DataMember]
        public bool IsElementAdded { get => isElementAdded; set => isElementAdded = value; }

        public SCADAUpdateModel()
        {
            gid = -1;
        }

        public SCADAUpdateModel(long gid, bool isEnergized)
        {
            Gid = gid;
            IsEnergized = isEnergized;
        }
        public SCADAUpdateModel(long gid, bool isEnergized, States state)
        {
            Gid = gid;
            IsEnergized = isEnergized;
            State = state;
        }
        public SCADAUpdateModel(long gid, bool isEnergised, CrewResponse response)
        {
            Gid = gid;
            IsEnergized = isEnergised;
            Response = response;

            if (isEnergized)
                State = States.CLOSED;
            else
            {
                State = States.OPENED;
            }
            isElementAdded = false;
        }

        public SCADAUpdateModel(bool isElementAdded, long gid)
        {
            Gid = gid;
            this.isElementAdded = isElementAdded;
        }
    }
}
