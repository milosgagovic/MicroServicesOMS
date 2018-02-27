
namespace ModbusTCPDriver
{
    public abstract class Response
    {
        public FunctionCodes FunCode { get; set; }

        public abstract Response GetObjectResponse(byte[] bResponse);
    }
}
