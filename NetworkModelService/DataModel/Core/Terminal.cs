using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class Terminal : IdentifiedObject
    {

        private long conductingequipment = 0;

        private long connectivitynode = 0;





        public Terminal(long globalId) :
                base(globalId)
        {
        }

        public long ConductingEquipment
        {
            get
            {
                return this.conductingequipment;
            }
            set
            {
                this.conductingequipment = value;
            }
        }


        public long ConnectivityNode
        {
            get
            {
                return this.connectivitynode;
            }
            set
            {
                this.connectivitynode = value;
            }
        }


        public override bool IsReferenced
        {
            get
            {
                return
                base.IsReferenced;
            }
        }

        public override bool Equals(object obj)
        {
            if ((true && base.Equals(obj)))
            {
                Terminal x = (Terminal)obj;
                return (
                (x.ConductingEquipment == this.ConductingEquipment) &&
                (x.ConnectivityNode == this.ConnectivityNode));

            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.TERMINAL_CONDEQUIP:
                    property.SetValue(ConductingEquipment);
                    break;
              
                case ModelCode.TERMINAL_CONNECTNODE:
                    property.SetValue(ConnectivityNode);
                    break;
                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.TERMINAL_CONDEQUIP:
                case ModelCode.TERMINAL_CONNECTNODE:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.TERMINAL_CONDEQUIP:
                    ConductingEquipment = property.AsReference();
                    break;
                case ModelCode.TERMINAL_CONNECTNODE:
                    ConnectivityNode = property.AsReference();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (ConductingEquipment != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_CONDEQUIP] = new List<long>();
                references[ModelCode.TERMINAL_CONDEQUIP].Add(ConductingEquipment);
            }
          
            if (ConnectivityNode != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_CONNECTNODE] = new List<long>();
                references[ModelCode.TERMINAL_CONNECTNODE].Add(ConnectivityNode);
            }
            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
              
                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }
    }
}
