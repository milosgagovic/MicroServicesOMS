using FTN.Common;
using OMSSCADACommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherApp.Model.Measurements
{
    public class DigitalMeasurement : Measurement, INotifyPropertyChanged
    {
        private OMSSCADACommon.States state;

        public new void ReadFromResourceDescription(ResourceDescription rd)
        {
            try
            {
                var temp = rd.GetProperty(ModelCode.DISCRETE_NORMVAL).AsInt();
                this.State = (OMSSCADACommon.States)Enum.GetValues(typeof(OMSSCADACommon.States)).GetValue(temp);
            } catch { }
            base.ReadFromResourceDescription(rd);
        }

        public OMSSCADACommon.States State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
                RaisePropertyChanged("State");
            }
        }
    }
}
