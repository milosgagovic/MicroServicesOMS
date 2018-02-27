using OMSSCADACommon;
using SCADA.RealtimeDatabase.Catalogs;
using SCADA.RealtimeDatabase.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SCADA.RealtimeDatabase
{
    public class DBContext
    {
        public Database Database { get; set; }

        public DBContext()
        {
            Database = Database.Instance;
        }
      
        public static event EventHandler OnAnalogAdded;

        /// <summary>
        /// Attempts to add the specified key and value. Return true if success, 
        /// false if the key already exists.
        /// </summary>
        /// <param name="rtu"></param>
        public bool AddRTU(RTU rtu)
        {
            return Database.Instance.RTUs.TryAdd(rtu.Name, rtu);
        }

        /// <summary>
        /// Attempts to get value associated with the name. Returns value 
        /// if exists, otherwise null.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RTU GetRTUByName(string name)
        {
            RTU rtu;
            Database.Instance.RTUs.TryGetValue(name, out rtu);

            return rtu;
        }

        public ConcurrentDictionary<string, RTU> GettAllRTUs()
        {
            return Database.RTUs;
        }

        /// <summary>
        /// Storing the Process Varible in dictionary. Key= pv.Name, Value=pv
        /// </summary>
        /// <param name="pv"></param>
        public void AddProcessVariable(ProcessVariable pv)
        {
            Console.WriteLine("Adding process variable Name= {0}", pv.Name);
            Database.Instance.ProcessVariablesName.TryAdd(pv.Name, pv);
            if (pv.Type == VariableTypes.ANALOG)
            {
                OnAnalogAdded?.Invoke(this, (Analog)pv);
            }
        }

        /// <summary>
        /// Return Process Variable if exists; 
        /// otherwise pv=null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetProcessVariableByName(string name, out ProcessVariable pv)
        {
            return (Database.Instance.ProcessVariablesName.TryGetValue(name, out pv));
        }

        public List<ProcessVariable> GetAllProcessVariables()
        {
            return Database.ProcessVariablesName.Values.ToList();
        }


        /* possible scenarios:
       
         1. there is a .data file on DMS, NMS, and there is nothing in ScadaModel.xml part for ProcessVariables (e.g. empty <digitals> tag...)
           -> everything is ok, just apply delta again regularly...
       
        2. there is no .data file on DMS, NMS, and there IS data in ScadaModel.xml part
           -> PROBLEM: ako npr. dodamo 3 prekidaca, a sa strane skade ta tri vec postoje, to treba posmatrati kao UPDATE! 
            znaci iako je u insert operaciji jer u ostatku sistema ne postoji (nema .data), 
            zapravo je update jer na skadi postoji...    
        */

        public bool ApplyDelta(ScadaDelta delta)
        {
            bool retVal = false;

            /*
            prvo proveriti prirodu delte: 
               da li radimo SAMO UPDATE, 
               ili I INSERT i UPDATE, 
               ili samo INSERT

            1. ako nije <samo update> proveri da li uopste imas mesta za one koji su insert, ako nema return false (rejecting whole delta)
            2. ako ima mesta za insert pronadji elemente koji su za update, izvadi ih u zasebnu listu i prvo njih radi, a ostatak radi insert
           */

            List<ScadaElement> updateOperations = delta.UpdateOps;

            // this list acutally can contains update operations also. case 2. above descripted 
            // we have to segregate treal update from insert...
            List<ScadaElement> deltaOperations = delta.InsertOps;
            List<ScadaElement> realInsertOperatins = new List<ScadaElement>();
            List<ScadaElement> realUpdateOperatins = new List<ScadaElement>();

            if (updateOperations.Count != 0)
            {
                realUpdateOperatins = updateOperations;
            }

            bool isOnlyUpdate = true;

            foreach (var el in deltaOperations)
            {
                // if only one element from scadaDelta DOES NOT already exist in db, it means it is for inserting
                var pvs = Database.ProcessVariablesName;
                if (!pvs.ContainsKey(el.Name))
                {
                    // we have some elements for insert
                    isOnlyUpdate = false;
                    break;
                }
            }

            List<RTU> availableRtus = null;

            // temporary storage for elements for adding. key is RTU name, value is PVs mapped to that RTU
            Dictionary<string, List<ProcessVariable>> rtuElementsMap = null;

            // we have some elements in deltaOperatins for insert  
            if (!isOnlyUpdate)
            {
                availableRtus = new List<RTU>();

                availableRtus = GettAllRTUs().Values.Where(r => r.FreeSpaceForDigitals == true ||
                                                            r.FreeSpaceForAnalogs == true).ToList();

                // there is no space for inserting in any RTU, rejecting whole delta
                if (availableRtus.Count == 0)
                {
                    Console.WriteLine("There is no available RTUs for inserting.");
                    return false;
                }
                else
                {
                    // init
                    rtuElementsMap = new Dictionary<string, List<ProcessVariable>>();
                    foreach (var ar in availableRtus)
                    {
                        rtuElementsMap.Add(ar.Name, new List<ProcessVariable>());
                    }

                    // separating elements for update from elements for insert
                    var pvs = Database.ProcessVariablesName;

                    foreach (var el in deltaOperations)
                    {
                        if (!pvs.ContainsKey(el.Name))
                        {
                            realInsertOperatins.Add(el);
                        }
                        else
                        {
                            realUpdateOperatins.Add(el);
                        }
                    }
                    // this list is not necessary anymore
                    deltaOperations.Clear();
                }
            }
            else
            {
                // whole list deltaOperations is actually for update
                realUpdateOperatins = deltaOperations;
            }

            // to do: 
            // processing real updates - ZA SADA SAMO TRUE VRACA!!! i odmah breakuje
            foreach (var updateEl in realUpdateOperatins)
            {
                // TO DO: nekad u buducnosti....
                // napraviti  metodu. da proverava da li su objekti isti? (novi i ovaj koji se dodaje) i koji propertiji nisu...ako 
                //  u sustini treba porediti samo stanja, komande, i trenutno stanje komandu; a za analog novu comm value sa trenutnom u deviceu
                // i onda se proveri ako postoji na kom je rtu-u i update-uje se. 
                //https://stackoverflow.com/questions/2901289/updating-fields-of-values-in-a-concurrentdictionary
                //https://stackoverflow.com/questions/41592129/updating-list-in-concurrentdictionary

                Console.WriteLine("Variable Name = {0} is successfully updated.", updateEl.Name);
                retVal = true;
                break;
            }

            // processing real insertions
            int possibleInsertionsCount = 0;
            foreach (var insertEl in realInsertOperatins)
            {
                bool isInsertingPossible = false;
                ProcessVariable pv;

                if (!GetProcessVariableByName(insertEl.Name, out pv))
                {
                    switch (insertEl.Type)
                    {
                        case DeviceTypes.DIGITAL:

                            if (insertEl.ValidCommands.Count != insertEl.ValidStates.Count)
                            {
                                Console.WriteLine("Element Name = {0} -> ValidCommands.Count!=ValidStates.Count", insertEl.Name);
                                break;
                            }

                            Digital newDigital = new Digital()
                            {
                                Name = insertEl.Name,
                                ValidCommands = insertEl.ValidCommands,
                                ValidStates = insertEl.ValidStates
                            };

                            // pokusaj da nadjes mesta za tu varijablu u bilo kom RTUu
                            // nekad u buducnosti tu se moze neki lep algoritam narpaviti, optimizovati :)
                            foreach (var availableRtu in availableRtus)
                            {
                                // there is no channel with RTU-2 currently
                                // test this case sometime in the future xD
                                if (availableRtu.Name == "RTU-2")
                                    continue;

                                ushort relativeAddress;

                                // kljucno! check if is possible mapping in this rtu
                                if (availableRtu.TryMap(newDigital, out relativeAddress))
                                {
                                    newDigital.RelativeAddress = relativeAddress;
                                    // mapiranje se vrsi u metodi addProcessVariable kasnije,
                                    // preko relativne adrese upravo dodeljene

                                    // Od ovog trenutka se podrazumeva da ce dodavanje uspeti! 
                                    // brojac mapiranih adresa u RTUu je nepovratno promenjen... 
                                    // ako nesto crkne posle, u rollbacku vracamo na staru konfiguraciju citanjem iz fajla
                                    // mozda nije bas najveselije resenje za sada xD

                                    newDigital.ProcContrName = availableRtu.Name;

                                    // podrazumevamo da insertujemo prekidac u dozvoljenom - zeljenom stanju
                                    newDigital.State = States.CLOSED;
                                    newDigital.Command = CommandTypes.CLOSE;

                                    var elements = rtuElementsMap[availableRtu.Name];
                                    elements.Add(newDigital);

                                    possibleInsertionsCount++;

                                    isInsertingPossible = true;
                                    break; // it is possible to insert element
                                }
                            }

                            break;

                        case DeviceTypes.ANALOG:

                            // to do:
                            // provera da li je nova vrednost u dozvoljenom rangeu


                            Analog newAnalog = new Analog() { Name = insertEl.Name };

                            foreach (var availableRtu in availableRtus)
                            {
                                // there is no channel with RTU-2 currently
                                // test this case sometime in the future xD
                                if (availableRtu.Name == "RTU-2")
                                    continue;

                                ushort relativeAddress;

                                // kljucno! if is possible mapping in this rtu? 
                                if (availableRtu.TryMap(newAnalog, out relativeAddress))
                                {
                                    newAnalog.RelativeAddress = relativeAddress;

                                    // Od ovog trenutka se podrazumeva da ce dodavanje uspeti! 

                                    newAnalog.ProcContrName = availableRtu.Name;

                                    newAnalog.AcqValue = insertEl.WorkPoint;
                                    newAnalog.CommValue = insertEl.WorkPoint;

                                    newAnalog.RawBandLow = availableRtu.AnaInRawMin;
                                    newAnalog.RawBandHigh = availableRtu.AnaInRawMax;

                                    string unitSimString = insertEl.UnitSymbol;
                                    UnitSymbol unitSym = (UnitSymbol)Enum.Parse(typeof(UnitSymbol), unitSimString, true);

                                    var elements = rtuElementsMap[availableRtu.Name];

                                    elements.Add(newAnalog);
                                                                   
                                    possibleInsertionsCount++;

                                    isInsertingPossible = true;
                                    break; // it is possible to insert element
                                }
                            }

                            break;

                        default:

                            // all other - not supported yet, will return false
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("ozbiljan error, variabla ne postoji! nije smelo false da se desi :D");
                }

                // if we encounter ProcessVarible that can not be inserted, dont process the rest -> reject whole delta
                if (!isInsertingPossible)
                    break;
            }

            // if we have insertions, and only if it is possible to insert all, otherwise nothing 
            if (realInsertOperatins.Count != 0 && possibleInsertionsCount == realInsertOperatins.Count)
            {
                int addedCount = 0;

                foreach (var rtuElements in rtuElementsMap)
                {
                    var availableRtu = GetRTUByName(rtuElements.Key);
                    var elements = rtuElementsMap[rtuElements.Key];

                    foreach (var elForAdd in elements)
                    {
                        if (availableRtu.MapProcessVariable(elForAdd))
                        {
                            AddProcessVariable(elForAdd);
                            addedCount++;
                            // if is an analog, SEND COMMAND to init sim

                        }
                    }
                }

                if (addedCount == realInsertOperatins.Count)
                {
                    retVal = true;
                }

            }
            else
            {
                // problem -> mappingDig je promenjen nepovratno u tryMap, i Bog zna u kojim sve RTUovima. znaci radi se opet 
                // deserijalizacija ako je false ovde. ovde se radi deserijalizacija iz onog konfig fajla osnovnog
            }

            return retVal;
        }


    }
}
