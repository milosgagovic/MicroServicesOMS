using PubSubContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSubscribeService.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class SubscriptionService : ISubscription
    {       
        public void Subscribe()
        {
            IPublishing subscriber = OperationContext.Current.GetCallbackChannel<IPublishing>();
            PubSubscribeDB.AddSubscriber(subscriber);
        }

        public void UnSubscribe()
        {
            IPublishing subscriber = OperationContext.Current.GetCallbackChannel<IPublishing>();
            PubSubscribeDB.RemoveSubsriber(subscriber);
        }
    }
}
