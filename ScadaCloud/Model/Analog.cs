using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScadaCloud.Model
{
   public class Analog
    {
        [Key]
        public int Id { get; set; }

        public bool IsInit { get; set; }

        /// <summary>
        /// Unique name (e.g. Meas_A_1, Meas_D_1)
        /// </summary>
        public string Name { get; set; }

        public ushort NumOfRegisters { get; set; }
        /// <summary>
        /// Associated ProcessController Name
        /// </summary>
        public string ProcContrName { get; set; }
        /// <summary>
        /// Starts from 0. It is an offset in array of Process Variables of same type in specified Process Controller
        /// </summary>
        public int RelativeAddress { get; set; }
        // to do: add previous command and stuff...
        public float AcqValue { get; set; }

        /// <summary>The value ve command</summary>
        /// <remarks>This is value that we want to have initialy on simulator</remarks>
        public float CommValue { get; set; }

        public string UnitSymbol { get; set; }

        // to do: maybe use these limits for alarm generetion in future implementations. or raw limits.
        // limits to Acq/Comm value   
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public Analog()
        {

        }
    }
}
