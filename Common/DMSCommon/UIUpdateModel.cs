using OMSSCADACommon;
using System;
using System.Runtime.Serialization;

namespace DMSCommon
{
    /// <summary>
    /// Class describes changes on SCADA, used in the 
    /// process oh their pushing towards the client.
    /// </summary>
    [Serializable]
    [DataContract]
    public class UIUpdateModel
    {
        private long gid;
        private bool isEnergized;
        private States state;
        private CrewResponse response;
        private bool isElementAdded;
        private bool canCommand;
        private float anValue;

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

        [DataMember]
        public float AnValue { get => anValue; set => value = anValue = value; }

        public UIUpdateModel()
        {
            gid = -1;
        }

        public UIUpdateModel(long gid, bool isEnergized)
        {
            Gid = gid;
            IsEnergized = isEnergized;
        }
        public UIUpdateModel(long gid, bool isEnergized, States state)
        {
            Gid = gid;
            IsEnergized = isEnergized;
            State = state;
        }
        public UIUpdateModel(long gid, bool isEnergised, CrewResponse response)
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

        public UIUpdateModel(bool isElementAdded, long gid)
        {
            Gid = gid;
            this.isElementAdded = isElementAdded;
        }
    }
}
