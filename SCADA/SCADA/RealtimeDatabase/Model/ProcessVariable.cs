using System;

namespace SCADA.RealtimeDatabase.Model
{
    public abstract class ProcessVariable : EventArgs
    {
        /// <summary>
        /// Indicates if variable should be included in acquistion. If it is inserted with some value,
        /// first we have to init it on simulator (set appropriate simulator registers).
        /// </summary>
        public bool IsInit { get; set; }

        /// <summary>
        /// Unique name (e.g. Meas_A_1, Meas_D_1)
        /// </summary>
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
