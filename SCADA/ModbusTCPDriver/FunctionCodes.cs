using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusTCPDriver
{
    // u modbus registarskoj mapi su 4 tipa registara -> input discrete, coils, input registers, holding registers
    // citanje je moguce nad svim registrima, dok je i pisanje moguce nad registrima koji predstavljaju izlaze

    // ulazi -> vrednosti merene velicine, podaci sa senzora. citanje stanja
    // izlazi -> komandovanje
    public enum FunctionCodes : byte
    {
        // citanje nad svim registrima, pisanje samo nad izlazima

        ReadCoils = 1,          // digital output   R
        ReadDiscreteInput,      // digital input    R
        ReadHoldingRegisters,   // analog output    R
        ReadInputRegisters,     // analog input     R

        WriteSingleCoil,        // digital output   RW
        WriteSingleRegister    // analog output     RW
    }
}
