using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OMSSCADACommon.Responses
{
    [DataContract]
    [KnownType(typeof(AnalogVariable))]
    [KnownType(typeof(DigitalVariable))]
    [KnownType(typeof(CounterVariable))]
    public class Response
    {
        [DataMember]
        public List<ResponseVariable> Variables;

        [DataMember]
        public ResultMessage ResultMessage;

        public Response()
        {
            Variables = new List<ResponseVariable>();
        }
    }
}
