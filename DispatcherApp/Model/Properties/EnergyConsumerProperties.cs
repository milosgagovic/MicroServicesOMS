using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DispatcherApp.Model.Properties
{
    public class EnergyConsumerProperties : ElementProperties
    {
        public new void ReadFromResourceDescription(ResourceDescription rd)
        {
            base.ReadFromResourceDescription(rd);
        }
    }
}
