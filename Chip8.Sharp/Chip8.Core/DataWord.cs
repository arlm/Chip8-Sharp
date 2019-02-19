using System;
using System.Runtime.InteropServices;

namespace Chip8.Core
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct DataWord : IEquatable<DataWord>, IEquatable<ushort>
    {
        [FieldOffset(0)]
        public readonly ushort Word;

        [FieldOffset(0)]
        public readonly byte Lo;

        [FieldOffset(1)]
        public readonly byte Hi;

        public DataWord(ushort word) : this()
        {
            Word = word;
        }

        public override int GetHashCode()
        {
            return (Word, Lo, Hi).GetHashCode();
        }

        public override bool Equals(object obj) => (obj is DataWord opcode) && Equals(opcode);

        public bool Equals(DataWord other) => (Word, Lo, Hi) == (other.Word, other.Lo, other.Hi);

        public bool Equals(ushort other) => Word == other;

        public static bool operator ==(DataWord left, DataWord right) => left.Equals(right);

        public static bool operator !=(DataWord left, DataWord right) => !left.Equals(right);

        public static bool operator ==(DataWord left, ushort right) => left.Equals(right);

        public static bool operator !=(DataWord left, ushort right) => !left.Equals(right);

        public override string ToString()
        {
            return $"0x{Word:X4} (Low-Byte: 0x{Lo:X2}, High-byte: 0x{Hi:X2})";
        }
    }
}