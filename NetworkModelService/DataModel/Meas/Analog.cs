using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Meas
{
    public class Analog : Measurement
    {

        /// Normal value range maximum for any of the MeasurementValue.values. Used for scaling, e.g. in bar graphs or of telemetered raw values.
        private float maxValue;

        /// Normal value range minimum for any of the MeasurementValue.values. Used for scaling, e.g. in bar graphs or of telemetered raw values.
        private float minValue;

        /// Normal measurement value, e.g., used for percentage calculations.
        private float normalValue;

        public Analog(long globalId) :
                base(globalId)
        {
        }

        public float MaxValue
        {
            get
            {
                return this.maxValue;
            }
            set
            {
                this. maxValue = value;
            }
        }

        public float MinValue
        {
            get
            {
                return this.minValue;
            }
            set
            {
                this.minValue = value;
            }
        }

        public float NormalValue
        {
            get
            {
                return this.normalValue;
            }
            set
            {
                this.normalValue = value;
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
                Analog x = (Analog)obj;
                return (
                (x.maxValue == this.maxValue) &&
                (x.minValue == this.minValue) &&
                (x.normalValue == this.normalValue));
               
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
                case ModelCode.ANALOG_MAXVAL:
                    property.SetValue(maxValue);
                    break;
                case ModelCode.ANALOG_MINVAL:
                    property.SetValue(minValue);
                    break;
                case ModelCode.ANALOG_NORMVAL:
                    property.SetValue(normalValue);
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
                case ModelCode.ANALOG_MAXVAL:
                case ModelCode.ANALOG_MINVAL:
                case ModelCode.ANALOG_NORMVAL:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ANALOG_MAXVAL:
                    maxValue = property.AsFloat();
                    break;
                case ModelCode.ANALOG_MINVAL:
                    minValue = property.AsFloat();
                    break;
                case ModelCode.ANALOG_NORMVAL:
                    normalValue = property.AsFloat();
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
