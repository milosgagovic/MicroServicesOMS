using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class EnergyConsumer : ConductingEquipment
    {

        /// Active power of the load that is a fixed quantity. Load sign convention is used, i.e. positive sign means flow out from a node.
        private float pfixed;

        /// Reactive power of the load that is a fixed quantity. Load sign convention is used, i.e. positive sign means flow out from a node.
        private float qfixed;


        public EnergyConsumer(long globalId) :
                base(globalId)
        {
        }

        public float Pfixed
        {
            get
            {
                return this.pfixed;
            }
            set
            {
                this.pfixed = value;
            }
        }

        public float Qfixed
        {
            get
            {
                return this.qfixed;
            }
            set
            {
                this.qfixed = value;
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
                EnergyConsumer x = (EnergyConsumer)obj;
                return (
                (x.pfixed == this.pfixed) &&
                (x.qfixed == this.qfixed));

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
                case ModelCode.ENERGCONSUMER_PFIXED:
                    property.SetValue(pfixed);
                    break;
                case ModelCode.ENERGCONSUMER_QFIXED:
                    property.SetValue(qfixed);
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
                case ModelCode.ENERGCONSUMER_PFIXED:
                case ModelCode.ENERGCONSUMER_QFIXED:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ENERGCONSUMER_PFIXED:
                    pfixed = property.AsFloat();
                        break;
                case ModelCode.ENERGCONSUMER_QFIXED:
                    qfixed = property.AsFloat();
                        break;
                default:
                    base.SetProperty(property);
                    break;
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
