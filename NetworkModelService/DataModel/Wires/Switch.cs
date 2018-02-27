using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class Switch : ConductingEquipment
    {

        /// The attribute is used in cases when no Measurement for the status value is present. If the Switch has a status measurment the Discrete.normalValue is expected to match with the Switch.normalOpen.
        private bool normalopen;

        /// The switch on count since the switch was last reset or initialized.
        private int switchoncount;

        /// The date and time when the switch was last switched on.
        private DateTime switchondate;


        public Switch(long globalId) :
                base(globalId)
        {
        }

        public bool NormalOpen
        {
            get
            {
                return this.normalopen;
            }
            set
            {
                this.normalopen = value;
            }
        }

        public int SwitchonCount
        {
            get
            {
                return this.switchoncount;
            }
            set
            {
                this.switchoncount = value;
            }
        }

        public DateTime SwitchOnDate
        {
            get
            {
                return this.switchondate;
            }
            set
            {
                this.switchondate = value;
            }
        }

      

       

        public override bool Equals(object obj)
        {
            if ((true && base.Equals(obj)))
            {
                Switch x = (Switch)obj;
                return (
                (x.NormalOpen == this.NormalOpen) &&
                (x.SwitchonCount == this.SwitchonCount) &&
                (x.SwitchOnDate == this.SwitchOnDate));
               
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
                case ModelCode.SWITCH_NORMOPEN:
                    property.SetValue(NormalOpen);
                    break;
                case ModelCode.SWITCH_ONCOUNT:
                    property.SetValue(SwitchonCount);
                    break;
                case ModelCode.SWITCH_ONDATE:
                    property.SetValue(SwitchOnDate);
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
                case ModelCode.SWITCH_NORMOPEN:
                case ModelCode.SWITCH_ONCOUNT:
                case ModelCode.SWITCH_ONDATE:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SWITCH_NORMOPEN:
                    normalopen = property.AsBool();
                    break;
                case ModelCode.SWITCH_ONCOUNT:
                    switchoncount = property.AsInt();
                    break;
                case ModelCode.SWITCH_ONDATE:
                    switchondate = property.AsDateTime();
                    break;
                default:
                    base.SetProperty(property);
                    break;
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

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
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
