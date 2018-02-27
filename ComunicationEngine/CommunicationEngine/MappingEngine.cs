using DispatcherApp.Model;
using FTN.Common;
using OMSSCADACommon.Commands;
using OMSSCADACommon.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationEngine
{
    public class MappingEngine
    {
        private static MappingEngine instance;
        private static ModelGda model;
        public static MappingEngine Instance
        {
            get
            {
                if (instance == null)
                    instance = new MappingEngine();
                return instance;
            }
        }
        public MappingEngine()
        {
            model = new ModelGda();
            instance = new MappingEngine();
        }
        public List<ResourceDescription> MappResult(Response response)
        {
            List<ResourceDescription> retVal = new List<ResourceDescription>();
            List<long> AnalogList = model.GetExtentValues(ModelCode.ANALOG);
            List<long> DiscreteList = model.GetExtentValues(ModelCode.DISCRETE);
            ResourceDescription rd;
            foreach (ResponseVariable rv in response.Variables)
            {
                //izmijena zato sto vraca mrid a ne gid
                rd = new ResourceDescription();
                DigitalVariable dv = rv as DigitalVariable;
                rd = new ResourceDescription();
                rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, dv.Id));
                if(dv.State.ToString() == "CLOSED")
                {
                    rd.AddProperty(new Property(ModelCode.DISCRETE_NORMVAL, 0));
                }
                else
                {
                    rd.AddProperty(new Property(ModelCode.DISCRETE_NORMVAL,1));
                }
                retVal.Add(rd);
                //long measID = Convert.ToInt64(rv.Id);
                //string variableType = rv.GetType().ToString();
                //if (variableType == "AnalogVariable" && AnalogList.Contains(measID))
                //{
                //    // model.GetValues(measID) ako bude trebalo jos nesto da se ucita da se nasminka
                //    AnalogVariable av = rv as AnalogVariable;
                //    rd = new ResourceDescription();
                //    rd.Id = measID;
                //    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, measID));
                //    rd.AddProperty(new Property(ModelCode.ANALOG_NORMVAL, av.Value));
                //    //dodati vrijednost
                //    retVal.Add(rd);
                //}
                //else if (variableType == "DigitalVariable" && DiscreteList.Contains(measID))
                //{
                //    DigitalVariable dv = rv as DigitalVariable;
                //    rd = new ResourceDescription();
                //    rd.Id = measID;
                //    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, measID));
                //    rd.AddProperty(new Property(ModelCode.DISCRETE_NORMVAL, dv.State.ToString()));
                //    //dodati vrijednost
                //    retVal.Add(rd);
                //}
            }

            //vratiti rezultat korisniku
            return retVal;
        }

        public Command MappCommand(TypeOfSCADACommand typeOfCommand)
        {
            switch (typeOfCommand)
            {
                case TypeOfSCADACommand.ReadAll:
                    return new ReadAll();
                case TypeOfSCADACommand.WriteAnalog:
                    return new WriteSingleAnalog();
                case TypeOfSCADACommand.WriteDigital:
                    return new WriteSingleDigital();
            }

            return null;
            ////naapirati klijentsku komandu na scada komandu
            //ReadAll readAllCommand = new ReadAll();
            //return readAllCommand;
        }

        public MappingEngine getInstanceForTest()
        {
            return Instance;
        }
    }
}
