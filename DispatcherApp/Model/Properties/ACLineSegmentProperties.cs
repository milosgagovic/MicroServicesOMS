using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DispatcherApp.Model.Properties
{
    public class ACLineSegmentProperties : ElementProperties
    {
        private float lenght;

        public new void ReadFromResourceDescription(ResourceDescription rd)
        {
            try { this.Length = rd.GetProperty(ModelCode.CONDUCTOR_LEN).AsFloat(); } catch { }
            base.ReadFromResourceDescription(rd);
        }

        public float Length
        {
            get
            {
                return lenght;
            }
            set
            {
                lenght = value;
                RaisePropertyChanged("Length");
            }
        }
    }
}
