using System;

namespace FTN.Common
{
    public enum TransactionAnswer : short
    {
        Prepared = 1,
        Unprepared = 2,
        Unanswered = 3
    }
    public enum TypeOfSCADACommand : short
    {
        ReadAll = 1,
        WriteDigital = 2,
        WriteAnalog = 3
    }
    public enum PhaseCode : short
    {
        Unknown = 0x0,
        N = 0x1,
        C = 0x2,
        CN = 0x3,
        B = 0x4,
        BN = 0x5,
        BC = 0x6,
        BCN = 0x7,
        A = 0x8,
        AN = 0x9,
        AC = 0xA,
        ACN = 0xB,
        AB = 0xC,
        ABN = 0xD,
        ABC = 0xE,
        ABCN = 0xF
    }

    public enum TransformerFunction : short
    {
        Supply = 1,             // Supply transformer
        Consumer = 2,           // Transformer supplying a consumer
        Grounding = 3,          // Transformer used only for grounding of network neutral
        Voltreg = 4,            // Feeder voltage regulator
        Step = 5,               // Step
        Generator = 6,          // Step-up transformer next to a generator.
        Transmission = 7,       // HV/HV transformer within transmission network.
        Interconnection = 8     // HV/HV transformer linking transmission network with other transmission networks.
    }

    public enum WindingConnection : short
    {
        Y = 1,      // Wye
        D = 2,      // Delta
        Z = 3,      // ZigZag
        I = 4,      // Single-phase connection. Phase-to-phase or phase-to-ground is determined by elements' phase attribute.
        Scott = 5,   // Scott T-connection. The primary winding is 2-phase, split in 8.66:1 ratio
        OY = 6,     // 2-phase open wye. Not used in Network Model, only as result of Topology Analysis.
        OD = 7      // 2-phase open delta. Not used in Network Model, only as result of Topology Analysis.
    }

    public enum WindingType : short
    {
        None = 0,
        Primary = 1,
        Secondary = 2,
        Tertiary = 3
    }
    public enum UnitSymbol : short
    {
        VA = 0,
        W = 1,
        VAr = 2,
        VAh = 3,
        Wh = 4,
        VArh = 5,
        V = 6,
        ohm = 7,
        A = 8,
        F = 9,
        H = 10,
        degC = 11,
        s = 12,
        min = 13,
        h = 14,
        deg = 15,
        rad = 16,
        J = 17,
        N = 18,
        S = 19,
        none = 20,
        Hz = 21,
        g = 22,
        Pa = 23,
        m = 24,
        m2 = 25,
        m3 = 26
    }

    public enum DirectionType : short
    {
        Read = 0,
        Write = 1,
        ReadWrite = 2
    }

    public enum Commands : short
    {
        OPEN = 0,
        CLOSE = 1
    }

    public enum States : short
    {
        OPENED = 0,
        CLOSED = 1
    }
}
