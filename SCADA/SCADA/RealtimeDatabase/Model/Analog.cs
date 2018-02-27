using SCADA.RealtimeDatabase.Catalogs;

namespace SCADA.RealtimeDatabase.Model
{
    public class Analog : ProcessVariable
    {
        public Analog()
        {
            this.Type = VariableTypes.ANALOG;

            // at least one, it will be alway 1 in current implementation
            this.NumOfRegisters = 1;

            IsInit = false;
            // zakucala 
            MinValue = 50;
            MaxValue = 500;
        }

        public bool IsInit { get; set; }

        // total number of registers requested (register length = 2 bytes)
        public ushort NumOfRegisters { get; set; }


        // physical variable is implied by unit
        public UnitSymbol UnitSymbol { get; set; }

        // podrazumevamo da je bez multipliera
        // kada klijent zahteva kasnije u nekoj jedinici, moze da se vrsi pretvaranje...
        //public Multiplier Multiplier { get; set; }

        //--------- EGU properties ------------

        // Stanje na skadi je uvek i uvek osnovnoj jedinici i bezMultipliera! u zavisnosti od toga
        // ovo je vrednost koja zanima skada klijenta
        public float AcqValue { get; set; }

        // OVO JE ONA VREDNOST KOJU MI ZELIMO DA IMAMO INCIJALNO!
        public float CommValue { get; set; }

        // limits to Acq/Comm value
        public float MaxValue { get; set; }
        public float MinValue { get; set; }

        //----------RAW properties--------------

        // ovo je vrednost sa simulatora! nista nam ne znaci dok je ne konvertujemo u AcqValue i CommValue
        // na simulatoru vrednosti idu od 0 do 4095
        public ushort RawAcqValue { get; set; }
        public ushort RawCommValue { get; set; }

        // limits to Raw Band values
        public ushort RawBandLow { get; set; }
        public ushort RawBandHigh { get; set; }
    }
}
