namespace CPU6502 {
class CPU {
  public ushort PC; // Program Counter
  public ushort SP; // Stack Counter

  // Registers
  public byte A;
  public byte X;
  public byte Y;

  // Status Flags
  public bool C;
  public bool Z;
  public bool I;
  public bool D;
  public bool B;
  public bool V;
  public bool N;

  public void Reset(Mem memory) {
    PC = 0xFFFC;
    SP = 0x0100;
    C = Z = I = D = B = V = N = false;
    A = X = Y = 0;
    memory.Initialize();
  }

  public byte FetchByte(ref uint Cycles, Mem memory) {
    byte Data = memory[PC];
    PC++;
    Cycles--;
    return Data;
  }

  public ushort FetchWord(ref uint Cycles, Mem memory) {
    // 6502 is little endian
    ushort Data = memory[PC];
    PC++;

    Data |= (ushort)(memory[PC] << 8);
    PC++;

    Cycles -= 2;

    /*// If you wanted to handle endianness
    // You would need to swap bytes here
    if ( PLATFORM_BIG_ENDIAN)
      SwapBytesInWord(Data);
    */

    return Data;
  }

  public byte ReadByte(ref uint Cycles, byte Address, Mem memory) {
    byte Data = memory[Address];
    Cycles--;
    return Data;
  }

  // opcodes
  public const byte INS_LDA_IM  = 0xA9;
  public const byte INS_LDA_ZP  = 0xA5;
  public const byte INS_LDA_ZPX = 0xB5;
  public const byte INS_JSR     = 0x20;

  public void LDASetStatus() {
    Z = A == 0;
    N = (A & 0b10000000) > 0;
  }

  /// <returns>The Cycles actually executed</returns>
  public int Execute(uint Cycles, Mem memory) {
    uint CyclesRequested = Cycles;

    while (Cycles > 0) {
      // Console.Write($"Reading from addr 0x{PC:X4}, ");
      byte Ins = FetchByte(ref Cycles, memory);
      // Console.WriteLine($"Ins = 0x{Ins:X2}");
      switch (Ins) {
        case INS_LDA_IM: {
          byte Value = FetchByte(ref Cycles, memory);
          A = Value;
          LDASetStatus();
          break;
        }
        case INS_LDA_ZP: {
          byte ZeroPageAddr = FetchByte(ref Cycles, memory);
          A = ReadByte(ref Cycles, ZeroPageAddr, memory);
          LDASetStatus();
          break;
        }
        case INS_LDA_ZPX: {
          byte ZeroPageAddr = FetchByte(ref Cycles, memory);
          ZeroPageAddr += X;
          Cycles--;
          A = ReadByte(ref Cycles, ZeroPageAddr, memory);
          LDASetStatus();
          break;
        }
        case INS_JSR: {
          ushort SubAddr = FetchWord(ref Cycles, memory);
          memory.WriteWord((ushort)(PC - 1), (ushort)SP, ref Cycles);
          SP += 2;
          PC = SubAddr;
          Cycles--;
          break;
        }
        default: {
          Console.WriteLine($"Instruction not handled, recieved instruction {Ins:X2}");
          break;
        }
      }
    }

    return (int)CyclesRequested - (int)Cycles;
  }

  public CPU() {}
}

}
