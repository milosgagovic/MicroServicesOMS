using System;

namespace ModbusTCPDriver
{
    public class ReadRequest : Request
    {
        public ushort Quantity { get; set; }

        public override byte[] GetByteRequest()
        {
            byte[] stAddr = BitConverter.GetBytes(StartAddr);
            byte[] qnt = BitConverter.GetBytes(Quantity);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(stAddr);
                Array.Reverse(qnt);
            }

            byte[] byteRequest = new byte[5]
            {
                (byte)FunCode,
                stAddr[0], stAddr[1],
                qnt[0], qnt[1]
            };

            return byteRequest;
        }
    }
}
