using System;

namespace ModbusTCPDriver
{
    public class WriteRequest : Request
    {
        public ushort Value { get; set; }

        public override byte[] GetByteRequest()
        {
            byte[] stAddr = BitConverter.GetBytes(StartAddr);
            byte[] val = BitConverter.GetBytes(Value);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(stAddr);
                Array.Reverse(val);
            }

            byte[] byteRequest = new byte[5]
            {
                (byte)FunCode,
                stAddr[0], stAddr[1],
                val[0], val[1]
            };

            return byteRequest;
        }
    }
}
