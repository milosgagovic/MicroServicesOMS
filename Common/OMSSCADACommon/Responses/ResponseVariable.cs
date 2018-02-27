using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSSCADACommon.Responses
{
    public abstract class ResponseVariable
    {
        public string Id { get; set; }

        public ResponseType VariableType { get; set; }
    }
}
