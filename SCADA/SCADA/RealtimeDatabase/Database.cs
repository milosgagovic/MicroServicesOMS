using SCADA.RealtimeDatabase.Model;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SCADA.RealtimeDatabase
{
    public class Database
    {
        public ConcurrentDictionary<string, ProcessVariable> ProcessVariablesName = null;
        public ConcurrentDictionary<string, RTU> RTUs = null;

        private static Database instance;
        private static bool isDataReady;
        private static ReaderWriterLockSlim locker;

        private Database()
        {
            Console.WriteLine("Instancing Database");

            this.ProcessVariablesName = new ConcurrentDictionary<string, ProcessVariable>();
            this.RTUs = new ConcurrentDictionary<string, RTU>();
        }

        public static Database Instance
        {
            get
            {
                if (instance == null)
                {
                    locker = new ReaderWriterLockSlim();
                    instance = new Database();
                }
                return instance;
            }
        }

        public static bool IsConfigurationRunning
        {
            get
            {
                bool retVal = false;

                locker.EnterReadLock();
                retVal = isDataReady;
                locker.ExitReadLock();

                return retVal;
            }
            set
            {
                locker.EnterWriteLock();
                isDataReady = value;
                locker.ExitWriteLock();
            }
        }
    }
}
