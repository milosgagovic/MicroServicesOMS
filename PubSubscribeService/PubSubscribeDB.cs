using PubSubContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubscribeService
{
    public class PubSubscribeDB
    {
        private static object locker = new object();

        private static List<IPublishing> subscribers = new List<IPublishing>();

        public static List<IPublishing> Subscribers
        {
            get
            {
                return subscribers;
            }
        }

        public static void AddSubscriber(IPublishing subscriber)
        {
            lock (locker)
            {
                try
                {
                    Subscribers.Add(subscriber);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public static void RemoveSubsriber(IPublishing subscriber)
        {
            lock (locker)
            {
                Subscribers.Remove(subscriber);
            }
        }
    }
}
