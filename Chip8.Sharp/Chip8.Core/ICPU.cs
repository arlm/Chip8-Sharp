using System;

namespace Chip8.Core
{
    public interface ICPU
    {
        double ClockRate { get; }

        double ProcessingTime { get; }

        double FrameRate { get; }

        double RenderingTime { get; }

        Action<byte[]> OnDraw { get; set; }

        Action<int, int, int> OnStartSound { get; set; }

        Action<int> OnEndSound { get; set; }

        void EmulateCycle();

        void LoadGame(string fileName);

        void SetKeys(byte[] keys);

        void Reset();
    }
}