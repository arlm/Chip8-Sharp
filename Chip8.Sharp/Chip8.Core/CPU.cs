using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Chip8.Core
{
    public partial class CPU
    {
        public const int WIDTH = 64;
        public const int HEIGHT = 32;

        // The CHIP-8 has 35 opcodes which are all two bytes long.
        internal ushort opcode;

        // The CHIP-8 has 4K memory in total
        internal byte[] memory;

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
        internal byte[] gfx;


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
        internal ushort[] stack;

        // The stack pointer (SP) can be 8-bit, it is used to point to the topmost level of the stack.
        internal ushort SP;

        // The CHIP-8 has a HEX based keypad(0x0 - 0xF), you can use an array to store the current state of the key.
        internal byte[] keys;

        public bool DrawFlag { get; set; }

        public byte[] Graphics => gfx;

        Random rand = new Random();

        // Initialize registers and memory once
        public CPU()
        {
            Reset();
        }

        public void Reset()
        {
            // The system expects the application to be loaded at memory location 0x200. 
            PC = 0x200;     // Program counter starts at 0x200

            opcode = 0;     // Reset current opcode  
            I = 0;          // Reset index register
            SP = 0;         // Reset stack pointer

            //Clear keys
            keys = new byte[16];

            // Clear display  
            gfx = new byte[WIDTH * HEIGHT];
            DrawFlag = false;

            // Clear stack
            stack = new ushort[16];
            SP = 0;

            // Clear registers V0-VF
            V = new byte[16];

            // Clear memory
            memory = new byte[0x1000];

            // Load fontset
            Buffer.BlockCopy(CHIP8_FONTSET, 0, memory, 0, CHIP8_FONTSET.Length);

            // Reset timers
            DT = 0;
            ST = 0;
        }

        public void LoadGame(string fileName)
        {
            // Buffer of 4KiB (4096) minus 0x200 (512 bytes)
            const int MAX_BUFFER_SIZE = 4096 - 0x200;

            using (var stream = File.OpenRead(fileName))
            {
                var buffer = new byte[MAX_BUFFER_SIZE];
                var bufferSize = stream.Read(memory, 0x200, MAX_BUFFER_SIZE);
                Console.WriteLine($"Loaded {bufferSize} bytes");
            }
        }

        public void LoadMemory(byte[] data, int index = 0)
        {
            data.CopyTo(this.memory, index);

            Buffer.BlockCopy(CHIP8_FONTSET, 0, memory, 0, CHIP8_FONTSET.Length);
        }

        public void EmulateCycle()
        {
            // Fetch Opcode

            // During this step, the system will fetch one opcode from the memory at the location specified by the program counter (pc).
            // In the emulator, data is stored in an array in which each address contains one byte.
            // As one opcode is 2 bytes long, we will need to fetch two successive bytes and merge them to get the actual opcode.
            opcode = unchecked((ushort)((memory[PC] << 8) | memory[PC + 1]));

            // Decode Opcode
            int x = (opcode & 0x0F00) >> 8;
            int y = (opcode & 0x00F0) >> 4;

            byte vx = V[x];
            byte vy = V[y];

            byte nn = unchecked((byte)(opcode & 0x00FF));
            ushort nnn = unchecked((ushort)(opcode & 0x0FFF));

            switch (opcode & 0xF000)
            {
                // In some cases we can not rely solely on the first four bits to see what the opcode means.
                // For example, 0x00E0 and 0x00EE both start with 0x00.
                // In this case we add an additional switch and compare the last four bits:
                case 0x0000:
                    switch (nn)
                    {
                        // 0x00E0: Clears the screen
                        case 0x00E0:
                            gfx = new byte[WIDTH * HEIGHT];

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            PC += 2;
                            break;

                        // 0x00EE: Returns from subroutine
                        case 0x0EE:
                            if (SP == 0)
                            {
                                string message = "Illegal return operation, stack is empty.";
                                Console.WriteLine(message);
                                throw new StackOverflowException(message);
                            }

                            PC = stack[SP - 1];
                            SP--;

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            PC += 2;

                            break;

                        default:
                            {
                                string message = $"Illegal call to RCA 1802 program: 0x{opcode:X4}";
                                Console.WriteLine(message);
                                throw new InvalidOperationException(message);
                            }
                    }
                    break;

                // 0x1NNN: Jumps to address NNN.
                case 0x1000:
                    // Now that we have stored the program counter, we can set it to the address NNN.
                    // Remember, because we’re calling a subroutine at a specific address, 
                    // you should not increase the program counter by two.

                    if (nnn < 0x200)
                    {
                        string message = $"Illegal jump target: 0x{opcode:X4}";
                        Console.WriteLine(message);
                        throw new InvalidOperationException(message);
                    }

                    PC = nnn;
                    break;

                // 0x2NNN: This opcode calls the subroutine at address NNN.
                case 0x2000:
                    if (SP > 0xF)
                    {
                        string message = "Illegal call operation, stack is full.";
                        Console.WriteLine(message);
                        throw new StackOverflowException(message);
                    }

                    // Because we will need to temporary jump to address NNN,
                    // it means that we should store the current address of the program counter in the stack.
                    stack[SP] = PC;

                    //  After storing the value of the program counter in the stack,
                    // increase the stack pointer to prevent overwriting the current stack. 
                    ++SP;

                    // Now that we have stored the program counter, we can set it to the address NNN.
                    // Remember, because we’re calling a subroutine at a specific address, 
                    // you should not increase the program counter by two.
                    PC = nnn;
                    break;

                // 0x3XNN: Skips the next instruction if VX equals NN.
                // (Usually the next instruction is a jump to skip a code block)    
                case 0x3000:
                    if (vx == nn)
                    {
                        // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                        // This is true unless you jump to a certain address in the memory or if you call a subroutine
                        // (in which case you need to store the program counter in the stack).
                        // If the next opcode should be skipped, increase the program counter by four.
                        PC += 4;
                    }
                    else
                    {
                        // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                        // This is true unless you jump to a certain address in the memory or if you call a subroutine
                        // (in which case you need to store the program counter in the stack).
                        // If the next opcode should be skipped, increase the program counter by four.
                        PC += 2;
                    }
                    break;

                // 0x4XNN: Skips the next instruction if VX doesn't equal NN.
                // (Usually the next instruction is a jump to skip a code block)    
                case 0x4000:
                    if (vx != nn)
                    {
                        // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                        // This is true unless you jump to a certain address in the memory or if you call a subroutine
                        // (in which case you need to store the program counter in the stack).
                        // If the next opcode should be skipped, increase the program counter by four.
                        PC += 4;
                    }
                    else
                    {
                        // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                        // This is true unless you jump to a certain address in the memory or if you call a subroutine
                        // (in which case you need to store the program counter in the stack).
                        // If the next opcode should be skipped, increase the program counter by four.
                        PC += 2;
                    }
                    break;

                // 0x5XY0: Skips the next instruction if VX equals VY.
                // (Usually the next instruction is a jump to skip a code block)    
                case 0x5000:
                    if (x > 0xF)
                    {
                        throw new InvalidOperationException("SE Vx, Vy operation should not use X bigger than 0xF");
                    }

                    if (y > 0xF)
                    {
                        throw new InvalidOperationException("LD Vx, Vy operation should not use Y bigger than 0xF");
                    }

                    if (vx == vy)
                    {
                        // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                        // This is true unless you jump to a certain address in the memory or if you call a subroutine
                        // (in which case you need to store the program counter in the stack).
                        // If the next opcode should be skipped, increase the program counter by four.
                        PC += 4;
                    }
                    else
                    {
                        // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                        // This is true unless you jump to a certain address in the memory or if you call a subroutine
                        // (in which case you need to store the program counter in the stack).
                        // If the next opcode should be skipped, increase the program counter by four.
                        PC += 2;
                    }
                    break;

                // 0x6XNN: Sets VX to NN.
                case 0x6000:
                    if (x > 0xF)
                    {
                        throw new InvalidOperationException("LD Vx, byte operation should not use X bigger than 0xF");
                    }

                    V[x] = nn;

                    // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                    // This is true unless you jump to a certain address in the memory or if you call a subroutine
                    // (in which case you need to store the program counter in the stack).
                    // If the next opcode should be skipped, increase the program counter by four.
                    PC += 2;
                    break;

                // 0x7XNN: Adds NN to VX. (Carry flag is not changed)
                case 0x7000:
                    if (x > 0xF)
                    {
                        throw new InvalidOperationException("ADD Vx, byte operation should not use X bigger than 0xF");
                    }

                    V[x] += nn;

                    // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                    // This is true unless you jump to a certain address in the memory or if you call a subroutine
                    // (in which case you need to store the program counter in the stack).
                    // If the next opcode should be skipped, increase the program counter by four.
                    PC += 2;
                    break;

                case 0x8000:
                    {
                        switch (opcode & 0x000F)
                        {
                            // 0x8XY0: Sets VX to the value of VY.
                            case 0x0000:
                                if (x > 0xF)
                                {
                                    throw new InvalidOperationException("LD Vx, Vy operation should not use X bigger than 0xF");
                                }

                                if (y > 0xF)
                                {
                                    throw new InvalidOperationException("LD Vx, Vy operation should not use Y bigger than 0xF");
                                }

                                V[x] = vy;

                                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                                // (in which case you need to store the program counter in the stack).
                                // If the next opcode should be skipped, increase the program counter by four.
                                PC += 2;
                                break;

                            // 0x8XY1: Sets VX to VX or VY. (Bitwise OR operation)
                            case 0x0001:
                                if (x > 0xF)
                                {
                                    throw new InvalidOperationException("OR Vx, Vy operation should not use X bigger than 0xF");
                                }

                                if (y > 0xF)
                                {
                                    throw new InvalidOperationException("OR Vx, Vy operation should not use Y bigger than 0xF");
                                }

                                V[x] |= vy;

                                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                                // (in which case you need to store the program counter in the stack).
                                // If the next opcode should be skipped, increase the program counter by four.
                                PC += 2;
                                break;

                            // 0x8XY2: Sets VX to VX and VY. (Bitwise AND operation)
                            case 0x0002:
                                if (x > 0xF)
                                {
                                    throw new InvalidOperationException("AND Vx, Vy operation should not use X bigger than 0xF");
                                }

                                if (y > 0xF)
                                {
                                    throw new InvalidOperationException("AND Vx, Vy operation should not use Y bigger than 0xF");
                                }


                                V[x] &= vy;

                                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                                // (in which case you need to store the program counter in the stack).
                                // If the next opcode should be skipped, increase the program counter by four.
                                PC += 2;
                                break;

                            // 0x8XY3: Sets VX to VX xor VY.
                            case 0x0003:
                                if (x > 0xF)
                                {
                                    throw new InvalidOperationException("XOR Vx, Vy operation should not use X bigger than 0xF");
                                }

                                if (y > 0xF)
                                {
                                    throw new InvalidOperationException("XOR Vx, Vy operation should not use Y bigger than 0xF");
                                }

                                V[x] ^= vy;

                                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                                // (in which case you need to store the program counter in the stack).
                                // If the next opcode should be skipped, increase the program counter by four.
                                PC += 2;
                                break;

                            // 0x8XY4: This opcode adds the value of VY to VX.
                            // Register VF is set to 1 when there is a carry and set to 0 when there isn’t.
                            case 0x0004:
                                if (x >= 0xF)
                                {
                                    throw new InvalidOperationException("ADD Vx, Vy operation should not use X as 0xF or bigger (the carry flag register)");
                                }

                                if (y >= 0xF)
                                {
                                    throw new InvalidOperationException("ADD Vx, Vy operation should not use Y as 0xF or bigger (the carry flag register)");
                                }

                                if (vy > (0xFF - vx))
                                {
                                    V[x] += vy;

                                    // Carry
                                    // Because the register can only store values from 0 to 255 (8 bit value),
                                    // it means that if the sum of VX and VY is larger than 255,
                                    // it can’t be stored in the register (or actually it starts counting from 0 again).
                                    // If the sum of VX and VY is larger than 255, 
                                    // we use the carry flag to let the system know that the total sum of both values was indeed larger than 255. 
                                    V[0xF] = 1;
                                }
                                else
                                {
                                    V[x] += vy;

                                    V[0xF] = 0;
                                }

                                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                                // (in which case you need to store the program counter in the stack).
                                // If the next opcode should be skipped, increase the program counter by four.
                                PC += 2;
                                break;

                            // 0x8XY5: VY is subtracted from VX. VF is set to 0 when
                            // there's a borrow, and 1 when there isn't. 
                            case 0x0005:
                                if (x >= 0xF)
                                {
                                    throw new InvalidOperationException("SUB Vx, Vy operation should not use X as 0xF or bigger (the carry flag register)");
                                }

                                if (y >= 0xF)
                                {
                                    throw new InvalidOperationException("SUB Vx, Vy operation should not use Y as 0xF or bigger (the carry flag register)");
                                }

                                if (vx > vy)
                                {
                                    V[x] -= vy;

                                    // Carry
                                    // Because the register can only store values from 0 to 255 (8 bit value),
                                    // it means that if the sum of VX and VY is larger than 255,
                                    // it can’t be stored in the register (or actually it starts counting from 0 again).
                                    // If the sum of VX and VY is larger than 255, 
                                    // we use the carry flag to let the system know that the total sum of both values was indeed larger than 255. 
                                    V[0xF] = 1;
                                }
                                else
                                {
                                    V[x] -= vy;

                                    V[0xF] = 0;
                                }

                                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                                // (in which case you need to store the program counter in the stack).
                                // If the next opcode should be skipped, increase the program counter by four.
                                PC += 2;
                                break;

                            // 0x8XY6: Stores the least significant bit of VX in VF 
                            // and then shifts VX to the right by 1.
                            case 0x0006:
                                if (x >= 0xF)
                                {
                                    throw new InvalidOperationException("SHR Vx, {Vy} operation should not use X as 0xF or bigger (the carry flag register)");
                                }

                                if (y >= 0xF)
                                {
                                    throw new InvalidOperationException("SHR Vx, {Vy} operation should not use Y as 0xF or bigger (the carry flag register)");
                                }

                                V[0xF] = unchecked((byte)(vx & 0x01));
                                V[x] >>= 1;

                                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                                // (in which case you need to store the program counter in the stack).
                                // If the next opcode should be skipped, increase the program counter by four.
                                PC += 2;
                                break;

                            // 0x8XY7: Sets VX to VY minus VX. VF is set to 0 when 
                            // there's a borrow, and 1 when there isn't.
                            case 0x0007:
                                if (x >= 0xF)
                                {
                                    throw new InvalidOperationException("SUBN Vx, Vy operation should not use X as 0xF or bigger (the carry flag register)");
                                }

                                if (y >= 0xF)
                                {
                                    throw new InvalidOperationException("SUBN Vx, Vy operation should not use Y as 0xF or bigger (the carry flag register)");
                                }

                                if (vy > vx)
                                {
                                    V[x] = unchecked((byte)(vy - vx));
                                 
                                       // Carry
                                    // Because the register can only store values from 0 to 255 (8 bit value),
                                    // it means that if the sum of VX and VY is larger than 255,
                                    // it can’t be stored in the register (or actually it starts counting from 0 again).
                                    // If the sum of VX and VY is larger than 255, 
                                    // we use the carry flag to let the system know that the total sum of both values was indeed larger than 255. 
                                    V[0xF] = 1;
                                }
                                else
                                {
                                    V[x] = unchecked((byte)(vy - vx));
                                   
                                     V[0xF] = 0;
                                }

                                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                                // (in which case you need to store the program counter in the stack).
                                // If the next opcode should be skipped, increase the program counter by four.
                                PC += 2;
                                break;

                            // 0x8XYE: Stores the most significant bit of VX in VF  
                            // and then shifts VX to the left by 1.
                            case 0x000E:
                                if (x >= 0xF)
                                {
                                    throw new InvalidOperationException("SUBN Vx, Vy operation should not use X as 0xF or bigger (the carry flag register)");
                                }

                                if (y >= 0xF)
                                {
                                    throw new InvalidOperationException("SUBN Vx, Vy operation should not use Y as 0xF or bigger (the carry flag register)");
                                }

                                V[0xF] = unchecked((byte)((vx & 0x80) >> 7));
                                V[x] <<= 1;

                                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                                // (in which case you need to store the program counter in the stack).
                                // If the next opcode should be skipped, increase the program counter by four.
                                PC += 2;
                                break;

                            default:
                                Console.WriteLine($"Unknown opcode: {opcode:X4}");
                                break;
                        }
                        break;
                    }

                // 0x9XY0: Skips the next instruction if VX doesn't equal VY.
                // (Usually the next instruction is a jump to skip a code block)    
                case 0x9000:
                    if (x > 0xF)
                    {
                        throw new InvalidOperationException("SNE Vx, Vy operation should not use X bigger than 0xF");
                    }

                    if (y > 0xF)
                    {
                        throw new InvalidOperationException("SNE Vx, Vy operation should not use Y bigger than 0xF");
                    }

                    if (vx != vy)
                    {
                        // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                        // This is true unless you jump to a certain address in the memory or if you call a subroutine
                        // (in which case you need to store the program counter in the stack).
                        // If the next opcode should be skipped, increase the program counter by four.
                        PC += 4;
                    }
                    else
                    {
                        // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                        // This is true unless you jump to a certain address in the memory or if you call a subroutine
                        // (in which case you need to store the program counter in the stack).
                        // If the next opcode should be skipped, increase the program counter by four.
                        PC += 2;
                    }
                    break;

                // 0xANNN: Sets I to the address NNN
                case 0xA000:
                    I = nnn;

                    // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                    // This is true unless you jump to a certain address in the memory or if you call a subroutine
                    // (in which case you need to store the program counter in the stack).
                    // If the next opcode should be skipped, increase the program counter by four.
                    PC += 2;
                    break;

                // 0xBNNN: Jumps to the address NNN plus V0.
                case 0xB000:
                    // Now that we have stored the program counter, we can set it to the address NNN.
                    // Remember, because we’re calling a subroutine at a specific address, 
                    // you should not increase the program counter by two.
                    int dest = V[0] + nnn;

                    if (dest < 0x200)
                    {
                        string message = $"Illegal jump target: 0x{dest:X4} => 0x{opcode:X4} + V[0] (0x{V[0]:X2})";
                        Console.WriteLine(message);
                        throw new InvalidOperationException(message);
                    }

                    if (dest > 0xFFF)
                    {
                        string message = $"Illegal jump target: 0x{dest:X4} => 0x{opcode:X4} + V[0] (0x{V[0]:X2})";
                        Console.WriteLine(message);
                        throw new InvalidOperationException(message);
                    }

                    PC = unchecked((ushort)dest);
                    break;

                // 0xCXNN: Sets VX to the result of a bitwise and operation on a random number
                // (Typically: 0 to 255) and NN.
                case 0xC000:
                    if (x > 0xF)
                    {
                        throw new InvalidOperationException("RND Vx, byte operation should not use X bigger than 0xF");
                    }

                    V[x] = unchecked((byte)(rand.Next(0, 0x100) & nn));

                    // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                    // This is true unless you jump to a certain address in the memory or if you call a subroutine
                    // (in which case you need to store the program counter in the stack).
                    // If the next opcode should be skipped, increase the program counter by four.
                    PC += 2;
                    break;

                // 0xDXYN: Draws a sprite at coordinate (VX, VY) 
                case 0xD000:
                    {
                        // Fetch the position and height of the sprite
                        ushort height = unchecked((ushort)(opcode & 0x000F));

                        ushort pixel;

                        // Reset register VF
                        V[0xF] = 0;

                        // Loop over each row
                        for (int row = 0; row < height; row++)
                        {
                            // Fetch the pixel value from the memory starting at location I
                            pixel = memory[I + row];

                            // Loop over 8 bits (columns) of one row
                            for (int column = 0; column < 8; column++)
                            {
                                // Check if the current evaluated pixel is set to 1
                                // Note that (0x80 >> column) scan through the byte, one bit at the time
                                if ((pixel & (0x80 >> column)) != 0)
                                {
                                    var currentPixel = vx + column + ((vy + row) * WIDTH);

                                    // Check if the pixel on the display is set to 1.
                                    //  If it is set, we need to register the collision by setting the VF register
                                    if (gfx[currentPixel] == 1)
                                    {
                                        V[0xF] = 1;
                                    }

                                    // Set the pixel value by using XOR
                                    gfx[currentPixel] ^= 1;
                                }
                            }
                        }

                        // We changed our gfx[] array and thus need to update the screen.
                        DrawFlag = true;

                        // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                        // This is true unless you jump to a certain address in the memory or if you call a subroutine
                        // (in which case you need to store the program counter in the stack).
                        // If the next opcode should be skipped, increase the program counter by four.
                        PC += 2;
                    }
                    break;

                case 0xE000:
                    switch (opcode & 0x00FF)
                    {
                        // 0xEX9E: Skips the next instruction if the key stored in VX is pressed
                        case 0x009E:
                            if (x > 0xF)
                            {
                                throw new InvalidOperationException("SKP Vx operation should not use X bigger than 0xF");
                            }

                            if (vx > 0xF)
                            {
                                throw new InvalidOperationException("SKP Vx operation should not use VX value bigger than 0xF");
                            }

                            if (keys[vx] != 0)
                            {
                                PC += 4;
                            }
                            else
                            {
                                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                                // (in which case you need to store the program counter in the stack).
                                // If the next opcode should be skipped, increase the program counter by four.
                                PC += 2;
                            }
                            break;

                        // 0xEX9E: Skips the next instruction if the key stored in VX is pressed
                        case 0x00A1:
                            if (x > 0xF)
                            {
                                throw new InvalidOperationException("SKP Vx operation should not use X bigger than 0xF");
                            }

                            if (vx > 0xF)
                            {
                                throw new InvalidOperationException("SKP Vx operation should not use VX value bigger than 0xF");
                            }

                            if (keys[vx] == 0)
                            {
                                PC += 4;
                            }
                            else
                            {
                                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                                // (in which case you need to store the program counter in the stack).
                                // If the next opcode should be skipped, increase the program counter by four.
                                PC += 2;
                            }
                            break;

                        default:
                            Console.WriteLine($"Unknown opcode: {opcode:X4}");
                            break;
                    }
                    break;

                case 0xF000:
                    switch (opcode & 0x00FF)
                    {
                        // 0xFX07: Sets VX to the value of the delay timer.
                        case 0x0007:
                            if (x > 0xF)
                            {
                                throw new InvalidOperationException("LD Vx, DT operation should not use X bigger than 0xF");
                            }

                            V[x] = DT;

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            PC += 2;
                            break;

                        // 0xFX0A: A key press is awaited, and then stored in VX.
                        // (Blocking Operation. All instruction halted until next key event)
                        case 0x000A:
                            bool keyPressed = false;

                            while (!keyPressed)
                            {
                                for (int index = 0; index < keys.Length; index++)
                                {
                                    byte keyValue = keys[index];
                                    keyPressed |= keyValue > 0;

                                    if (keyPressed)
                                    {
                                        V[x] = unchecked((byte)index);
                                        break;
                                    }
                                }

                                Thread.Sleep(250);
                            }

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            PC += 2;
                            break;

                        // 0xFX15: Sets the delay timer to VX.
                        case 0x0015:
                            if (x > 0xF)
                            {
                                throw new InvalidOperationException("LD DT, Vx operation should not use X bigger than 0xF");
                            }

                            DT = vx;

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            PC += 2;
                            break;

                        // 0xFX18: Sets the sound timer to VX.
                        case 0x0018:
                            if (x > 0xF)
                            {
                                throw new InvalidOperationException("LD ST, Vx operation should not use X bigger than 0xF");
                            }

                            ST = vx;

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            PC += 2;
                            break;

                        // 0xFX1E: Adds VX to I.
                        // VF is set to 1 when there is a range overflow (I+VX>0xFFF), and to 0 when there isn't.
                        case 0x001E:
                            if (x > 0xF)
                            {
                                throw new InvalidOperationException("ADD I, Vx operation should not use X bigger than 0xF");
                            }

                            if ((I + vx) > 0xFFF)
                            {
                                throw new InvalidOperationException("ADD I, Vx operation should not exceed 0xFFF");
                            }

                            I += vx;

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            PC += 2;
                            break;

                        // 0xFX29: Sets I to the location of the sprite for the character in VX.
                        // Characters 0x0-0xF are represented by a 4x5 font.
                        case 0x0029:
                            if (x > 0xF)
                            {
                                throw new InvalidOperationException("LD F, Vx operation should not use X bigger than 0xF");
                            }

                            if (vx > 0x0F)
                            {
                                throw new InvalidOperationException("LD F, Vx operation should not try to access sprites higher than 0x0F");
                            }

                            I = unchecked((ushort)(vx * 5));

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            PC += 2;
                            break;

                        // 0x0033: Stores the Binary-coded decimal representation of VX at the addresses I, I plus 1, and I plus 2
                        case 0x0033:
                            if (x > 0xF)
                            {
                                throw new InvalidOperationException("LD B, Vx operation should not use X bigger than 0xF");
                            }

                            if (I < 0x200)
                            {
                                throw new InvalidOperationException("LD B, Vx operation should be below 0x200");
                            }

                            if ((I + 2) > 0xFFF)
                            {
                                throw new InvalidOperationException("LD B, Vx operation should not exceed address 0xFFF");
                            }

                            memory[I] = unchecked((byte)(vx / 100));
                            memory[I + 1] = unchecked((byte)(vx / 10 % 10));
                            memory[I + 2] = unchecked((byte)(vx % 100 % 10));

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            PC += 2;
                            break;

                        // 0xFX55: Stores V0 to VX (including VX) in memory starting at address I.
                        // The offset from I is increased by 1 for each value written, but I itself is left unmodified
                        case 0x0055:
                            if (x > 0xF)
                            {
                                throw new InvalidOperationException("LD [I], Vx operation should not use X bigger than 0xF");
                            }

                            int destAddr = I + x;

                            if (destAddr > 0xFFF)
                            {
                                throw new InvalidOperationException("LD [I], Vx operation should not exceed 0xFFF");
                            }

                            if (destAddr < 0x200)
                            {
                                throw new InvalidOperationException("LD [I], Vx operation should be below 0x200");
                            }

                            for (int index = 0; index <= x; ++index)
                            {
                                memory[I + index] = V[index];
                            }

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            PC += 2;
                            break;

                        // 0xFX65: Fills V0 to VX (including VX) with values from memory starting at address I.
                        // The offset from I is increased by 1 for each value written, but I itself is left unmodified
                        case 0x0065:
                            if (x > 0xF)
                            {
                                throw new InvalidOperationException("LD Vx, [I] operation should not use X bigger than 0xF");
                            }

                            if ((I + x) > 0xFFF)
                            {
                                throw new InvalidOperationException("LD Vx, [I] operation should not exceed 0xFFF");
                            }

                            for (int index = 0; index <= x; ++index)
                            {
                                V[index] = memory[I + index];
                            }

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            PC += 2;
                            break;

                        default:
                            Console.WriteLine($"Unknown opcode: {opcode:X4}");
                            break;
                    }
                    break;

                default:
                    Console.WriteLine($"Unknown opcode: {opcode:X4}");
                    break;
            }

            // Execute Opcode

            // Update timers
            if (DT > 0)
            {
                --DT;
            }

            if (ST > 0)
            {
                if (ST == 1)
                {
                    Console.WriteLine("BEEP!");
                    //Console.Beep(500, milliseconds);
                }

                --ST;
            }
        }

        public void SetKeys(byte[] keys)
        {
            if (keys.Length != this.keys.Length)
            {
                throw new InvalidOperationException($"Keys should be exactly {this.keys.Length} bytes long.");
            }

            keys.CopyTo(this.keys, 0);
        }
    }
}
