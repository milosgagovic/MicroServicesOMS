using PCCommon;
using ScadaCloud;
using ScadaCloud.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace SCADA.ConfigurationParser
{
    public class CommunicationModelParser
    {
        private Dictionary<string, ProcessController> processControllers;

        private string basePath;

        public CommunicationModelParser(string basePath = "")
        {
            processControllers = new Dictionary<string, ProcessController>();
            //this.basePath = basePath == "" ? Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName : basePath;
        }

        public bool DeserializeCommunicationModel(string deserializationSource = "RtuConfiguration.xml")
        {
            string message = string.Empty;

            try
            {
               // XElement xdocument = XElement.Load(Path.Combine(basePath, deserializationSource));
              //  IEnumerable<XElement> elements = xdocument.Elements();

                List<ProcessControlers> pcs = new List<ProcessControlers>();

                using (ScadaContextDB ctx = new ScadaContextDB())
                {
                    pcs = ctx.ProcesControlers.ToList();
                }

                if (pcs.Count != 0)
                {
                    foreach (var pc in pcs)
                    {
                        ProcessController newPc;
                        string uniqueName = (string)pc.Name;
                        if (!processControllers.ContainsKey(uniqueName))
                        {

                            TransportHandler transport = TransportHandler.TCP;

                            int devAddr = (int)pc.DeviceAddress;
                            string hostName = (string)pc.HostName;
                            short hostPort = (short)pc.HostPort;
                            newPc = new ProcessController()
                            {
                                Name = uniqueName,
                                DeviceAddress = devAddr,
                                TransportHandler = transport,
                                HostName = hostName,
                                HostPort = hostPort
                            };

                            // !!! samo zato sto trenutno ne koristimo druge, a ne radi dok se ne pokrenu svi procitani iz konfiguracije
                            if (newPc.Name.Equals("RTU-1"))
                            {
                                processControllers.Add(newPc.Name, newPc);
                            }
                        }
                        else
                        {
                            message = string.Format("Invalid config: There is multiple ProcessControllers with Name={0}!", uniqueName);
                            Console.WriteLine(message);
                            return false;
                        }
                    }
                }
                else
                {
                    message = string.Format("Invalid config: file must contain at least 1 Process Controller!");
                    Console.WriteLine(message);
                    return false;
                }
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
                return false;
            }

            return true;
        }

        public Dictionary<string, ProcessController> GetProcessControllers()
        {
            return processControllers;
        }

        // there is no Serialization of communication model. we do not need it. 
    }
}
