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

    /// <summary>
    /// Commanding/Acquisition request description
    /// </summary>
    /// <remarks>Request defines single (or broadcast - in future implementations) communication transaction
    /// between SCADA sw and process controller(s).</remarks>
    public class IORequestBlock
    {
        /// <summary>Transaction type</summary>
        public RequestType RequestType { get; set; }

        /// <summary>Request address in ProcessController address map</summary>
        public ushort ReqAddress { get; set; }

        /// <summary>Target slave device Identification</summary>
        public string ProcessControllerName { get; set; }

        /* request parameters*/

        // to do: for future implementation
        // public int MaxRepeat { get; set; }

        /// <summary>Request length</summary>
        public int SendMsgLength { get; set; }

        /// <summary>Transmitter buffer for storing outgoing data</summary>
        public Byte[] SendBuff { get; set; }


        /* reply parameters*/

        /// <summary>Reply Length</summary>
        public int RcvMsgLength { get; set; }

        /// <summary>Receiver buffer for incoming data</summary>
        public Byte[] RcvBuff { get; set; }

        // to do:
        // setovati ga kod init sim na 0 ili -1

        /// <summary>Has different roles, based on request type and request content.
        /// Currently, used for indicating count of variables for which request was sent</summary>
        public int Flags { get; set; }
    }
}
