using OMSSCADACommon;
using PCCommon;
using SCADA.CommunicationAndControlling.SecondaryDataProcessing;
using SCADA.RealtimeDatabase;
using SCADA.RealtimeDatabase.Catalogs;
using SCADA.RealtimeDatabase.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;


namespace SCADA.ConfigurationParser
{
    public class ScadaModelParser
    {
        private string basePath;
        private DBContext dbContext = null;

        public ScadaModelParser(string basePath = "")
        {
           // this.basePath = basePath == "" ? Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName : basePath;
            dbContext = new DBContext();
        }

        public bool LoadScadaModel(string deserializationSource = "ScadaModel.xml")
        {
            // to do
            Database.IsConfigurationFinished = false;

            string message = string.Empty;
            string configurationName = deserializationSource;
          //  string source = Path.Combine(basePath, configurationName);

            if (Database.Instance.RTUs.Count != 0)
                Database.Instance.RTUs.Clear();

            if (Database.Instance.ProcessVariablesName.Count != 0)
                Database.Instance.ProcessVariablesName.Clear();

            try
            {
              //  XElement xdocument = XElement.Load(source);

                // access RTUS, DIGITALS, ANALOGS, COUNTERS from ScadaModel root
             //   IEnumerable<XElement> elements = xdocument.Elements();

                //var rtus = xdocument.Element("RTUS").Elements("RTU").ToList();
                //var digitals = (from dig in xdocument.Element("Digitals").Elements("Digital")
                //                orderby (int)dig.Element("RelativeAddress")
                //                select dig).ToList();

                //var analogs = (from dig in xdocument.Element("Analogs").Elements("Analog")
                //               orderby (int)dig.Element("RelativeAddress")
                //               select dig).ToList();

                //var counters = (from dig in xdocument.Element("Counters").Elements("Counter")
                //                orderby (int)dig.Element("RelativeAddress")
                //                select dig).ToList();

                List<ScadaDBClassLib.ModelData.RTU> rtus = new List<ScadaDBClassLib.ModelData.RTU>();
                List<ScadaDBClassLib.ModelData.Digital> digitals = new List<ScadaDBClassLib.ModelData.Digital>();
                List<ScadaDBClassLib.ModelData.Analog> analogs = new List<ScadaDBClassLib.ModelData.Analog>();

                using (ScadaDBClassLib.ScadaCtxcs ctx = new ScadaDBClassLib.ScadaCtxcs())
                {
                    rtus = ctx.RTUs.ToList();
                    digitals = ctx.Digitals.ToList();
                    analogs = ctx.Analogs.ToList();
                }

                // parsing RTUS
                if (rtus.Count != 0)
                {
                    foreach (var rtu in rtus)
                    {
                        RTU newRtu;
                        string uniqueName = (string)rtu.Name;

                        // if RTU with that name does not already exist?
                        if (!dbContext.Database.RTUs.ContainsKey(uniqueName))
                        {
                            byte address = (byte)(int)rtu.Address;

                            bool freeSpaceForDigitals = (bool)rtu.FreeSpaceForDigitals;
                            bool freeSpaceForAnalogs = (bool)rtu.FreeSpaceForAnalogs;


                            IndustryProtocols protocol = IndustryProtocols.ModbusTCP;

                            int digOutStartAddr = (int)rtu.DigOutStartAddr;
                            int digInStartAddr = (int)rtu.DigInStartAddr;
                            int anaInStartAddr = (int)rtu.AnaInStartAddr;
                            int anaOutStartAddr = (int)rtu.AnaOutStartAddr;
                            int counterStartAddr = (int)rtu.CounterStartAddr;

                            int digOutCount = (int)rtu.NoDigOut;
                            int digInCount = (int)rtu.NoDigIn;
                            int anaInCount = (int)rtu.NoAnaIn;
                            int anaOutCount = (int)rtu.NoAnaOut;
                            int counterCount = (int)rtu.NoCnt;

                            ushort anaInRawMin = (ushort)(int)rtu.AnaInRawMin;
                            ushort anaInRawMax = (ushort)(int)rtu.AnaInRawMax;
                            ushort anaOutRawMin = (ushort)(int)rtu.AnaOutRawMin;
                            ushort anaOutRawMax = (ushort)(int)rtu.AnaOutRawMax;

                            if (digOutCount != digInCount)
                            {
                                message = string.Format("Invalid config: RTU - {0}: Value of DigOutCount must be the same as Value of DigInCount", uniqueName);
                                Console.WriteLine(message);
                                return false;
                            }

                            newRtu = new RTU()
                            {
                                Name = uniqueName,
                                Address = address,
                                FreeSpaceForDigitals = freeSpaceForDigitals,
                                FreeSpaceForAnalogs = freeSpaceForAnalogs,
                                Protocol = protocol,

                                DigOutStartAddr = digOutStartAddr,
                                DigInStartAddr = digInStartAddr,
                                AnaInStartAddr = anaInStartAddr,
                                AnaOutStartAddr = anaOutStartAddr,
                                CounterStartAddr = counterStartAddr,

                                NoDigOut = digOutCount,
                                NoDigIn = digInCount,
                                NoAnaIn = anaInCount,
                                NoAnaOut = anaOutCount,
                                NoCnt = counterCount,

                                AnaInRawMin = anaInRawMin,
                                AnaInRawMax = anaInRawMax,
                                AnaOutRawMin = anaOutRawMin,
                                AnaOutRawMax = anaOutRawMax
                            };

                            //using (ScadaContextDB ctx = new ScadaContextDB())
                            //{
                            //    ctx.RTUs.Add(new ScadaCloud.Model.RTU
                            //    {
                            //        Name = uniqueName,
                            //        Address = address,
                            //        FreeSpaceForDigitals = freeSpaceForDigitals,
                            //        FreeSpaceForAnalogs = freeSpaceForAnalogs,
                            //        Protocol = protocol,

                            //        DigOutStartAddr = digOutStartAddr,
                            //        DigInStartAddr = digInStartAddr,
                            //        AnaInStartAddr = anaInStartAddr,
                            //        AnaOutStartAddr = anaOutStartAddr,
                            //        CounterStartAddr = counterStartAddr,

                            //        NoDigOut = digOutCount,
                            //        NoDigIn = digInCount,
                            //        NoAnaIn = anaInCount,
                            //        NoAnaOut = anaOutCount,
                            //        NoCnt = counterCount,

                            //        AnaInRawMin = anaInRawMin,
                            //        AnaInRawMax = anaInRawMax,
                            //        AnaOutRawMin = anaOutRawMin,
                            //        AnaOutRawMax = anaOutRawMax

                            //    });
                            //    ctx.SaveChanges();

                            //}

                            dbContext.AddRTU(newRtu);
                        }
                        else
                        {
                            // to do: bacati exception mozda
                            message = string.Format("Invalid config: There is multiple RTUs with Name={0}!", uniqueName);
                            Console.WriteLine(message);
                            return false;
                        }
                    }
                }
                else
                {
                    message = string.Format("Invalid config: file must contain at least 1 RTU!");
                    Console.WriteLine(message);
                    return false;
                }

                //parsing DIGITALS. ORDER OF RELATIVE ADDRESSES IS IMPORTANT
                if (digitals.Count != 0)
                {
                    foreach (var d in digitals)
                    {
                        string procContr = (string)d.ProcContrName;

                        // does RTU exists?
                        RTU associatedRtu;
                        if ((associatedRtu = dbContext.GetRTUByName(procContr)) != null)
                        {
                            Digital newDigital = new Digital();

                            // SETTING ProcContrName
                            newDigital.ProcContrName = procContr;

                            string uniqueName = (string)d.Name;

                            // variable with that name does not exists in db?
                            if (!dbContext.Database.ProcessVariablesName.ContainsKey(uniqueName))
                            {
                                // SETTING Name
                                newDigital.Name = uniqueName;

                                // SETTING State                             
                                string stringCurrentState = (string)d.State;
                                States stateValue = (States)Enum.Parse(typeof(States), stringCurrentState);
                                newDigital.State = stateValue;

                                // SETTING Command parameter - for initializing Simulator with last command
                                string lastCommandString = (string)d.Command;
                                CommandTypes command = (CommandTypes)Enum.Parse(typeof(CommandTypes), lastCommandString);

                                // SETTING Class

                                DigitalDeviceClasses devClass = DigitalDeviceClasses.SWITCH;
                                newDigital.Class = devClass;

                                // SETTING RelativeAddress
                                ushort relativeAddress = (ushort)(int)d.RelativeAddress;
                                newDigital.RelativeAddress = relativeAddress;

                                // SETTING ValidCommands
                                newDigital.ValidCommands.Add(CommandTypes.OPEN);
                                newDigital.ValidCommands.Add(CommandTypes.CLOSE);

                               
                                newDigital.ValidStates.Add(States.CLOSED);
                                newDigital.ValidStates.Add(States.OPENED);


                                //using (ScadaContextDB ctx = new ScadaContextDB())
                                //{
                                //    ctx.Digirals.Add(new ScadaCloud.Model.Digital
                                //    {
                                //        Name = uniqueName,
                                //        RelativeAddress = relativeAddress,
                                //        ProcContrName = procContr,
                                //        State = stringCurrentState,
                                //        Command = lastCommandString
                                //    });
                                //    ctx.SaveChanges();
                                //}

                                ushort calculatedRelativeAddres;
                                if (associatedRtu.TryMap(newDigital, out calculatedRelativeAddres))
                                {
                                    if (relativeAddress == calculatedRelativeAddres)
                                    {
                                        if (associatedRtu.MapProcessVariable(newDigital))
                                        {
                                            dbContext.AddProcessVariable(newDigital);
                                        }
                                    }
                                    else
                                    {
                                        message = string.Format("Invalid config: Variable = {0} RelativeAddress = {1} is not valid.", uniqueName, relativeAddress);
                                        Console.WriteLine(message);
                                        continue;
                                    }
                                }

                            }
                            else
                            {
                                message = string.Format("Invalid config: Name = {0} is not unique. Variable already exists", uniqueName);
                                Console.WriteLine(message);
                                continue;
                            }

                        }
                        else
                        {
                            message = string.Format("Invalid config: Parsing Digitals, ProcContrName = {0} does not exists.", procContr);
                            Console.WriteLine(message);
                            return false;
                        }
                    }
                }

                // parsing ANALOGS. ORDER OF RELATIVE ADDRESSES IS IMPORTANT
                if (analogs.Count != 0)
                {
                    foreach (var a in analogs)
                    {
                        string procContr = (string)a.ProcContrName;

                        // does RTU exists?
                        RTU associatedRtu;
                        if ((associatedRtu = dbContext.GetRTUByName(procContr)) != null)
                        {
                            Analog newAnalog = new Analog();

                            // SETTING ProcContrName
                            newAnalog.ProcContrName = procContr;

                            string uniqueName = (string)a.Name;

                            // variable with that name does not exists in db?
                            if (!dbContext.Database.ProcessVariablesName.ContainsKey(uniqueName))
                            {
                                // SETTING Name
                                newAnalog.Name = uniqueName;

                                // SETTING NumOfRegisters
                                ushort numOfReg = (ushort)(int)a.NumOfRegisters;
                                newAnalog.NumOfRegisters = numOfReg;

                                // SETTING AcqValue
                                ushort acqValue = (ushort)(float)a.AcqValue;
                                newAnalog.AcqValue = acqValue;

                                // SETTING CommValue
                                ushort commValue = (ushort)(float)a.CommValue;
                                newAnalog.CommValue = commValue;

                                // SETTING MinValue
                                float minValue = (float)a.MinValue;
                                newAnalog.MinValue = minValue;

                                // SETTING MaxValue
                                float maxValue = (float)a.MaxValue;
                                newAnalog.MaxValue = maxValue;

                                // SETTING UnitSymbol                             
                                string stringUnitSymbol = (string)a.UnitSymbol;
                                UnitSymbol unitSymbolValue = (UnitSymbol)Enum.Parse(typeof(UnitSymbol), stringUnitSymbol, true);
                                newAnalog.UnitSymbol = unitSymbolValue;

                                // SETTING RelativeAddress
                                ushort relativeAddress = (ushort)(int)a.RelativeAddress;
                                newAnalog.RelativeAddress = relativeAddress;

                                // svejedno je uzeli AnaInRawMin ili AnaOutRawMin -> isti su trenutni, 
                                // sve dok imamo samo Analog.cs a ne AnaIn.cs + AnaOut.cs (dok je kao za digital)
                                newAnalog.RawBandLow = associatedRtu.AnaInRawMin;
                                newAnalog.RawBandHigh = associatedRtu.AnaInRawMax;

                                //using (ScadaContextDB ctx = new ScadaContextDB())
                                //{
                                //    ctx.Analogs.Add(new ScadaCloud.Model.Analog
                                //    {
                                //        Name = uniqueName,
                                //        NumOfRegisters = numOfReg,
                                //        AcqValue = acqValue,
                                //        CommValue = commValue,
                                //        MaxValue = maxValue,
                                //        MinValue = minValue,
                                //        ProcContrName = procContr,
                                //        RelativeAddress = relativeAddress,
                                //        UnitSymbol = stringUnitSymbol

                                //    });
                                //    ctx.SaveChanges();

                                //}

                                // SETTING RawAcqValue and RawCommValue
                                AnalogProcessor.EGUToRawValue(newAnalog);

                                ushort calculatedRelativeAddres;
                                if (associatedRtu.TryMap(newAnalog, out calculatedRelativeAddres))
                                {
                                    if (relativeAddress == calculatedRelativeAddres)
                                    {
                                        if (associatedRtu.MapProcessVariable(newAnalog))
                                        {
                                            dbContext.AddProcessVariable(newAnalog);
                                        }
                                    }
                                    else
                                    {
                                        message = string.Format("Invalid config: Analog Variable = {0} RelativeAddress = {1} is not valid.", uniqueName, relativeAddress);
                                        Console.WriteLine(message);
                                        return false;
                                    }
                                }

                            }
                            else
                            {
                                message = string.Format("Invalid config: Name = {0} is not unique. Analog Variable already exists", uniqueName);
                                Console.WriteLine(message);
                                return false;
                            }
                        }
                        else
                        {
                            message = string.Format("Invalid config: Parsing Analogs, ProcContrName = {0} does not exists.", procContr);
                            Console.WriteLine(message);
                            return false;
                        }
                    }
                }

                // to do:
                //if (counters.Count != 0)
                //{

                //}

                Console.WriteLine("Configuration passed successfully.");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }

            Database.IsConfigurationFinished = true;
            return true;
        }

