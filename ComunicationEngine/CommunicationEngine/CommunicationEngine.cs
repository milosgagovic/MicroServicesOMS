using CommunicationEngineContract;
using FTN.Common;
using OMSSCADACommon.Responses;
using PubSubscribe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationEngine
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class CommunicationEngine: ICommunicationEngineContract
    {
        private static ICommuncEngineContract_CallBack callback = null;

        private static CommunicationEngine instance;
        public static CommunicationEngine Instance
        {
            get
            {
                if (instance == null)
                    instance = new CommunicationEngine();
                return instance;
            }
        }

        public static ICommuncEngineContract_CallBack Callback
        {
            get { return callback; }
            set { callback = value; }
        }

        public CommunicationEngine()
        {
        }

        public bool SendResponseToClient(Response response)
        {
            //Delta delta = new Delta();
            //List<ResourceDescription> result = MappingEngine.Instance.MappResult(response);
            //Publisher publisher = new Publisher();
            //delta.TestOperations = result;
            //publisher.PublishDelta(delta);
            //proslijediti klijentu
            return true;
        }

        public bool ReceiveValue()
        {
            Callback = OperationContext.Current.GetCallbackChannel<ICommuncEngineContract_CallBack>();
            Console.WriteLine("Something comes from SCADA");
            return true;
        }

    }
}
