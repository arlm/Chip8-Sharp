using System;

namespace Chip8.Core
{
    public interface ICPU
    {
        Action<byte[]> OnDraw { get; set; }

        void EmulateCycle();
        void LoadGame(string fileName);
        void SetKeys(byte[] keys);
        void Reset();
    }
}