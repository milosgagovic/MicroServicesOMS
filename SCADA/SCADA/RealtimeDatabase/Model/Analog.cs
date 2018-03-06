using SCADA.RealtimeDatabase.Catalogs;
using System.ComponentModel.DataAnnotations;

namespace SCADA.RealtimeDatabase.Model
{
    public class Analog : ProcessVariable
    {
        public Analog()
        {
            this.Type = VariableTypes.ANALOG;

            // at least one, it will be always 1 in current implementation
            this.NumOfRegisters = 1;

            IsInit = false;
            
            // samo tako zakucano trenutno
            MinValue = 50;
            MaxValue = 500;
        }

        [Key]
        public int Id { get; set; }
        /// <summary>Total number of registers requested (register length = 2 bytes) </summary>
        public ushort NumOfRegisters { get; set; }

        // physical variable is implied by unit
        public UnitSymbol UnitSymbol { get; set; }

        // za sada podrazumevamo da je bez multipliera
        // u buducnosti kada klijent zahteva kasnije u nekoj jedinici, moze da se vrsi pretvaranje...
        // to do: for future implementations...
        // public Multiplier Multiplier { get; set; }

        
        //--------- EGU properties ------------
        // these values persist in configuration file

        /// <summary>Measurement value</summary>
        /// <remarks>This is value that interest SCADA clients (e.g. OMS)</remarks>
        public float AcqValue { get; set; }

        /// <summary>The value ve command</summary>
        /// <remarks>This is value that we want to have initialy on simulator</remarks>
        public float CommValue { get; set; }

        // to do: maybe use these limits for alarm generetion in future implementations. or raw limits.
        // limits to Acq/Comm value   
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        

              
        //----------RAW properties--------------
        // These are the values from the simulators that are not stored in the configuration
        // because their values are based on limits of values (RawBandLow, RawBandHigh) on the simulator, and that range is variable
       
        public ushort RawAcqValue { get; set; }
        public ushort RawCommValue { get; set; }

       
        // limits to Raw Band values
        // to su 4. i 5. kolona za 3. i 4. vrstu u fajlu RtuCfg.txt
        // i vrednosti AnaInRawMin, AnaOutRawMin, AnaInRawMax i AnaOutRawMax u ScadaModel.xml
        // na simulatoru vrednosti idu od 0 do 16000 

        /// <summary>RawBand Min value (AnaInRawMin)</summary>
        /// <remarks>In this implementation, AnaInRawMin and AnaOutRawMin are the same, 
        /// because Analog inputs are mapped 1-1 to Analog outputs</remarks>
        public ushort RawBandLow { get; set; }

        /// <summary>RawBand Max value (AnaInRawMax)</summary>
        /// <remarks>In this implementation, AnaInRawMin and AnaOutRawMin are the same, 
        /// because Analog inputs are mapped 1-1 to Analog outputs</remarks>
        public ushort RawBandHigh { get; set; }
    }
}
