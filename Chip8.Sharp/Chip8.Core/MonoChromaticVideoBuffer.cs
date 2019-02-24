using System;

namespace Chip8.Core
{
    public class MonoChromaticVideoBuffer
    {
        private readonly int[] memory;

        private int ForegroundColor { get; set; }

        private int BackgroundColor { get; set; }

        public int Width { get; }

        public int Height { get; }

        public int Length => memory.Length;

        public bool IsDirty { get; private set; } = false;

        public MonoChromaticVideoBuffer(int height, int width)
        {
            this.Width = width;
            this.Height = height;

            var length = width * height;
            this.memory = new int[length];

            this.Clear();
        }

        public void Clear()
        {
            for (int index = 0; index < this.memory.Length; index++)
            {
                this.memory[index] = this.BackgroundColor;
            }

            this.IsDirty = true;
        }

        public bool this[int index]
        {
            get
            {
                if (this.memory.Length > index)
                {
                    return this.memory[index] != this.BackgroundColor;
                }

                throw new InvalidOperationException("Pixel out of buffer scope");
            }

            set
            {
                if (this.memory.Length > index)
                {
                    this.memory[index] = value ? this.ForegroundColor : this.BackgroundColor;
                }
                else
                {
                    throw new InvalidOperationException("Pixel out of buffer scope");
                }
            }
        }

        public bool this[int x, int y]
        {
            get
            {
                var location = x + (y * this.Width);

                if (this.memory.Length > location)
                {
                    return this.memory[location] != this.BackgroundColor;
                }

                throw new InvalidOperationException("Pixel out of buffer scope");
            }

            set
            {
                var location = x + (y * this.Width);

                if (this.memory.Length > location)
                {
                    this.memory[location] = value ? this.ForegroundColor : this.BackgroundColor;
                    this.IsDirty = true;
                }
                else
                {
                    throw new InvalidOperationException("Pixel out of buffer scope");
                }
            }
        }

        public byte[] ToByteArray()
        {
            var result = new byte[this.memory.Length * sizeof(int)];
            Buffer.BlockCopy(this.memory, 0, result, 0, result.Length);
            this.IsDirty = false;

            return result;
        }
    }
}