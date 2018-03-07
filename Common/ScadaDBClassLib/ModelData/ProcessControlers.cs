using PCCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScadaDBClassLib.ModelData
{
    public class ProcessControlers
    {
        [Key]
        public int Id { get; set; }
        public int DeviceAddress { get; set; }

        // unique name  
        public string Name { get; set; }

        // to do: use this in future implementations...
        // the time in which the message must be sent
        // public int Timeout { get; set; }
        public TransportHandler TransportHandler { get; set; }

        public string HostName { get; set; }

        public short HostPort { get; set; }
    }
}
