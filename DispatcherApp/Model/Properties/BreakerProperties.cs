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
        private OMSSCADACommon.States state;
        private List<CommandTypes> validCommands;

        public BreakerProperties()
        {
            ValidCommands = new List<CommandTypes>();
        }

        public new void ReadFromResourceDescription(ResourceDescription rd)
        {
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
