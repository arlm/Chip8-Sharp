using System;

namespace Chip8.Core
{

    internal readonly struct Opcode : IEquatable<Opcode>, IEquatable<DataWord>
    {
        private readonly DataWord opcode;
        private readonly byte nn;
        private readonly ushort nnn;
        private readonly byte n;
        private readonly byte x;
        private readonly byte y;
        private readonly byte type;

        public Opcode(ushort opcode)
        {
            this.opcode = new DataWord(opcode);

            nn = this.opcode.Lo;
            nnn = unchecked((ushort)(this.opcode.Word & 0x0FFF));
            n = unchecked((byte)(this.opcode.Lo & 0x0F));
            x = unchecked((byte)(this.opcode.Hi & 0x0F));
            y = unchecked((byte)((this.opcode.Lo & 0xF0) >> 4));
            type = unchecked((byte)((this.opcode.Hi & 0xF0) >> 4));
        }

        public DataWord OpCode => opcode;

        public byte NN => nn;
        public ushort NNN => nnn;
        public byte N => n; 
        public byte X => x;
        public byte Y => y;
        public byte Type => type;

        public override int GetHashCode()
        {
            return (opcode, nn, nnn, n, x, y, type).GetHashCode();
        }

        public override bool Equals(object obj) => (obj is Opcode opcode) && Equals(opcode);

        public bool Equals(Opcode other) =>
                    (opcode, nn, nnn, n, x, y, type) == (other.opcode, other.nn, other.nnn, other.n, other.y, other.y, other.type);

        public bool Equals(DataWord other) => opcode == other;

        public static bool operator ==(Opcode left, Opcode right) => left.Equals(right);

        public static bool operator !=(Opcode left, Opcode right) => !left.Equals(right);

        public static bool operator ==(Opcode left, DataWord right) => left.Equals(right);

        public static bool operator !=(Opcode left, DataWord right) => !left.Equals(right);

        public override string ToString()
        {
            return $"0x{OpCode.Word:X4} (X: 0x{X:X1}, Y: 0x{Y:X1}, N: 0x{N:X1}, NN: 0x{NN:X2}, NNN: 0x{NNN:X3} Type: 0x{Type:X1})";
        }
    }
}