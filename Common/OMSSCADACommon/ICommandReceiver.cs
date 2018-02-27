using OMSSCADACommon.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace OMSSCADACommon
{
    public interface ICommandReceiver
    {
        Response WriteSingleDigital(string id, CommandTypes command);
        Response WriteSingleAnalog(string id, float value);
        Response ReadSingleDigital(string id);
        Response ReadSingleAnalog(string id);
        Response ReadSingleCounter(string id);
        Response ReadAllDigital(DeviceTypes type);
        Response ReadAllAnalog(DeviceTypes type);
        Response ReadAllCounter(DeviceTypes type);
        Response ReadAll();
    }
}
