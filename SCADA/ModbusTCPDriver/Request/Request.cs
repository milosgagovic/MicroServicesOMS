
namespace ModbusTCPDriver
{
    public abstract class Request
    {
        public FunctionCodes FunCode { get; set; }
        public ushort StartAddr { get; set; }

        public abstract byte[] GetByteRequest();
    }
}
