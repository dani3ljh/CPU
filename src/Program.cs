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

    if (!TheCPUDoesNothingWhenWeExecuteZeroCycles())
      testsFailed.Add("TheCPUDoesNothingWhenWeExecuteZeroCycles");
    cpu.Reset(mem);

    if (!CPUCanExecuateMoreCyclesThanRequestedIfRequiredByInstruction())
      testsFailed.Add("CPUCanExecuateMoreCyclesThanRequestedIfRequiredByInstruction");
    cpu.Reset(mem);

    if (!LDAImmediateCanLoadValueIntoTheARegister())
      testsFailed.Add("LDAImmediateCanLoadValueIntoTheARegister");
    cpu.Reset(mem);

    if (!LDAZeroPageCanLoadValueIntoTheARegister())
      testsFailed.Add("LDAZeroPageCanLoadValueIntoTheARegister");
    cpu.Reset(mem);
    
    if (!LDAZeroPageXCanLoadValueIntoTheARegister())
      testsFailed.Add("LDAZeroPageXCanLoadValueIntoTheARegister");
    cpu.Reset(mem);

    if (!LDAZeroPageXCanLoadValueIntoTheARegisterWhenItWraps())
      testsFailed.Add("LDAZeroPageXCanLoadValueIntoTheARegisterWhenItWraps");
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

  public bool VerifyUnModifiedFlagsFromLDA(CPU cpu) {
    return (
        !cpu.C &&
        !cpu.I &&
        !cpu.D &&
        !cpu.B &&
        !cpu.V
    );
  }

  public bool TheCPUDoesNothingWhenWeExecuteZeroCycles() {
    // given:
    const int NUM_CYCLES = 0;

    // when:
    int CyclesUsed = cpu.Execute(NUM_CYCLES, mem);

    // then:
    return (
      CyclesUsed == 0
    );
  }

  public bool CPUCanExecuateMoreCyclesThanRequestedIfRequiredByInstruction() {
    // given:
    mem[0xFFFC] = CPU.INS_LDA_IM;
    mem[0xFFFD] = 0x84;

    // when:
    int CyclesUsed = cpu.Execute(1, mem);

    // then:
    return (
      CyclesUsed == 2
    );
  }

  public bool LDAImmediateCanLoadValueIntoTheARegister() {
    // given:
    mem[0xFFFC] = CPU.INS_LDA_IM;
    mem[0xFFFD] = 0x84;

    // when:
    int CyclesUsed = cpu.Execute(2, mem);

    // then:
    return (
      cpu.A == 0x84 &&
      CyclesUsed == 2 &&
      !cpu.Z &&
      cpu.N &&
      VerifyUnModifiedFlagsFromLDA(cpu)
    );
  }

  public bool LDAZeroPageCanLoadValueIntoTheARegister() {
    // given:
    mem[0xFFFC] = CPU.INS_LDA_ZP;
    mem[0xFFFD] = 0x42;
    mem[0x0042] = 0x37;

    // when:
    int CyclesUsed = cpu.Execute(3, mem);

    // then:
    return (
      cpu.A == 0x37 && 
      CyclesUsed == 3 &&
			!cpu.Z && 
			!cpu.N &&
      VerifyUnModifiedFlagsFromLDA(cpu)
    );
  }

  public bool LDAZeroPageXCanLoadValueIntoTheARegister() {
    // given:
    cpu.X = 0x05;
    mem[0xFFFC] = CPU.INS_LDA_ZPX;
    mem[0xFFFD] = 0x42;
    mem[0x0047] = 0x37;

    // when:
    int CyclesUsed = cpu.Execute(4, mem);

    // then:
    return (
      cpu.A == 0x37 &&
      CyclesUsed == 4 &&
		  !cpu.Z &&
		  !cpu.N &&
      VerifyUnModifiedFlagsFromLDA(cpu)
    );
  }

  public bool LDAZeroPageXCanLoadValueIntoTheARegisterWhenItWraps() {
    // given:
    cpu.X = 0xFF;
    mem[0xFFFC] = CPU.INS_LDA_ZPX;
    mem[0xFFFD] = 0x80;
    mem[0x007F] = 0x37;

    // when:
    int CyclesUsed = cpu.Execute(4, mem);

    // then:
    return (
      cpu.A == 0x37 && 
      CyclesUsed == 4 &&
			!cpu.Z && 
			!cpu.N &&
      VerifyUnModifiedFlagsFromLDA(cpu)
    );
  }
}

