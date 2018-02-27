using System;

namespace PCCommon
{
    public enum RequestType
    {
        SEND_RECV = 0, // used for unbalanced protocols
        RECV_SEND,
        SEND,
        RECV,
        CONNECT,
        DISCONNECT
    }

    /* 
     * Commanding/Acquisition request description
     * 
     * Request defines single (or broadcast) communication transaction
     * between SCADA sw and process controller(s).    
     */
    public class IORequestBlock
    {
        // transaction type.
        public RequestType RequestType { get; set; }

        // request address in ProcessController address map
        public ushort ReqAddress { get; set; }

        // target slave device Id - RTU address
        public string ProcessControllerName { get; set; }

        /* request parameters*/

        // public int MaxRepeat { get; set; }

        public int SendMsgLength { get; set; }

        // trsciever buffer 
        public Byte[] SendBuff { get; set; }


        /* reply parameters*/

        public int RcvMsgLength { get; set; }

        // receiver buffer 
        public Byte[] RcvBuff { get; set; }
    }
}
