using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.RealtimeDatabase.Model
{
    public abstract class ProcessVariable : EventArgs
    {
        public string Name { get; set; }

        /// <summary>
        /// Associated ProcessController Name
        /// </summary>
        public string ProcContrName { get; set; }

        /// <summary>
        /// Starts from 0. It is an offset in array of Process Variables of same type in specified Process Controller
        /// </summary>
        public ushort RelativeAddress { get; set; }

        public VariableTypes Type { get; set; }

        public ProcessVariable()
        {
        }
    }
}
