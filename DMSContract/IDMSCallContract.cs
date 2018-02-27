using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DMSContract
{
    [ServiceContract]
    public interface IDMSCallContract
    {
        [OperationContract]
        void SendCall(string mrid);
    }
}
