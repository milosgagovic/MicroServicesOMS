using FTN.Common;
using OMSSCADACommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DispatcherApp.Model.Properties
{
    public class BreakerProperties : ElementProperties
    {
        private List<CommandTypes> validCommands;

        public BreakerProperties()
        {
            ValidCommands = new List<CommandTypes>();
        }

        public new void ReadFromResourceDescription(ResourceDescription rd)
        {
            base.ReadFromResourceDescription(rd);
        }

        public List<CommandTypes> ValidCommands
        {
            get
            {
                return validCommands;
            }
            set
            {
                validCommands = value;
                RaisePropertyChanged("ValidCommands");
            }
        }
    }
}
