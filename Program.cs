using CPU6502;

class Program {
  static void Main() {
    Mem mem = new Mem();
    CPU cpu = new CPU();
    cpu.Reset(mem);

    // inline a little program
    mem[0xFFFC] = CPU.INS_JSR;
    mem[0xFFFD] = 0x42;
    mem[0xFFFE] = 0x42;
    mem[0x4242] = CPU.INS_LDA_IM;
    mem[0x4243] = 0x84;

    cpu.Execute(8, mem);

    // Debug
    Console.WriteLine($"A = 0x{cpu.A:X2}");
    Console.WriteLine($"X = 0x{cpu.X:X2}");
    Console.WriteLine($"Y = 0x{cpu.Y:X2}");
    Console.ReadLine();
  }
}
