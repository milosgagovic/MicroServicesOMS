using System;

namespace ModbusTCPDriver
{
    public class RegisterReadResponse : ReadResponse
    {
        public ushort[] RegValues;

        public override Response GetObjectResponse(byte[] bResponse)
        {
            FunCode = (FunctionCodes)bResponse[0];

            // number of bytes to follow (1 reg requested = 2 bytes)
            ByteCount = bResponse[1];

            // total number of registers requested
            int regReqCount = ByteCount / 2;
            RegValues = new ushort[regReqCount];

            for (int i = 0; i < regReqCount;)
            {
                Array.Reverse(bResponse, 2 + 2 * i, 2);
                RegValues[i] = BitConverter.ToUInt16(bResponse, 2 + 2 * i);
                i++;
            }
            return this;
        }
    }
}
