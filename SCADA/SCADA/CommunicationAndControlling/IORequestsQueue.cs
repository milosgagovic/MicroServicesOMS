using PCCommon;
using System.Collections.Concurrent;

namespace SCADA.CommunicationAndControlling
{
    /// <summary>
    /// Used for inter-thread communication
    /// (between ACQEngine and PCCommunicationEngine)
    /// </summary>
    public class IORequestsQueue
    {
        private static object syncObj = new object();
        private static volatile IORequestsQueue instance;

        private ConcurrentQueue<IORequestBlock> ioRequests;
        private ConcurrentQueue<IORequestBlock> ioAnswers;

        public ConcurrentQueue<IORequestBlock> IORequests
        {
            get
            {
                return ioRequests;
            }
            set
            {
                if (value != null)
                {
                    ioRequests = value;
                }
            }
        }
        public ConcurrentQueue<IORequestBlock> IOAnswers
        {
            get
            {
                return ioAnswers;
            }
            set
            {
                if (value != null)
                {
                    ioAnswers = value;
                }
            }
        }

        private IORequestsQueue()
        {
            IORequests = new ConcurrentQueue<IORequestBlock>();
            IOAnswers = new ConcurrentQueue<IORequestBlock>();
        }

        public static IORequestsQueue GetQueue()
        {
            if (instance == null)
            {
                lock (syncObj)
                {
                    if (instance == null)
                    {
                        instance = new IORequestsQueue();
                    }
                }
            }
            return instance;
        }

        /* IORequests queue methods */
        public void EnqueueRequest(IORequestBlock iorb)
        {
            IORequests.Enqueue(iorb);
        }

        // read more about this methods and ConcurentQueue
        public IORequestBlock DequeueRequest(out bool isSuccessful)
        {
            IORequestBlock req;
            isSuccessful = IORequests.TryDequeue(out req);
            return req;
        }

        /* IOAnswers queue methods */
        public void EnqueueAnswer(IORequestBlock iorb)
        {
            IOAnswers.Enqueue(iorb);
        }

        public IORequestBlock DequeueAnswer(out bool isSuccessful)
        {
            IORequestBlock answ;
            isSuccessful = IOAnswers.TryDequeue(out answ);
            return answ;
        }
    }
}
