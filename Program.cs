class Mem {
  static uint MAX_MEM = 1024 * 64;
  byte[] Data = new byte[MAX_MEM];

  public Mem() {}

  public void Initialize() {
    Data = new byte[MAX_MEM];
  }

  public byte this[uint address] {
    get => Data[address];
    set => Data[address] = value;
  }
}

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

  public byte ReadByte(ref uint Cycles, byte Address, Mem memory) {
    byte Data = memory[Address];
    Cycles--;
    return Data;
  }

  // opcodes
  public const byte INS_LDA_IM = 0xA9;
  public const byte INS_LDA_ZP = 0xA5;

  public void LDASetStatus() {
    Z = A == 0;
    N = (A & 0b10000000) > 0;
  }

  public void Execute(uint Cycles, Mem memory) {
    while (Cycles > 0) {
      byte Ins = FetchByte(ref Cycles, memory);
      switch (Ins) {
        case INS_LDA_IM:
          byte Value = FetchByte(ref Cycles, memory);
          A = Value;
          LDASetStatus();
          break;
        case INS_LDA_ZP:
          byte ZeroPageAddr = FetchByte(ref Cycles, memory);
          A = ReadByte(ref Cycles, ZeroPageAddr, memory);
          LDASetStatus();
          break;
        default:
          Console.WriteLine("Instruction not handled");
          break;
      }
    }
  }

  public CPU() {}
}

class Program {
  static void Main() {
    Mem mem = new Mem();
    CPU cpu = new CPU();
    cpu.Reset(mem);

    // inline a little program
    mem[0xFFFC] = CPU.INS_LDA_ZP;
    mem[0xFFFD] = 0x42;
    mem[0x0042] = 0x84;

    cpu.Execute(3, mem);

    // Debug
    Console.WriteLine($"A = 0x{cpu.A:X2}");
    Console.WriteLine($"X = 0x{cpu.X:X2}");
    Console.WriteLine($"Y = 0x{cpu.Y:X2}");
  }
}
