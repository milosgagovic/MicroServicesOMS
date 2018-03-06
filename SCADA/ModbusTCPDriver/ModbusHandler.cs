using System;
using PCCommon;

namespace ModbusTCPDriver
{
    /// <summary>
    /// Concrete protocol handler class
    /// </summary>
    public class ModbusHandler : IIndustryProtocolHandler
    {
        public IndustryProtocols ProtocolType { get; set; }

        public ModbusApplicationHeader Header { get; set; }
        public Request Request { get; set; }
        public Response Response { get; set; }

        public byte[] PackData()
        {
            // message must be in big endian order

            var bHeader = Header.getByteHeader();
            var bRequest = Request.GetByteRequest();

            byte[] packedData = new byte[bHeader.Length + bRequest.Length];

            bHeader.CopyTo(packedData, 0);
            bRequest.CopyTo(packedData, bHeader.Length);
            return packedData;
        }

        public void UnpackData(byte[] data, int length)
        {
            Header = new ModbusApplicationHeader();
            // Header = Header.getObjectHeader(data); // nepotrebno ipak

            byte[] responseData = new byte[length - 7];
            Buffer.BlockCopy(data, 7, responseData, 0, length - 7);

            // to do: handle exceptions in future implementation
            // if exception happens here, means that simulator could not process request. maybe request address and function code was not "aligned"
            // also, PAY ATTENTION to configuration files (values in RtuCfg.txt and ScadaModel.xml must correspond!)          
            if ((responseData[0] & 0x80) != 1) // check for exception
            {
                switch ((FunctionCodes)responseData[0])
                {
                    case FunctionCodes.WriteSingleCoil:
                    case FunctionCodes.WriteSingleRegister:

                        Response = new WriteResponse();
                        Response.GetObjectResponse(responseData);
                        break;

                    case FunctionCodes.ReadCoils:
                    case FunctionCodes.ReadDiscreteInput:

                        Response = new BitReadResponse();
                        Response.GetObjectResponse(responseData);

                        //Console.WriteLine("ReadDiscreteInput Response");
                        //Console.WriteLine(BitConverter.ToString(data, 0, length));
                        break;

                    case FunctionCodes.ReadHoldingRegisters:
                    case FunctionCodes.ReadInputRegisters:

                        Response = new RegisterReadResponse();
                        Response.GetObjectResponse(responseData);

                        //Console.WriteLine("ReadHoldingRegisters Response");
                        //Console.WriteLine(BitConverter.ToString(data, 0, length));
                        break;

                    default:
                        // obrati paznju na konfig fajlove ako ovvde pukne
                        Console.WriteLine("Error, ovo ne treeeba da se desava :O !!!!!");
                        throw new Exception("Something went wrong.Slave can not process request.");
                }
            }
            else
            {
                //Console.WriteLine("Something went wrong. Slave can not process request.");
                throw new Exception("Something went wrong.Slave can not process request.");
            }
        }
    }
}
