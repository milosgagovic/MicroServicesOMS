using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class EnergySource : ConductingEquipment
    {

        /// High voltage source load.
        private float activepower;

        /// Phase-to-phase nominal voltage.
        private float nominalvoltage;


        public EnergySource(long globalId) :
                base(globalId)
        {
        }

        public float ActivePower
        {
            get
            {
                return this.activepower;
            }
            set
            {
                this.activepower = value;
            }
        }

        public float NominalVoltage
        {
            get
            {
                return this.nominalvoltage;
            }
            set
            {
                this.nominalvoltage = value;
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
                EnergySource x = (EnergySource)obj;
                return (
                (x.ActivePower == this.ActivePower) &&
                (x.NominalVoltage == this.NominalVoltage));
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
                case ModelCode.ENERGSOURCE_ACTPOW:
                    property.SetValue(activepower);
                    break;
                case ModelCode.ENERGSOURCE_NOMVOLT:
                    property.SetValue(nominalvoltage);
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
                case ModelCode.ENERGSOURCE_ACTPOW:
                case ModelCode.ENERGSOURCE_NOMVOLT:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ENERGSOURCE_ACTPOW:
                    activepower = property.AsFloat();

                        break;
                case ModelCode.ENERGSOURCE_NOMVOLT:
                    nominalvoltage = property.AsFloat();

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
