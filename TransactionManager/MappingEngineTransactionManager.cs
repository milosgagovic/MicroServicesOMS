using DispatcherApp.Model;
using FTN.Common;
using OMSSCADACommon;
using OMSSCADACommon.Commands;
using OMSSCADACommon.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransactionManager
{
    public class MappingEngineTransactionManager
    {
        private static MappingEngineTransactionManager instance;
        public static MappingEngineTransactionManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new MappingEngineTransactionManager();
                return instance;
            }
        }
        public MappingEngineTransactionManager()
        {
        }


        public List<ResourceDescription> MappResult(Response response)
        {
            List<ResourceDescription> retVal = new List<ResourceDescription>();
            ResourceDescription rd;

            foreach (ResponseVariable rv in response.Variables)
            {
                rd = new ResourceDescription();
                rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, rv.Id));

                switch (rv.VariableType)
                {
                    case ResponseType.Analog:
                        AnalogVariable av = rv as AnalogVariable;
                        UnitSymbol unitSymbolValue = UnitSymbol.none;
                        try
                        {
                            unitSymbolValue = (UnitSymbol)Enum.Parse(typeof(UnitSymbol), av.UnitSymbol, true);
                        }
                        catch (Exception e)
                        {

                            //throw;
                        }

                        Console.WriteLine("Variable = {0}, Value = {1}, Unit={2}", av.Id, av.Value,av.UnitSymbol.ToString());
                        rd.AddProperty(new Property(ModelCode.ANALOG_NORMVAL, av.Value));

                        break;

                    case ResponseType.Digital:

                        DigitalVariable dv = rv as DigitalVariable;

                        Console.WriteLine("Variable = {0}, STATE = {1}", dv.Id, dv.State);

                        if (dv.State.ToString() == "CLOSED")
                        {
                            rd.AddProperty(new Property(ModelCode.DISCRETE_NORMVAL, 0));
                        }
                        else
                        {
                            rd.AddProperty(new Property(ModelCode.DISCRETE_NORMVAL, 1));
                        }
                        break;

                    case ResponseType.Counter:
                        break;
                }

                retVal.Add(rd);
            }
            return retVal;
        }

        public Command MappCommand(TypeOfSCADACommand typeOfCommand, string mrid, CommandTypes command, float value)
        {
            switch (typeOfCommand)
            {
                case TypeOfSCADACommand.ReadAll:
                    return new ReadAll();
                case TypeOfSCADACommand.WriteAnalog:
                    return new WriteSingleAnalog() { Id = mrid, Value = value };
                case TypeOfSCADACommand.WriteDigital:
                    return new WriteSingleDigital() { Id = mrid, CommandType = command };
            }

            return null;
            ////naapirati klijentsku komandu na scada komandu
            //ReadAll readAllCommand = new ReadAll();
            //return readAllCommand;
        }

        public MappingEngineTransactionManager getInstanceForTest()
        {
            return Instance;
        }
    }
}
