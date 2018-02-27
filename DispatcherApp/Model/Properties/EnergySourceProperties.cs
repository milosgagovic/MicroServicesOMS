using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DispatcherApp.Model.Properties
{
    public class EnergySourceProperties  : ElementProperties
    {
        public new void ReadFromResourceDescription(ResourceDescription rd)
        {
            base.ReadFromResourceDescription(rd);
        }
    }
}
