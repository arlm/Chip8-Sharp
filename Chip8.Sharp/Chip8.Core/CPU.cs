using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Chip8.Core
{
    public sealed partial class CPU : ICPU
    {
        public const int WIDTH = 64;
        public const int HEIGHT = 32;
        private const double SMOOTHING = 0.9;

        // The CHIP-8 has 35 opcodes which are all two bytes long.
        internal ushort Opcode;

        // The CHIP-8 has 4K memory in total
        internal byte[] Memory;

        // The CHIP-8 has 16 8-bit general purpose registers named V0, V1 up to VF.
        // Usually referred to as Vx, where x is a hexadecimal digit (0 through F).
        // The 16th register is used  for the ‘carry flag’.
        // The VF register should not be used by any program, 
        // as it is used as a flag by some instructions.
        internal byte[] V;

        // The Index register which can have a value from 0x000 to 0xFFF
        // This register is generally used to store memory addresses, 
        // so only the lowest (rightmost) 12 bits are usually used.
        internal ushort I;

        // Theis program counter which can have a value from 0x000 to 0xFFF
        // It is used to store the currently executing address. 
        internal ushort PC;

        // The systems memory map:
        // 0x000-0x1FF - CHIP-8 interpreter (contains font set in emu)
        // 0x050-0x0A0 - Used for the built in 4x5 pixel font set(0-F)
        // 0x200-0xFFF - Program ROM and work RAM

        // The graphics system: The CHIP-8 has one instruction that draws sprite to the screen.
        // Drawing is done in XOR mode and if a pixel is turned off as a result of drawing, the VF register is set. 
        // This is used for collision detection.

        // The graphics of the CHIP-8 are black and white and the screen has a total of 2048 pixels(64 x 32).
        // This can easily be implemented using an array that hold the pixel state(1 or 0)
        internal MonoChromaticVideoBuffer VideoBuffer;

        // Interupts and hardware registers.
        // The CHIP-8  has none, but there are two timer registers that count at 60 Hz.
        // Delay Timer
        // When set above zero they will count down to zero.
        // When these registers are non-zero, they are automatically decremented at a rate of 60Hz.
        internal byte DT;

        // The system’s buzzer sounds whenever the sound timer reaches zero.
        // Sound Timer
        // When these registers are non-zero, they are automatically decremented at a rate of 60Hz.
        // The sound produced by the CHIP-8 has only one tone. The frequency of this tone is decided by the author.
        internal byte ST;

        // While the specification don’t mention a stack, you will need to implement one as part of the interpreter yourself.
        // The stack is used to remember the current location before a jump is performed.
        // So anytime you perform a jump or call a subroutine, store the program counter in the stack before proceeding.
        // The system has 16 levels of stack and in order to remember which level of the stack is used, you need to implement a stack pointer (sp).
        internal ushort[] Stack;

        // The stack pointer (this.SP) can be 8-bit, it is used to point to the topmost level of the stack.
        internal ushort SP;

        // The CHIP-8 has a HEX based keypad(0x0 - 0xF), you can use an array to store the current state of the key.
        internal byte[] Keys;

        // frame rate
        private DateTime clockDateTime;

        private DateTime frameDateTime;

        private double averageDeltaClockTime;

        private double averageDeltaFrameTime;

        public double ClockRate { get; private set; }

        public double ProcessingTime { get; private set; }

        public double FrameRate { get; private set; }

        public double RenderingTime { get; private set; }

        public Action<byte[]> OnDraw { get; set; }

        public Action<int, int, int> OnStartSound { get; set; }

        public Action<int> OnEndSound { get; set; }

        private Random rand = new Random();

        [SuppressMessage("Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "All disposing opportunities manually checked")]
        private SemaphoreSlim cpuBusy = new SemaphoreSlim(1, 1);

        [SuppressMessage("Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "All disposing opportunities manually checked")]
        private SemaphoreSlim gfxBusy = new SemaphoreSlim(1, 1);

        [SuppressMessage("Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "All disposing opportunities manually checked")]
        private CancellationTokenSource ctsCPU = new CancellationTokenSource();

        [SuppressMessage("Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "All disposing opportunities manually checked")]
        private CancellationTokenSource ctsGraphics = new CancellationTokenSource();

        private bool firstCycle = true;

        // Initialize registers and memory once
        public CPU(uint foregroundColor = 0xFF_FF_FF_FF, uint backgroundColor = 0xFF_00_00_00)
        {
            this.VideoBuffer = new MonoChromaticVideoBuffer(HEIGHT, WIDTH, foregroundColor, backgroundColor);

            this.Reset();
        }

        public void Reset()
        {
            if (this.cpuBusy.CurrentCount == 0)
            {
                this.ctsCPU.Cancel();
                this.cpuBusy.Dispose();
                this.cpuBusy = new SemaphoreSlim(1, 1);
            }

            try
            {
                this.cpuBusy.Wait(this.ctsCPU.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Print("CPU Reset Operation cancelled");
                return;
            }

            try
            {
                // The system expects the application to be loaded at memory location 0x200. 
                this.PC = 0x200;     // Program counter starts at 0x200

                this.Opcode = 0;     // Reset current opcode  
                this.I = 0;          // Reset index register
                this.SP = 0;         // Reset stack pointer

                //Clear this.keys
                this.Keys = new byte[16];

                // Clear display  
                this.VideoBuffer.Clear();

                // Clear stack
                this.Stack = new ushort[16];
                this.SP = 0;

                // Clear registers V0-VF
                this.V = new byte[16];

                // Clear memory
                this.Memory = new byte[0x1000];

                // Load fontset
                Buffer.BlockCopy(CHIP8_FONTSET, 0, this.Memory, 0, CHIP8_FONTSET.Length);

                // Reset timers
                this.DT = 0;
                this.ST = 0;
            }
            finally
            {
                this.cpuBusy.Release();
            }
        }

        public void LoadGame(string fileName)
        {
            // Buffer of 4KiB (4096) minus 0x200 (512 bytes)
            const int MAX_BUFFER_SIZE = 4096 - 0x200;

            using (var stream = File.OpenRead(fileName))
            {
                var buffer = new byte[MAX_BUFFER_SIZE];
                var bufferSize = stream.Read(this.Memory, 0x200, MAX_BUFFER_SIZE);
                Debug.Print($"Loaded {bufferSize.ToString(NumberFormatInfo.CurrentInfo)} bytes");
            }
        }

        public void LoadMemory(byte[] data, int index = 0)
        {
            data.CopyTo(this.Memory, index);

            Buffer.BlockCopy(CHIP8_FONTSET, 0, this.Memory, 0, CHIP8_FONTSET.Length);
        }

        public void EmulateCycle()
        {
            if (this.cpuBusy.CurrentCount == 0)
            {
                return;
            }

            try
            {
                this.cpuBusy.Wait(this.ctsCPU.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Print("CPU Operation cancelled");
                return;
            }

            try
            {
                if (this.firstCycle)
                {
                    this.Draw();
                    this.firstCycle = false;
                }

                var watch = Stopwatch.StartNew();

                // Fetch this.Opcode

                // During this step, the system will fetch one opcode from the memory at the location specified by the program counter (pc).
                // In the emulator, data is stored in an array in which each address contains one byte.
                // As one opcode is 2 bytes long, we will need to fetch two successive bytes and merge them to get the actual opcode.
                this.Opcode = unchecked((ushort)((this.Memory[this.PC] << 8) | this.Memory[this.PC + 1]));

                var op = new Opcode(this.Opcode);

                var vx = this.V[op.X];
                var vy = this.V[op.Y];

                switch (op.Type)
                {
                    // In some cases we can not rely solely on the first four bits to see what the opcode means.
                    // For example, 0x00E0 and 0x00EE both start with 0x00.
                    // In this case we add an additional switch and compare the last four bits:
                    case 0x0:
                        switch (op.KK)
                        {
                            case 0xE0:
                                this.op_CLS();
                                break;

                            case 0xEE:
                                this.op_RET();
                                break;

                            default:
                                this.op_SYS_addr();
                                break;
                        }
                        break;

                    case 0x1:
                        this.op_JP_addr(op);
                        break;

                    case 0x2:
                        this.op_CALL_addr(op);
                        break;

                    case 0x3:
                        this.op_SE_Vx_byte(op, vx);
                        break;

                    case 0x4:
                        this.op_SNE_Vx_byte(op, vx);
                        break;

                    case 0x5:
                        this.op_SE_Vx_Vy(op, vx, vy);
                        break;

                    case 0x6:
                        this.op_LD_Vx_byte(op);
                        break;

                    case 0x7:
                        this.op_ADD_Vx_byte(op);
                        break;

                    case 0x8:
                        {
                            switch (op.N)
                            {
                                case 0x0:
                                    this.op_LD_Vx_Vy(op, vy);
                                    break;

                                case 0x1:
                                    this.op_OR_Vx_Vy(op, vy);
                                    break;

                                case 0x2:
                                    this.op_AND_Vx_Vy(op, vy);
                                    break;

                                case 0x3:
                                    this.op_XOR_Vx_Vy(op, vy);
                                    break;

                                case 0x4:
                                    this.op_ADD_Vx_Vy(op, vx, vy);
                                    break;

                                case 0x5:
                                    this.op_SUB_Vx_Vy(op, vx, vy);
                                    break;

                                case 0x6:
                                    this.op_SHR_Vx_Vy(op, vx);
                                    break;

                                case 0x7:
                                    this.op_SUBN_Vx_Vy(op, vx, vy);
                                    break;

                                case 0xE:
                                    this.op_SHL_Vx_Vy(op, vx);
                                    break;

                                default:
                                    Debug.Print($"Unknown opcode: {this.Opcode.ToString("X4", NumberFormatInfo.CurrentInfo)}");
                                    break;
                            }
                            break;
                        }

                    case 0x9:
                        this.op_SNE_Vx_Vy(op, vx, vy);
                        break;

                    case 0xA:
                        this.op_LD_I_addr(op);
                        break;

                    case 0xB:
                        this.op_JP_V0_addr(op);
                        break;

                    case 0xC:
                        this.op_RND_Vx_byte(op);
                        break;

                    case 0xD:
                        this.op_DRW_Vx_Vy_nibble(op, vx, vy);
                        break;

                    case 0xE:
                        switch (op.KK)
                        {
                            case 0x9E:
                                this.op_SKP_Vx(op, vx);
                                break;

                            case 0xA1:
                                this.op_SKNP_Vx(op, vx);
                                break;

                            default:
                                Debug.Print($"Unknown opcode: {this.Opcode.ToString("X4", NumberFormatInfo.CurrentInfo)}");
                                break;
                        }
                        break;

                    case 0xF:
                        switch (op.KK)
                        {
                            case 0x07:
                                this.op_LD_Vx_DT(op);
                                break;

                            case 0x0A:
                                this.op_LD_Vx_K(op);
                                break;

                            case 0x15:
                                this.op_LD_DT_Vx(op, vx);
                                break;

                            case 0x18:
                                this.op_LD_ST_Vx(op, vx);
                                break;

                            case 0x1E:
                                this.op_ADD_I_Vx(op, vx);
                                break;

                            case 0x29:
                                this.op_LD_F_Vx(op, vx);
                                break;

                            case 0x33:
                                this.op_LD_B_Vx(op, vx);
                                break;

                            case 0x55:
                                this.op_LD_IPtr_Vx(op);
                                break;

                            case 0x65:
                                this.op_LD_Vx_IPtr(op);
                                break;

                            default:
                                Debug.Print($"Unknown opcode: {this.Opcode.ToString("X4", NumberFormatInfo.CurrentInfo)}");
                                break;
                        }
                        break;

                    default:
                        Debug.Print($"Unknown opcode: {this.Opcode.ToString("X4", NumberFormatInfo.CurrentInfo)}");
                        break;
                }

                // Update timers
                if (this.DT > 0)
                {
                    --this.DT;
                }

                if (this.ST > 0)
                {
                    if (this.ST == 1)
                    {
                        this.OnEndSound?.Invoke(0);
                    }

                    --this.ST;
                }

                watch.Stop();
                this.ProcessingTime = (this.ProcessingTime * SMOOTHING) + (watch.ElapsedTicks * (1 - SMOOTHING));

                var currentDateTime = DateTime.Now;
                var currentDeltaTime = (currentDateTime - this.clockDateTime).TotalSeconds;
                this.clockDateTime = currentDateTime;
                this.averageDeltaClockTime = (this.averageDeltaClockTime * SMOOTHING) + (currentDeltaTime * (1 - SMOOTHING));
                this.ClockRate = 1.0 / this.averageDeltaClockTime;

                if (this.VideoBuffer.IsDirty)
                {
                    this.Draw();
                }
            }
            finally
            {
                this.cpuBusy.Release();
            }
        }

        public void SetKeys(byte[] keys)
        {
            if (keys.Length != this.Keys.Length)
            {
                throw new InvalidOperationException($"this.keys should be exactly {this.Keys.Length.ToString(NumberFormatInfo.CurrentInfo)} bytes long.");
            }

            keys.CopyTo(this.Keys, 0);
        }

        public void Dispose()
        {
            this.ctsCPU?.Cancel();
            this.ctsCPU?.Dispose();
            this.cpuBusy?.Dispose();

            this.ctsGraphics?.Cancel();
            this.ctsGraphics?.Dispose();
            this.gfxBusy?.Dispose();
        }

        private void Draw()
        {
            try
            {
                this.gfxBusy.Wait(this.ctsGraphics.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Print("GFX rendering operation cancelled");
                return;
            }

            try
            {
                var watch = Stopwatch.StartNew();

                this.OnDraw?.Invoke(this.VideoBuffer.ToByteArray());

                watch.Stop();
                this.RenderingTime = (this.RenderingTime * SMOOTHING) + (watch.ElapsedTicks * (1 - SMOOTHING));

                var currentDateTime = DateTime.Now;
                var currentDeltaTime = (currentDateTime - this.frameDateTime).TotalSeconds;
                this.frameDateTime = currentDateTime;
                this.averageDeltaFrameTime = (this.averageDeltaFrameTime * SMOOTHING) + (currentDeltaTime * (1 - SMOOTHING));
                this.FrameRate = 1.0 / this.averageDeltaFrameTime;
            }
            finally
            {
                this.gfxBusy.Release();
            }
        }
    }
}
