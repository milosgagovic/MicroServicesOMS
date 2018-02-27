using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace OMSSCADACommon
{
    public enum DeltaOpType : byte
    {
        Insert = 0,
        Update = 1,
        Delete = 2
    }

    [Serializable]
    [DataContract]
    public class ScadaDelta
    {

        private List<ScadaElement> insertOps = new List<ScadaElement>();
        private List<ScadaElement> updateOps = new List<ScadaElement>();

        public ScadaDelta()
        {

        }

        [DataMember]
        public List<ScadaElement> InsertOps
        {
            get { return insertOps; }
            set { insertOps = value; }
        }

        [DataMember]
        public List<ScadaElement> UpdateOps
        {
            get { return updateOps; }
            set { updateOps = value; }
        }


        public void AddScadaDeltaOperation(DeltaOpType type, ScadaElement element, bool addAtEnd)
        {
            List<ScadaElement> operations = null;
            switch (type)
            {
                case DeltaOpType.Insert:
                    operations = insertOps;
                    break;
                case DeltaOpType.Update:
                    operations = updateOps;
                    break;
            }

            if (addAtEnd)
            {
                operations.Add(element);
            }
            else
            {
                operations.Insert(0, element);
            }
        }
    }
}
