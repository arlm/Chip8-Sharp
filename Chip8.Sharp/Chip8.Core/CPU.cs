using System;
using System.IO;

namespace Chip8.Core
{
    public partial class CPU
    {

        // The CHIP-8 has 35 opcodes which are all two bytes long.
        ushort opcode;

        // The CHIP-8 has 4K memory in total
        readonly byte[] memory = new byte[4096];

        // The CHIP-8 has 15 8-bit general purpose registers named V0, V1 up to VE.
        // The 16th register is used  for the ‘carry flag’.
        readonly byte[] V = new byte[16];

        // The Index register which can have a value from 0x000 to 0xFFF
        ushort I;

        // Theis program counter which can have a value from 0x000 to 0xFFF
        ushort pc;

        // The systems memory map:
        // 0x000-0x1FF - CHIP-8 interpreter (contains font set in emu)
        // 0x050-0x0A0 - Used for the built in 4x5 pixel font set(0-F)
        // 0x200-0xFFF - Program ROM and work RAM

        // The graphics system: The CHIP-8 has one instruction that draws sprite to the screen.
        // Drawing is done in XOR mode and if a pixel is turned off as a result of drawing, the VF register is set. 
        // This is used for collision detection.

        // The graphics of the CHIP-8 are black and white and the screen has a total of 2048 pixels(64 x 32).
        // This can easily be implemented using an array that hold the pixel state(1 or 0)
        readonly byte[] gfx = new byte[64 * 32];


        // Interupts and hardware registers.
        // The CHIP-8  has none, but there are two timer registers that count at 60 Hz.
        // When set above zero they will count down to zero.
        byte delay_timer;

        // The system’s buzzer sounds whenever the sound timer reaches zero.
        byte sound_timer;

        // While the specification don’t mention a stack, you will need to implement one as part of the interpreter yourself.
        // The stack is used to remember the current location before a jump is performed.
        // So anytime you perform a jump or call a subroutine, store the program counter in the stack before proceeding.
        // The system has 16 levels of stack and in order to remember which level of the stack is used, you need to implement a stack pointer (sp).
        readonly ushort[] stack = new ushort[16];
        ushort sp;

        // The CHIP-8 has a HEX based keypad(0x0 - 0xF), you can use an array to store the current state of the key.
        byte[] key = new byte[16];

        public bool DrawFlag { get; set; }

        // Initialize registers and memory once
        public CPU()
        {
            // The system expects the application to be loaded at memory location 0x200. 
            pc = 0x200;     // Program counter starts at 0x200

            opcode = 0;     // Reset current opcode  
            I = 0;          // Reset index register
            sp = 0;         // Reset stack pointer

            // Clear display  
            // Clear stack
            // Clear registers V0-VF
            // Clear memory

            // Load fontset
            for (int i = 0; i < 80; ++i)
            {
                memory[i] = CHIP8_FONTSET[i];
            }

            // Reset timers
        }

        public void LoadGame(string fileName)
        {
            // Buffer of 4KiB (4096) minus 0x200 (512 bytes)
            const int MAX_BUFFER_SIZE = 4096 - 0x200;

            using (var stream = File.OpenRead(fileName))
            {
                var buffer = new byte[MAX_BUFFER_SIZE];
                var bufferSize = stream.Read(buffer, 0, MAX_BUFFER_SIZE);

                for (int index = 0; index < bufferSize; ++index)
                {
                    memory[index + 0x200] = buffer[index];
                }
            }
        }

