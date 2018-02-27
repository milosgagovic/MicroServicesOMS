using OMSSCADACommon.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OMSSCADACommon.Commands
{
    [DataContract]
    public class WriteSingleAnalog : Command
    {
        [DataMember]
        public float Value { get; set; }

        public override Response Execute()
        {
            return this.Receiver.WriteSingleAnalog(this.Id, this.Value);
        }
    }
}
