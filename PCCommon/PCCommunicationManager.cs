using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace PCCommon.Communication
{
    // for udp there can be some other parameters...
    public class CommunicationParameters
    {
        public string HostName { get; set; }

        public short HostPort { get; set; }

        // to do: use this in future implementations...
        // public int Timeout { get; set; }

        public CommunicationParameters()
        {

        }

        public CommunicationParameters(string hName, short hPort)
        {
            HostName = hName;
            HostPort = hPort;
        }
    }

    /// <summary>
    /// Wrapper around TcpClien, UdpClient...
    /// </summary>
    public abstract class CommunicationObject
    {
        public TransportHandler TrHandler { get; set; }

        public IORequestsQueue IORequests;

        public CommunicationObject()
        {
            IORequests = IORequestsQueue.GetQueue();
        }

        public CommunicationObject(TransportHandler trHndl)
        {
            IORequests = IORequestsQueue.GetQueue();
            TrHandler = trHndl;
        }

        public abstract bool Setup();

        public abstract bool ProcessRequest(IORequestBlock forProcess);

    }
    public abstract class CommunicationObjectFactory
    {
        public abstract CommunicationObject CreateNew();

        public abstract CommunicationObject CreateNew(CommunicationParameters commPar);
    }

    public static class CommunicationManager
    {
        public static ConcurrentDictionary<string, CommunicationObject> CommunicationObjects = null;

        public static CommunicationObjectFactory Factory = null;

        private static TransportHandler currentTrHnd;
        public static TransportHandler CurrentTransportHndl
        {
            get { return currentTrHnd; }
            set
            {
                currentTrHnd = value;
                switch (value)
                {
                    case TransportHandler.TCP:
                        Factory = new TcpCommObjFactory();
                        break;
                    default:
                        break;
                }
            }
        }

        static CommunicationManager()
        {
            CommunicationObjects = new ConcurrentDictionary<string, CommunicationObject>();

            // default
            currentTrHnd = TransportHandler.TCP;
        }
    }

    public class TcpCommObj : CommunicationObject
    {
        TcpClient tcpClient;
        CommunicationParameters communicationParams;

        public TcpCommObj() : base()
        {
            TrHandler = TransportHandler.TCP;
            tcpClient = new TcpClient();

            // to do: add some default params? :S 
            communicationParams = null;
        }

        public TcpCommObj(CommunicationParameters commPar) : base()
        {
            TrHandler = TransportHandler.TCP;
            tcpClient = new TcpClient();
            communicationParams = commPar;
        }

        public override bool ProcessRequest(IORequestBlock forProcess)
        {
            bool retVal = false;

            if (TrHandler != TransportHandler.TCP)
            {
                Console.WriteLine("ne sme da se desi xD ");
                // to do: throw exception?
                return false;
            }


            if (tcpClient != null)
            {
                try
                {
                    // to do: test this case...connection lasts forever? 
                    if (!tcpClient.Connected)
                    {
                        // to do...sredjivati ovo nekad
                    }

                    NetworkStream stream = tcpClient.GetStream();
                    int offset = 0;

                    stream.Write(forProcess.SendBuff, offset, forProcess.SendMsgLength);

                    // to do: processing big messages.  whole, or in parts?
                    // ...
                    // ovde dodati taski koji receieve radi

                    forProcess.RcvBuff = new byte[tcpClient.ReceiveBufferSize];

                    var length = stream.Read(forProcess.RcvBuff, offset, tcpClient.ReceiveBufferSize);
                    forProcess.RcvMsgLength = length;

                    IORequests.EnqueueAnswer(forProcess);
                }
                catch (Exception e )
                {
                    Console.WriteLine(e.Message);
                 //   throw;
                }



                return retVal;
            }

            return retVal;
        }

        public override bool Setup()
        {
            bool retVal = false;

            if (communicationParams == null)
            {
                Console.WriteLine("Error: Tcp communication parameters are not specified.");
                return false;
            }

            try
            {
                // connecting to slave
                tcpClient.Connect(communicationParams.HostName, communicationParams.HostPort);
                retVal = true;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("ArgumentOutOfRangeException - paramName = {0}", e.ParamName);
                Console.WriteLine(e.Message);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketExeption - ErrorCode = {0}", e.ErrorCode);
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {

            }
            return retVal;
        }
    }
    public class UdpCommObj : CommunicationObject
    {
        public override bool ProcessRequest(IORequestBlock forProcess)
        {
            throw new NotImplementedException();
        }

        public override bool Setup()
        {
            throw new NotImplementedException();
        }
    }
    public class UartCommObj : CommunicationObject
    {
        public override bool ProcessRequest(IORequestBlock forProcess)
        {
            throw new NotImplementedException();
        }

        public override bool Setup()
        {
            throw new NotImplementedException();
        }
    }

    public class TcpCommObjFactory : CommunicationObjectFactory
    {
        //  to do: add other paramateres for tcp connection, like timeout and bla bla...

        // to do...
        public override CommunicationObject CreateNew()
        {
            // ovde podrazumevati neke default vrednosti?
            return new TcpCommObj();
        }

        public override CommunicationObject CreateNew(CommunicationParameters commPar)
        {
            CommunicationObject retVal = new TcpCommObj(commPar);

            try
            {

            }
            catch (Exception)
            {

                throw;
            }
            return retVal;
        }
    }
    public class UdpCommObjFactory : CommunicationObjectFactory
    {
        public override CommunicationObject CreateNew()
        {
            throw new NotImplementedException();
        }

        public override CommunicationObject CreateNew(CommunicationParameters commPar)
        {
            throw new NotImplementedException();
        }
    }
    public class UartCommObjFactory : CommunicationObjectFactory
    {
        public override CommunicationObject CreateNew()
        {
            throw new NotImplementedException();
        }

        public override CommunicationObject CreateNew(CommunicationParameters commPar)
        {
            throw new NotImplementedException();
        }
    }
}
