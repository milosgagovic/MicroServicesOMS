using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.RealtimeDatabase.Catalogs
{
    /// only subset of UnitSymbol.cs from Common 
    public enum UnitSymbol : short
    {
        /// <summary>
        /// volt-amper, complex power S, apparent power |S|
        /// </summary>
        VA = 0,

        /// <summary>
        /// watt, unit of active (real) power P. Quanitfying the rate of energy transfer
        /// </summary>
        W = 1,

        /// <summary>
        /// volt-ampere rective, reactive power Q
        /// </summary>
        VAr = 2,

        /// <summary>
        /// volt-ampere hours (apparent energy)
        /// </summary>
        VAh = 3,

        /// <summary>
        ///  watt-hour, does useful work (paying for it)
        /// </summary>
        Wh = 4,

        /// <summary>
        /// volt-ampere-reactive hours  reactive energy, charge capacitors and inductos and does no work
        /// </summary>
        VArh = 5,
        V = 6

    }

    public enum Multiplier
    {
        noMultiplier, // -> default, 10^0 

        m, // milli -> 10^(-3)
        k, // kilo -> 10^3
        M  // Meha -> 10^6        
    }
}
