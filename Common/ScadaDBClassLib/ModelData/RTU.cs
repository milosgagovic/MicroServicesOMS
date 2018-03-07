using PCCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScadaDBClassLib.ModelData
{
    public class RTU
    {
        [Key]
        public int Id { get; set; }
        //DOdati protokol
        public IndustryProtocols Protocol { get; set; }

        /// <summary>
        /// Identification of end device.
        /// </summary>
        /// <remarks>In case of Modbus - value in range 1 - 247 (0 - broadcast)</remarks>
        public int Address { get; set; }

        /// <summary>
        /// Unique Name (e.g. RTU-1)
        /// </summary>
        public string Name { get; set; }

        public bool FreeSpaceForDigitals { get; set; }
        public bool FreeSpaceForAnalogs { get; set; }
        // to do: add "free space" for counters...

        /* Controller pI/O starting Addresses. These are values of the second collumn of 
        the RtuCfg.txt file, and they are also in ScadaModel.xml under the corresponding names. */
        public int DigOutStartAddr { get; set; }
        public int DigInStartAddr { get; set; }
        public int AnaInStartAddr { get; set; }
        public int AnaOutStartAddr { get; set; }
        public int CounterStartAddr { get; set; }

        // number of pI/O (max number of process variables of certain type)
        public int NoDigOut { get; set; }
        public int NoDigIn { get; set; }
        public int NoAnaIn { get; set; }
        public int NoAnaOut { get; set; }
        public int NoCnt { get; set; }

        // currently, we use only AnaInRawMin and AnaInRawMax, cause we suppose same values for Out registers. 
        // to do: fix everything in future implementations :) 
        // raw band limits
        public int AnaInRawMin { get; set; }
        public int AnaInRawMax { get; set; }
        public int AnaOutRawMin { get; set; }
        public int AnaOutRawMax { get; set; }

        // to do: add "band" for counter...

        public int MappedDig { get; set; }
        public int MappedAnalog { get; set; }
        public int MappedCounter { get; set; }
    }
}
