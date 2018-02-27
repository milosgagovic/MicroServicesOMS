using SCADA.RealtimeDatabase.Catalogs;
using SCADA.RealtimeDatabase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.CommunicationAndControlling.SecondaryDataProcessing
{
    public class AnalogProcessor
    {
        // bool da bude, void
        public static Tuple<float, float> GetMinMaxRangeForUnit(ProcessVariable pv, UnitSymbol uSymb)
        {
            Tuple<float, float> range = new Tuple<float, float>(float.MinValue, float.MaxValue);



            return range;
        }

        public static Tuple<float, float> GetAlarmRange(ProcessVariable pv, UnitSymbol uSymb)
        {
            Tuple<float, float> range = new Tuple<float, float>(float.MinValue, float.MaxValue);
            // to do


            return range;
        }

        //  linear conversion
        public static void RawValueToEGU(Analog analog, ushort inputValue, out float result)
        {
            var rawMin = analog.RawBandLow;
            var rawMax = analog.RawBandHigh;

            var rawCurrentAcq = analog.RawAcqValue;
            var rawCurrentComm = analog.RawCommValue;

            var EGUMin = analog.MinValue;
            var EGUMax = analog.MaxValue;

            var temp = ((inputValue - rawMin) * (EGUMax - EGUMin));
            var temp1 = (double)(temp / (rawMax - rawMin));
            result = (float)(Math.Ceiling(temp1) + EGUMin);
        }

        //  linear conversion
        public static void EGUToRawValue(Analog analog)
        {
            var rawMin = analog.RawBandLow;
            var rawMax = analog.RawBandHigh;

            var EGUCurrentAcq = analog.AcqValue;
            var EGUCurrentComm = analog.CommValue;

            var EGUMin = analog.MinValue;
            var EGUMax = analog.MaxValue;

            // sve ce biti celobrojno, i na simulatoru je celobrojno...
            analog.RawAcqValue = (ushort)((((rawMax - rawMin) * (EGUCurrentAcq - EGUMin)) / (EGUMax - EGUMin)) + rawMin);
            analog.RawCommValue = (ushort)((((rawMax - rawMin) * (EGUCurrentComm - EGUMin)) / (EGUMax - EGUMin)) + rawMin);
        }

        /// <summary>
        /// Calculates a Work Point value for setting, in dependence of Analog properties,
        /// returns true if should command to Sim.
        /// </summary>
        /// <param name="analog"></param>
        /// <returns></returns>
        public static bool InitialWorkPointAnalog(Analog analog)
        {
            bool retVal = true;

            // ovde uvek treba da se komanduje za sada, posto na simulatoru NISTA nije inicijalno podeseno... za switcheve smo imali da odgovara ako je 0, ne mora komanda onda
            // mozda nekad kasnije bude neka logika


            return retVal;
        }

        /// <summary>
        /// Check if new work point is different than current work point, returns true if is, otherwise false
        /// </summary>
        /// <param name="analog"></param>
        /// <returns></returns>
        public static bool SetNewWorkPoint(Analog analog, float newWorkPointValue)
        {
            bool retVal = false;

            if (newWorkPointValue != analog.CommValue)
            {
                analog.CommValue = newWorkPointValue;
                retVal = true;
            }
            return retVal;
        }

    }
}
