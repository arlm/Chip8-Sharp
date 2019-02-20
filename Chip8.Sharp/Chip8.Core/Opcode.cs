using System;
using System.Globalization;

namespace Chip8.Core
{

    internal readonly struct Opcode : IEquatable<Opcode>, IEquatable<DataWord>
    {
        private readonly DataWord opCode;
        private readonly byte nn;
        private readonly ushort nnn;
        private readonly byte n;
        private readonly byte x;
        private readonly byte y;
        private readonly byte type;

        public Opcode(ushort opcode)
        {
            this.opCode = new DataWord(opcode);

            nn = this.opCode.Lo;
            nnn = unchecked((ushort)(this.opCode.Word & 0x0FFF));
            n = unchecked((byte)(this.opCode.Lo & 0x0F));
            x = unchecked((byte)(this.opCode.Hi & 0x0F));
            y = unchecked((byte)((this.opCode.Lo & 0xF0) >> 4));
            type = unchecked((byte)((this.opCode.Hi & 0xF0) >> 4));
        }

        public DataWord OpCode => opCode;

        public byte NN => nn;
        public ushort NNN => nnn;
        public byte N => n; 
        public byte X => x;
        public byte Y => y;
        public byte Type => type;

        public override int GetHashCode()
        {
            return (opCode, nn, nnn, n, x, y, type).GetHashCode();
        }

        public override bool Equals(object obj) => (obj is Opcode opcode) && Equals(opcode);

        public bool Equals(Opcode other) =>
                    (opCode, nn, nnn, n, x, y, type) == (other.opCode, other.nn, other.nnn, other.n, other.y, other.y, other.type);

        public bool Equals(DataWord other) => opCode == other;

        public static bool operator ==(Opcode left, Opcode right) => left.Equals(right);

        public static bool operator !=(Opcode left, Opcode right) => !left.Equals(right);

        public static bool operator ==(Opcode left, DataWord right) => left.Equals(right);

        public static bool operator !=(Opcode left, DataWord right) => !left.Equals(right);

        public override string ToString()
        {
            return $"0x{OpCode.Word.ToString("X4", NumberFormatInfo.CurrentInfo)} (X: 0x{X.ToString("X1", NumberFormatInfo.CurrentInfo)}, Y: 0x{Y.ToString("X1", NumberFormatInfo.CurrentInfo)}, N: 0x{N.ToString("X1", NumberFormatInfo.CurrentInfo)}, NN: 0x{NN.ToString("X2", NumberFormatInfo.CurrentInfo)}, NNN: 0x{NNN.ToString("X3", NumberFormatInfo.CurrentInfo)} Type: 0x{Type.ToString("X1", NumberFormatInfo.CurrentInfo)})";
        }
    }
}