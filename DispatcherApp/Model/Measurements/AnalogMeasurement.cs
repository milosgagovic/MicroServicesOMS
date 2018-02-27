using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherApp.Model.Measurements
{
    public class AnalogMeasurement : Measurement
    {
        private float value;
        private string measurementType;
        private UnitSymbol unitSymbol;

        public new void ReadFromResourceDescription(ResourceDescription rd)
        {
            try { this.Value = rd.GetProperty(ModelCode.ANALOG_NORMVAL).AsFloat(); } catch { }
            try { this.MeasurementType = rd.GetProperty(ModelCode.MEASUREMENT_TYPE).AsString(); } catch { }
            try { this.UnitSymbol = (UnitSymbol)rd.GetProperty(ModelCode.MEASUREMENT_UNITSYMB).AsEnum(); } catch { }
            base.ReadFromResourceDescription(rd);
        }

        public float Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                RaisePropertyChanged("Value");
            }
        }

        public string MeasurementType
        {
            get
            {
                return this.measurementType;
            }
            set
            {
                this.measurementType = value;
                RaisePropertyChanged("MeasurementType");
            }
        }

        public UnitSymbol UnitSymbol
        {
            get
            {
                return this.unitSymbol;
            }
            set
            {
                this.unitSymbol = value;
                RaisePropertyChanged("UnitSymbol");
            }
        }
    }
}
