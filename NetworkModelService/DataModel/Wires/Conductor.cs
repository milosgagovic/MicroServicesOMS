using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class Conductor : ConductingEquipment
    {

        /// Segment length for calculating line section capabilities
        private float length;



        public Conductor(long globalId) :
                base(globalId)
        {
        }

        public float Length
        {
            get
            {
                return this.length;
            }
            set
            {
                this.length = value;
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
                Conductor x = (Conductor)obj;
                return (
                (x.length == this.length));
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
                case ModelCode.CONDUCTOR_LEN:
                    property.SetValue(length);
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
                case ModelCode.CONDUCTOR_LEN:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.CONDUCTOR_LEN:
                    length = property.AsFloat();
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
