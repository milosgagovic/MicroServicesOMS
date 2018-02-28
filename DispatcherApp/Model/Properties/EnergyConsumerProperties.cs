using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DispatcherApp.Model.Properties
{
    public class EnergyConsumerProperties : ElementProperties
    {
        private bool call;
        private double pfixed;

        public bool Call
        {
            get
            {
                return call;
            }
            set
            {
                call = value;
                RaisePropertyChanged("Call");
            }
        }

        public double Pfixed
        {
            get
            {
                return pfixed;
            }
            set
            {
                pfixed = value;
                RaisePropertyChanged("Pfixed");
            }
        }

        public new void ReadFromResourceDescription(ResourceDescription rd)
        {
            try { this.Pfixed = rd.GetProperty(ModelCode.ENERGCONSUMER_PFIXED).AsFloat(); } catch { }
            base.ReadFromResourceDescription(rd);
        }
    }
}
