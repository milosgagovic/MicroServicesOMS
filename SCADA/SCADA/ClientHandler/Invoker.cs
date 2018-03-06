using OMSSCADACommon;
using OMSSCADACommon.Commands;
using OMSSCADACommon.Responses;
using SCADAContracts;
using System.ServiceModel;


namespace SCADA.ClientHandler
{
    /*Generally, if a message arrives for an instance that violates its concurrency mode, the message waits until the instance is available, or until it times out.

In addition, if the ConcurrencyMode is set to Single and a reentrant call is blocked while waiting for the instance to be freed, the system detects the deadlock and throws an exception.

 In Single instancing, either Single or Multiple concurrency is relevant, depending on whether the single instance processes messages sequentially or concurrently*/
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class Invoker : ISCADAContract
    {
        ICommandReceiver receiver;
        public Invoker()
        {
            receiver = new CommunicationAndControlling.CommandingAcquisitionEngine();
            string[] args = { };
            SCADA.Program.Main(args);
        }

        public Response ExecuteCommand(Command command)
        {
            command.Receiver = receiver;
            return command.Execute();
        }

        public bool Ping()
        {
            return true;
        }
    }
}

     
   
