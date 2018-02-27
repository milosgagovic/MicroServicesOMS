using PCCommon;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace SCADA.RealtimeDatabase.Model
{
    public class RTU
    {
        private ConcurrentDictionary<ushort, string> PVsAddressAndNames = null;
        private DBContext dbContext = null;

        public IndustryProtocols Protocol { get; set; }

        // (Modbus slave address), value in range 1 - 247 (0 - broadcast)
        public Byte Address { get; set; }

        public string Name { get; set; }

        public bool FreeSpaceForDigitals { get; set; }
        public bool FreeSpaceForAnalogs { get; set; }

        // controller pI/O starting Addresses
        public int DigOutStartAddr { get; set; }
        public int DigInStartAddr { get; set; }
        public int AnaInStartAddr { get; set; }
        public int AnaOutStartAddr { get; set; }
        public int CounterStartAddr { get; set; }

        // number of pI/O
        public int DigOutCount { get; set; }
        public int DigInCount { get; set; }
        public int AnaInCount { get; set; }
        public int AnaOutCount { get; set; }
        public int CounterCount { get; set; }

        // dovoljno nam je samo jedno za sada, posto analog posmatramo da ima isti broj ulaza i izlaza, ali da kasnije nekad moze da se prosiri...
        // raw band limits
        public ushort AnaInRawMin { get; set; }
        public ushort AnaInRawMax { get; set; }
        public ushort AnaOutRawMin { get; set; }
        public ushort AnaOutRawMax { get; set; }

        // to do: add "band" for counter...

        public int MappedDig { get; set; }
        public int MappedAnalog { get; set; }
        public int MappedCounter { get; set; }

        // locks that support single writers and multiple readers
        private ReaderWriterLockSlim digInLock = new ReaderWriterLockSlim();
        private ReaderWriterLockSlim digOutLock = new ReaderWriterLockSlim();

        private ReaderWriterLockSlim anaInLock = new ReaderWriterLockSlim();
        private ReaderWriterLockSlim anaOutLock = new ReaderWriterLockSlim();

        private ReaderWriterLockSlim counterLock = new ReaderWriterLockSlim();

        // List of mapperd reading Addresses
        private List<int> digitalInAddresses;
        private List<int> analogInAddresses;

        // List of mapped commanding Addresses
        private List<int> digitalOutAddresses;
        private List<int> analogOutAddresses;

        private List<int> counterAddresses;

        public RTU()
        {
            this.PVsAddressAndNames = new ConcurrentDictionary<ushort, string>();
            dbContext = new DBContext();

            MappedDig = 0;
            MappedAnalog = 0;
            MappedCounter = 0;

            digitalInAddresses = new List<int>(DigInCount);
            analogInAddresses = new List<int>(AnaInCount);
            digitalOutAddresses = new List<int>(DigOutCount);
            analogOutAddresses = new List<int>(AnaOutCount);
            counterAddresses = new List<int>(CounterCount);
        }

        public int GetAcqAddress(ProcessVariable variable)
        {
            int retAddress = -1;
            switch (variable.Type)
            {
                case VariableTypes.DIGITAL:

                    digInLock.EnterReadLock();
                    retAddress = digitalInAddresses[variable.RelativeAddress];
                    digInLock.ExitReadLock();

                    break;

                case VariableTypes.ANALOG:

                    anaInLock.EnterReadLock();
                    retAddress = analogInAddresses[variable.RelativeAddress];
                    anaInLock.ExitReadLock();

                    break;

                default:
                    break;
            }

            return retAddress;
        }

        public int GetCommandAddress(ProcessVariable variable)
        {
            int retAddress = -1;
            switch (variable.Type)
            {
                case VariableTypes.DIGITAL:

                    digOutLock.EnterReadLock();
                    retAddress = digitalOutAddresses[variable.RelativeAddress];
                    digOutLock.ExitReadLock();
                    break;

                case VariableTypes.ANALOG:

                    anaOutLock.EnterReadLock();
                    retAddress = analogOutAddresses[variable.RelativeAddress];
                    anaOutLock.ExitReadLock();
                    break;

                default:
                    break;
            }
            return retAddress;
        }


        /// <summary>
        /// Mapping to Acquisition address in memory of concrete RTU. It is based
        /// on ProcessVariable RelativeAddress property (offset in array of Process
        /// variables of same type)
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="isSuccessfull"></param>
        /// <returns></returns>
        private int MapToAcqAddress(ProcessVariable variable)
        {
            int retAddr = -1;

            switch (variable.Type)
            {
                case VariableTypes.DIGITAL:
                    Digital digital = variable as Digital;

                    // 1st variable of type Digital, starts on 1st address of DigitalInputs memory (inputs are for acquistion)
                    if (digital.RelativeAddress == 0)
                    {
                        digInLock.EnterWriteLock();
                        try
                        {
                            digitalInAddresses.Insert(digital.RelativeAddress, DigInStartAddr);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine(e.Message);
                        }
                        finally
                        {
                            digInLock.ExitWriteLock();
                        }
                    }

                    digInLock.EnterReadLock();
                    var currentAcqAddress = digitalInAddresses[digital.RelativeAddress];
                    digInLock.ExitReadLock();

                    // this is address that we need currently
                    retAddr = currentAcqAddress;

                    // if we already reached the end of memory for this type of Process Variable
                    // in this Process Controller, than we do not have to calculate nextAddress
                    if (FreeSpaceForDigitals != false)
                    {
                        // calculating address of next variable of same type, 
                        // by adding number of registers (quantity) with starting address of current variable
                        var quantity = (ushort)(Math.Floor((Math.Log(digital.ValidStates.Count, 2))));
                        var nextAddress = currentAcqAddress + quantity;

                        // error, out of range. impossible to insert next variable of same type
                        if (nextAddress >= DigInStartAddr + DigInCount)
                        {
                            FreeSpaceForDigitals = false;
                            break;
                        }

                        digInLock.EnterWriteLock();
                        try
                        {
                            digitalInAddresses.Insert(digital.RelativeAddress + 1, nextAddress);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine(e.Message);
                        }
                        finally
                        {
                            digInLock.ExitWriteLock();
                        }
                    }

                    break;

                case VariableTypes.ANALOG:
                    Analog analog = variable as Analog;

                    // 1st variable of type Analog, starts on 1st address of AnalogInputs memory (inputs are for acquistion)
                    if (analog.RelativeAddress == 0)
                    {
                        anaInLock.EnterWriteLock();
                        try
                        {
                            analogInAddresses.Insert(analog.RelativeAddress, AnaInStartAddr);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine(e.Message);
                        }
                        finally
                        {
                            anaInLock.ExitWriteLock();
                        }
                    }

                    anaInLock.EnterReadLock();
                    // CURRENT ADDRESS ALREADY DEFINED IN THIS SCOPE? O.o
                    currentAcqAddress = analogInAddresses[analog.RelativeAddress];
                    anaInLock.ExitReadLock();

                    // this is address that we need currently
                    retAddr = currentAcqAddress;

                    // if we already reached the end of memory for this type of Process Variable
                    // in this Process Controller, than we do not have to calculate nextAddress
                    if (FreeSpaceForAnalogs != false)
                    {
                        // calculating address of next variable of same type, by adding length of variable
                        // length - num of registers (2,3,4..)
                        var nextAddress = currentAcqAddress + analog.NumOfRegisters;

                        // error, out of range. impossible to insert next variable of same type
                        if (nextAddress >= AnaInStartAddr + AnaInCount)
                        {
                            FreeSpaceForAnalogs = false;
                            break;
                        }

                        anaInLock.EnterWriteLock();
                        try
                        {
                            analogInAddresses.Insert(analog.RelativeAddress + 1, nextAddress);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine(e.Message);
                        }
                        finally
                        {
                            anaInLock.ExitWriteLock();
                        }
                    }
                    break;

                default:
                    break;
            }

            return retAddr;
        }

        /// <summary>
        /// Mapping to Commanding address in memory of concrete RTU. It is based
        /// on ProcessVariable RelativeAddress property (offset in array of Process
        /// variables of same type)
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="isSuccessfull"></param>
        /// <returns></returns>
        private int MapToCommandAddress(ProcessVariable variable)
        {
            int retAddr = -1;

            switch (variable.Type)
            {
                case VariableTypes.DIGITAL:
                    Digital digital = variable as Digital;

                    // 1st variable of type Digital, starts on 1st address of DigitalOutputs memory (outputs are for commanding)
                    if (digital.RelativeAddress == 0)
                    {
                        digOutLock.EnterWriteLock();
                        try
                        {
                            digitalOutAddresses.Insert(digital.RelativeAddress, DigOutStartAddr);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine(e.Message);
                        }
                        finally
                        {
                            digOutLock.ExitWriteLock();
                        }
                    }

                    digOutLock.EnterReadLock();
                    var currentCommAddress = digitalOutAddresses[digital.RelativeAddress];
                    digOutLock.ExitReadLock();

                    // this is address that we need currently
                    retAddr = currentCommAddress;

                    // if we already reached the end of memory for this type of Process Variable
                    // in this Process Controller, than we do not have to calculate nextAddress
                    if (FreeSpaceForDigitals != false)
                    {
                        var quantity = (ushort)(Math.Floor((Math.Log(digital.ValidCommands.Count, 2))));
                        var nextAddress = currentCommAddress + quantity;

                        // error, out of range. impossible to insert next variable of same type
                        if (nextAddress >= DigOutStartAddr + DigInCount)
                        {
                            FreeSpaceForDigitals = false;
                            break;
                        }

                        digOutLock.EnterWriteLock();
                        try
                        {
                            digitalOutAddresses.Insert(digital.RelativeAddress + 1, nextAddress);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine(e.Message);
                        }
                        finally
                        {
                            digOutLock.ExitWriteLock();
                        }
                    }

                    break;


                case VariableTypes.ANALOG:
                    Analog analog = variable as Analog;

                    // 1st variable of type Analog, starts on 1st address of AnalogOutputs memory (outputs are for commanding)
                    if (analog.RelativeAddress == 0)
                    {
                        anaOutLock.EnterWriteLock();
                        try
                        {
                            analogOutAddresses.Insert(analog.RelativeAddress, AnaOutStartAddr);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine(e.Message);
                        }
                        finally
                        {
                            anaOutLock.ExitWriteLock();
                        }
                    }

                    anaOutLock.EnterReadLock();
                    currentCommAddress = analogOutAddresses[analog.RelativeAddress];
                    anaOutLock.ExitReadLock();

                    // this is address that we need currently
                    retAddr = currentCommAddress;

                    // if we already reached the end of memory for this type of Process Variable
                    // in this Process Controller, than we do not have to calculate nextAddress
                    if (FreeSpaceForAnalogs != false)
                    {
                        var nextAddress = currentCommAddress + analog.NumOfRegisters;

                        // error, out of range. impossible to insert next variable of same type
                        if (nextAddress >= AnaOutStartAddr + AnaInCount)
                        {
                            FreeSpaceForAnalogs = false;
                            break;
                        }

                        anaOutLock.EnterWriteLock();
                        try
                        {
                            analogOutAddresses.Insert(analog.RelativeAddress + 1, nextAddress);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine(e.Message);
                        }
                        finally
                        {
                            anaOutLock.ExitWriteLock();
                        }
                    }

                    break;

                default:
                    break;
            }
            return retAddr;
        }


        /// <summary>
        /// Return Process Variable if exists, null if not.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool GetProcessVariableByAddress(ushort address, out ProcessVariable pv)
        {
            string pvName;

            if (PVsAddressAndNames.TryGetValue(address, out pvName))
            {
                //while (!Database.IsConfigurationRunning)
                //    Thread.Sleep(100);
                return (dbContext.GetProcessVariableByName(pvName, out pv));
            }
            else
            {
                pv = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to store variable by its address in RTU Memory.
        /// Mapping to reading/commanding address is done in this function.
        /// </summary>
        /// <param name="variable"></param>
        public bool MapProcessVariable(ProcessVariable variable)
        {
            bool isSuccessfull = false;

            // mapira trenutno insertovanje, i ukoliko je neuspeh za sledece insertovanje, setuje se free space to false
            var readAddr = MapToAcqAddress(variable);
            var writeAddr = MapToCommandAddress(variable);

            if (PVsAddressAndNames.TryAdd((ushort)readAddr, variable.Name)
                && PVsAddressAndNames.TryAdd((ushort)writeAddr, variable.Name))
            {
                isSuccessfull = true;
            }
            else
            {
                Console.WriteLine("JAO xD");
                // ovo ne bi trebalo da se desi xD
            }

            return isSuccessfull;
        }


        // ova funkcija nepovratno menja vrednost mappedDig, mappedAn...
        /// <summary>
        /// Check if it is possible to map new variable, calculates RelativeAddress for 
        /// new variable, based on previously mapped variables.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="relativeAddress"></param>
        /// <returns></returns>
        public bool TryMap(ProcessVariable variable, out ushort relativeAddress)
        {
            bool retVal = false;
            relativeAddress = ushort.MaxValue;

            switch (variable.Type)
            {
                case VariableTypes.DIGITAL:

                    Digital digital = variable as Digital;

                    int desiredDigIn = (ushort)(Math.Floor((Math.Log(digital.ValidStates.Count, 2))));
                    int desiredDigOut = (ushort)(Math.Floor((Math.Log(digital.ValidCommands.Count, 2))));

                    // mozda ovde treba < a ne <=
                    if (MappedDig + desiredDigIn <= DigInCount &&
                        MappedDig + desiredDigOut <= DigOutCount)
                    //if (digitalInAddresses.Count + desiredDigIn <= DigInCount &&
                    //digitalOutAddresses.Count + desiredDigOut <= DigOutCount)
                    {
                        relativeAddress = (ushort)MappedDig;
                        MappedDig++;
                        retVal = true;
                    }
                    break;
                case VariableTypes.ANALOG:
                    Analog analog = variable as Analog;

                    int desiredAnIn = analog.NumOfRegisters;
                    int desiredAnOut = analog.NumOfRegisters;

                    // mozda ovde treba < a ne <=
                    if (MappedAnalog + desiredAnIn <= AnaInCount &&
                        MappedAnalog + desiredAnOut <= AnaOutCount)
                    //if (analogInAddresses.Count + desiredAnIn <= AnaInCount &&
                    //analogOutAddresses.Count + desiredAnOut <= AnaOutCount)
                    {
                        relativeAddress = (ushort)MappedAnalog;
                        MappedAnalog++;
                        retVal = true;
                    }

                    break;
            }
            return retVal;
        }

    }
}
