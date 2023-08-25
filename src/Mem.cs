namespace CPU6502{
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

  public void WriteWord(ushort Value, uint Address, ref uint Cycles) {
    Data[Address] = (byte)(Value & 0xFF);
    Data[Address + 1] = (byte)(Value >> 8);
    Cycles -= 2;
  }
}
}
