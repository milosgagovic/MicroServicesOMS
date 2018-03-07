using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScadaDBClassLib.ModelData
{
    public class Digital
    {
        [Key]
        public int Id { get; set; }

        public bool IsInit { get; set; }

        public string State { get; set; }

        public string Command { get; set; }
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
        public int RelativeAddress { get; set; }
        // to do: add previous command and stuff...

        public Digital()
        {
        }
    }
}
