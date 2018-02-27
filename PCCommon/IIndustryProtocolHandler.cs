
namespace PCCommon
{
    public interface IIndustryProtocolHandler
    {
        IndustryProtocols ProtocolType { get; set; }
       
        byte[] PackData();

        void UnpackData(byte[] data, int length);     
    }
}
