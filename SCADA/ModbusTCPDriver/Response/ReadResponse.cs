using System;

namespace ModbusTCPDriver
{
    public abstract class ReadResponse : Response
    {
        public Byte ByteCount { get; set; }       
    }
}