        public void EmulateCycle()
        {
            // Fetch Opcode

            // During this step, the system will fetch one opcode from the memory at the location specified by the program counter (pc).
            // In the emulator, data is stored in an array in which each address contains one byte.
            // As one opcode is 2 bytes long, we will need to fetch two successive bytes and merge them to get the actual opcode.
            opcode = (ushort)((memory[pc] << 8) | memory[pc + 1]);

            // Decode Opcode
            switch (opcode & 0xF000)
            {
                // In some cases we can not rely solely on the first four bits to see what the opcode means.
                // For example, 0x00E0 and 0x00EE both start with 0x00.
                // In this case we add an additional switch and compare the last four bits:
                case 0x0000:
                    switch (opcode & 0x000F)
                    {
                        // 0x00E0: Clears the screen
                        case 0x0000:

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            pc += 2;

                            break;

                        // 0x0033: Stores the Binary-coded decimal representation of VX at the addresses I, I plus 1, and I plus 2
                        case 0x0003:
                            memory[I] = (byte)(V[(opcode & 0x0F00) >> 8] / 100);
                            memory[I + 1] = (byte)((V[(opcode & 0x0F00) >> 8] / 10) % 10);
                            memory[I + 2] = (byte)((V[(opcode & 0x0F00) >> 8] % 100) % 10);

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            pc += 2;
                            break;

                        // 0x8XY4: This opcode adds the value of VY to VX.
                        // Register VF is set to 1 when there is a carry and set to 0 when there isn’t.
                        case 0x0004:
                            int VX = (opcode & 0x0F00) >> 8;
                            int VY = (opcode & 0x00F0) >> 4;

                            if (V[VY] > (0xFF - V[VX]))
                            {
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
                                V[0xF] = 0;
                            }

                            V[VX] += V[VY];

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            pc += 2;
                            break;

                        // 0x00EE: Returns from subrouti
                        case 0x000E:

                            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                            // This is true unless you jump to a certain address in the memory or if you call a subroutine
                            // (in which case you need to store the program counter in the stack).
                            // If the next opcode should be skipped, increase the program counter by four.
                            pc += 2; break;

                        default:
                            Console.WriteLine($"Unknown opcode: {opcode:X}");
                            break;
                    }
                    break;

                // 0x2NNN: This opcode calls the subroutine at address NNN.
                case 0x2000:
                    // Because we will need to temporary jump to address NNN,
                    // it means that we should store the current address of the program counter in the stack.
                    stack[sp] = pc;

                    //  After storing the value of the program counter in the stack,
                    // increase the stack pointer to prevent overwriting the current stack. 
                    ++sp;

                    // Now that we have stored the program counter, we can set it to the address NNN.
                    // Remember, because we’re calling a subroutine at a specific address, 
                    // you should not increase the program counter by two.
                    pc = (ushort)(opcode & 0x0FFF);
                    break;

                // 0xANNN: Sets I to the address NNN
                case 0xA000:
                    I = (ushort)(opcode & 0x0FFF);

                    // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                    // This is true unless you jump to a certain address in the memory or if you call a subroutine
                    // (in which case you need to store the program counter in the stack).
                    // If the next opcode should be skipped, increase the program counter by four.
                    pc += 2;
                    break;

                case 0xD000:
                    {
                        // Fetch the position and height of the sprite
                        ushort x = V[(opcode & 0x0F00) >> 8];
                        ushort y = V[(opcode & 0x00F0) >> 4];
                        ushort height = (ushort)(opcode & 0x000F);

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
                                    var currentPixel = x + column + ((y + row) * 64);

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
                        pc += 2;
                    }
                    break;

                case 0xE000:
                    switch (opcode & 0x00FF)
                    {
                        // EX9E: Skips the next instruction if the key stored in VX is pressed
                        case 0x009E:
                            if (key[V[(opcode & 0x0F00) >> 8]] != 0)
                            {
                                pc += 4;
                            }
                            else
                            {
                                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                                // (in which case you need to store the program counter in the stack).
                                // If the next opcode should be skipped, increase the program counter by four.
                                pc += 2;
                            }
                            break;
                    }
                    break;

                default:
                    Console.WriteLine($"Unknown opcode: {opcode:X}");
                    break;
            }

            // Execute Opcode


            // Update timers
            if (delay_timer > 0)
            {
                --delay_timer;
            }

            if (sound_timer > 0)
            {
                if (sound_timer == 1)
                {
                    Console.WriteLine("BEEP!");
                }

                --sound_timer;
            }
        }

        public void SetKeys()
        {
            throw new NotImplementedException();
        }
    }
}