        public void SaveScadaModel(string serializationTarget = "ScadaModel.xml")
        {
            //string target = Path.Combine(basePath, serializationTarget);

            //XElement scadaModel = new XElement("ScadaModel");

            //XElement rtus = new XElement("RTUS");
            //XElement digitals = new XElement("Digitals");
            //XElement analogs = new XElement("Analogs");
            //XElement counters = new XElement("Counters");

            var rtusSnapshot = dbContext.Database.RTUs.ToArray();
            using (ScadaDBClassLib.ScadaCtxcs ctx = new ScadaDBClassLib.ScadaCtxcs())
            {
                foreach (var rtu in rtusSnapshot)
                {
                    #region xml
                    //XElement rtuEl = new XElement(
                    //     "RTU",
                    //     new XElement("Address", rtu.Value.Address),
                    //     new XElement("Name", rtu.Value.Name),
                    //     new XElement("FreeSpaceForDigitals", rtu.Value.FreeSpaceForDigitals),
                    //     new XElement("FreeSpaceForAnalogs", rtu.Value.FreeSpaceForAnalogs),
                    //     new XElement("Protocol", Enum.GetName(typeof(IndustryProtocols), rtu.Value.Protocol)),
                    //     new XElement("DigOutStartAddr", rtu.Value.DigOutStartAddr),
                    //     new XElement("DigInStartAddr", rtu.Value.DigInStartAddr),
                    //     new XElement("AnaOutStartAddr", rtu.Value.AnaOutStartAddr),
                    //     new XElement("AnaInStartAddr", rtu.Value.AnaInStartAddr),
                    //     new XElement("CounterStartAddr", rtu.Value.CounterStartAddr),
                    //     new XElement("NoDigOut", rtu.Value.NoDigOut),
                    //     new XElement("NoDigIn", rtu.Value.NoDigIn),
                    //     new XElement("NoAnaIn", rtu.Value.NoAnaIn),
                    //     new XElement("NoAnaOut", rtu.Value.NoAnaOut),
                    //     new XElement("NoCnt", rtu.Value.NoCnt),
                    //     new XElement("AnaInRawMin", rtu.Value.AnaInRawMin),
                    //     new XElement("AnaInRawMax", rtu.Value.AnaInRawMax),
                    //     new XElement("AnaOutRawMin", rtu.Value.AnaOutRawMin),
                    //     new XElement("AnaOutRawMax", rtu.Value.AnaOutRawMax)
                    //     );
                    #endregion
                    if (ctx.RTUs.FirstOrDefault(x => x.Name == rtu.Value.Name) == null)
                    {
                        ctx.RTUs.Add(new ScadaDBClassLib.ModelData.RTU
                        {
                            Name = rtu.Value.Name,
                            Address = rtu.Value.Address,
                            FreeSpaceForDigitals = rtu.Value.FreeSpaceForDigitals,
                            FreeSpaceForAnalogs = rtu.Value.FreeSpaceForAnalogs,
                            Protocol = rtu.Value.Protocol,

                            DigOutStartAddr = rtu.Value.DigOutStartAddr,
                            DigInStartAddr = rtu.Value.DigInStartAddr,
                            AnaInStartAddr = rtu.Value.AnaInStartAddr,
                            AnaOutStartAddr = rtu.Value.AnaOutStartAddr,
                            CounterStartAddr = rtu.Value.CounterStartAddr,

                            NoDigOut = rtu.Value.NoDigOut,
                            NoDigIn = rtu.Value.NoDigIn,
                            NoAnaIn = rtu.Value.NoAnaIn,
                            NoAnaOut = rtu.Value.NoAnaOut,
                            NoCnt = rtu.Value.NoCnt,

                            AnaInRawMin = rtu.Value.AnaInRawMin,
                            AnaInRawMax = rtu.Value.AnaInRawMax,
                            AnaOutRawMin = rtu.Value.AnaOutRawMin,
                            AnaOutRawMax = rtu.Value.AnaOutRawMax
                        });
                    }
                    //rtus.Add(rtuEl);
                }
                ctx.SaveChanges();
            }

            var pvsSnapshot = dbContext.Database.ProcessVariablesName.ToArray().OrderBy(pv => pv.Value.RelativeAddress);
            using (ScadaDBClassLib.ScadaCtxcs ctx = new ScadaDBClassLib.ScadaCtxcs())
            {

                foreach (var pv in pvsSnapshot)
                {
                    switch (pv.Value.Type)
                    {
                        case VariableTypes.DIGITAL:
                            Digital dig = pv.Value as Digital;
                            #region xml
                            //XElement validCommands = new XElement("ValidCommands");
                            //XElement validStates = new XElement("ValidStates");

                            //foreach (var state in dig.ValidStates)
                            //{
                            //    validStates.Add(new XElement("State", Enum.GetName(typeof(States), state)));
                            //}

                            //foreach (var command in dig.ValidCommands)
                            //{
                            //    validCommands.Add(new XElement("Command", Enum.GetName(typeof(CommandTypes), command)));
                            //}

                            //XElement digEl = new XElement(
                            //    "Digital",
                            //        new XElement("Name", dig.Name),
                            //        new XElement("State", dig.State),
                            //        new XElement("Command", dig.Command),
                            //        new XElement("ProcContrName", dig.ProcContrName),
                            //        new XElement("RelativeAddress", dig.RelativeAddress),
                            //        new XElement("Class", Enum.GetName(typeof(DigitalDeviceClasses), dig.Class)),
                            //        validCommands,
                            //        validStates
                            //    );
                            #endregion
                            if (ctx.Digitals.FirstOrDefault(x => x.Name == dig.Name) == null)
                            {
                                ctx.Digitals.Add(new ScadaDBClassLib.ModelData.Digital
                                {
                                    Name = dig.Name,
                                    RelativeAddress = dig.RelativeAddress,
                                    ProcContrName = dig.ProcContrName,
                                    State = dig.State.ToString(),
                                    Command = dig.Command.ToString()
                                });
                            }
                            // digitals.Add(digEl);
                            break;

                        case VariableTypes.ANALOG:
                            Analog analog = pv.Value as Analog;
                            #region xml
                            //XElement anEl = new XElement(
                            //    "Analog",
                            //        new XElement("Name", analog.Name),
                            //        new XElement("NumOfRegisters", analog.NumOfRegisters),
                            //        new XElement("AcqValue", analog.AcqValue),
                            //        new XElement("CommValue", analog.CommValue),
                            //        new XElement("MaxValue", analog.MaxValue),
                            //        new XElement("MinValue", analog.MinValue),
                            //        new XElement("ProcContrName", analog.ProcContrName),
                            //        new XElement("RelativeAddress", analog.RelativeAddress),
                            //        new XElement("UnitSymbol", Enum.GetName(typeof(UnitSymbol), analog.UnitSymbol))
                            //    );

                            //analogs.Add(anEl);
                            #endregion
                            if (ctx.Analogs.FirstOrDefault(x => x.Name == analog.Name) == null)
                            {
                                ctx.Analogs.Add(new ScadaDBClassLib.ModelData.Analog
                                {
                                    Name = analog.Name,
                                    NumOfRegisters = analog.NumOfRegisters,
                                    AcqValue = analog.AcqValue,
                                    CommValue = analog.CommValue,
                                    MaxValue = analog.MaxValue,
                                    MinValue = analog.MinValue,
                                    ProcContrName = analog.ProcContrName,
                                    RelativeAddress = analog.RelativeAddress,
                                    UnitSymbol = analog.UnitSymbol.ToString()
                                });
                            }
                            break;
                    }
                }
                //scadaModel.Add(rtus);
                //scadaModel.Add(digitals);
                //scadaModel.Add(analogs);
                //scadaModel.Add(counters);
                //var xdocument = new XDocument(scadaModel);
                try
                {
                    //xdocument.Save(target);
                    ctx.SaveChanges();
                    Console.WriteLine("Serializing ScadaModel succeed.");
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void SwapConfigs(string config1, string config2)
        {
            string config1path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, config1);
            string config2path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, config2);

            try
            {
                File.Move(config1path, "temp.txt");
                File.Move(config2path, config1path);
                File.Move("temp.txt", config2path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
            }
        }
    }
}
