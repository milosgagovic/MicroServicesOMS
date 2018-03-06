using System;

namespace ModbusTCPDriver
{
    public class ModbusApplicationHeader
    {       
        /// <summary>
        /// Identification of a MODBUS request/response transaction
        /// </summary>
        /// <remarks>Fixed to 0. Initialized by the client, copied by the server from request to the response</remarks>
        public ushort TransactionId
        {
            get; set;
        }

        /// <summary>
        /// 0 = ModbuTCP protocol
        /// </summary>
        /// <remarks>Initialized by the client, copied by the server from request to the response</remarks>
        public ushort ProtocolId
        {
            get; set;
        }

        /// <summary>
        /// Number of bytes in the message to follow
        /// </summary>
        /// <remarks>Initialized by the client (request), initialized by the server (response)</remarks>
        public ushort Length
        {
            get; set;
        }

        /// <summary>
        /// Identification of a remote slave unit.
        /// </summary>
        /// <remarks>Initialized by the client (request), copied by the server from request to the response</remarks>
        public Byte DeviceAddress { get; set; }

        /// <summary>
        /// Converting ModbusApplicationHeader properties to appropriate byte array, for sending over the communication channel.
        /// </summary>
        /// <returns></returns>
        public byte[] getByteHeader()
        {
            byte[] transId = BitConverter.GetBytes(TransactionId);
            byte[] protocolId = BitConverter.GetBytes(ProtocolId);
            byte[] length = BitConverter.GetBytes(Length);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(transId);
                Array.Reverse(protocolId);
                Array.Reverse(length);
            }

            byte[] byteHeader = new byte[7]
            {
                transId[0],transId[1],
                protocolId[0], protocolId[1],
                length[0],length[1],
                DeviceAddress
            };

            return byteHeader;
        }

        /// <summary>
        /// Converting byte array to properties of ModbusApplicationHeader, after receiving through the communication channel.
        /// </summary>
        /// <param name="bHeader"></param>
        /// <returns></returns>
        public ModbusApplicationHeader getObjectHeader(byte[] bHeader)
        {     
            // transaction Id
            Array.Reverse(bHeader, 0, 2);

            // protocol Id
            Array.Reverse(bHeader, 2, 2);

            // Length
            Array.Reverse(bHeader, 4, 2);

            TransactionId = BitConverter.ToUInt16(bHeader, 0);
            ProtocolId = BitConverter.ToUInt16(bHeader, 2);
            Length = BitConverter.ToUInt16(bHeader, 4);

            DeviceAddress = bHeader[6];
            return this;
        }
    }
}
