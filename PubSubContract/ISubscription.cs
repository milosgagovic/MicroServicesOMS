using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSubContract
{
    [ServiceContract(CallbackContract = typeof(IPublishing))]
    public interface ISubscription
    {
        [OperationContract]
        void Subscribe();

        [OperationContract]
        void UnSubscribe();
    }
}
