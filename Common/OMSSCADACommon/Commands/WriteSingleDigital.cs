using OMSSCADACommon.Responses;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OMSSCADACommon.Commands
{
    [DataContract]
    public class WriteSingleDigital : Command
    {
        [DataMember]
        public CommandTypes CommandType { get; set; }

        public override Response Execute()
        {
            return this.Receiver.WriteSingleDigital(this.Id, this.CommandType);
        }
    }
}
