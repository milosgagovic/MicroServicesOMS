using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Meas
{
    public class Discrete : Measurement
    {

        /// Normal value range maximum for any of the MeasurementValue.values. Used for scaling, e.g. in bar graphs or of telemetered raw values.
        private int maxValue;

        /// Normal value range minimum for any of the MeasurementValue.values. Used for scaling, e.g. in bar graphs or of telemetered raw values.
        private int minValue;

        /// Normal measurement value, e.g., used for percentage calculations.
        private int normalValue;

        private List<Commands> validCommands;

        private List<States> validStates;

        public Discrete(long globalId) :
                base(globalId)
        {
        }

        public int MaxValue
        {
            get
            {
                return this.maxValue;
            }
            set
            {
                this.maxValue = value;
            }
        }

        public int MinValue
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

        public int NormalValue
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

        public List<Commands> ValidCommands
        {
            get
            {
                return this.validCommands;
            }
            set
            {
                this.validCommands = value;
            }
        }

        public List<States> ValidStates
        {
            get
            {
                return this.validStates;
            }
            set
            {
                this.validStates = value;
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
                Discrete x = (Discrete)obj;
                return (
                (x.maxValue == this.maxValue) &&
                (x.minValue == this.minValue) &&
                (x.normalValue == this.normalValue) &&
                (x.validCommands == this.validCommands &&
                (x.validStates == this.validStates)));
               
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
                case ModelCode.DISCRETE_MAXVAL:
                    property.SetValue(maxValue);
                    break;
                case ModelCode.DISCRETE_MINVAL:
                    property.SetValue(minValue);
                    break;
                case ModelCode.DISCRETE_NORMVAL:
                    property.SetValue(normalValue);
                    break;
                case ModelCode.DISCRETE_VALIDCOMMANDS:
                    property.SetValue(validCommands.Cast<short>().ToList());  // ?
                    break;
                case ModelCode.DISCRETE_VALIDSTATES:
                    property.SetValue(validStates.Cast<short>().ToList());  // ?
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
                case ModelCode.DISCRETE_MAXVAL:
                case ModelCode.DISCRETE_MINVAL:
                case ModelCode.DISCRETE_NORMVAL:
                case ModelCode.DISCRETE_VALIDCOMMANDS:
                case ModelCode.DISCRETE_VALIDSTATES:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.DISCRETE_MAXVAL:
                    maxValue = property.AsInt();
                    break;
                case ModelCode.DISCRETE_MINVAL:
                    minValue = property.AsInt();
                    break;
                case ModelCode.DISCRETE_NORMVAL:
                    normalValue = property.AsInt();
                    break;
                case ModelCode.DISCRETE_VALIDCOMMANDS:
                    validCommands = property.AsEnums().Cast<Commands>().ToList();
                    break;
                case ModelCode.DISCRETE_VALIDSTATES:
                    validStates = property.AsEnums().Cast<States>().ToList();
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
