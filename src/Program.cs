using CPU6502;

class Program {
  static void Main() {
    Tests tests = new Tests();

    tests.RunAllTests();
  }
}

class Tests {
  CPU cpu;
  Mem mem;

  public Tests() {
    mem = new Mem();
    cpu = new CPU();
    cpu.Reset(mem);
  }

  public void RunAllTests() {
    List<string> testsFailed = new List<string>();

    if (!LittleInLineProgram())
      testsFailed.Add("LittleInLineProgram");
    cpu.Reset(mem);

    if (testsFailed.Count > 0) {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"{testsFailed.Count} tests failed. {String.Join(", ", testsFailed.ToArray())}");
    } else {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("All tests passed");
    }

    Console.ResetColor();
  }

  public bool LittleInLineProgram() {
    // inline a little program
    mem[0xFFFC] = CPU.INS_JSR;
    mem[0xFFFD] = 0x42;
    mem[0xFFFE] = 0x42;
    mem[0x4242] = CPU.INS_LDA_IM;
    mem[0x4243] = 0x84;

    cpu.Execute(8, mem);

    // Order Of Operations states that equality is ran before logical and operations
    return cpu.A == 0x84 && cpu.X == 0 && cpu.Y == 0;
  }
}

