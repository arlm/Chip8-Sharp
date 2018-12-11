using System;

namespace Chip8.CPU
{
    public class CPU
    {

        // The Chip 8 has 35 opcodes which are all two bytes long.
        ushort opcode;

        // The Chip 8 has 4K memory in total
        byte[] memory = new byte[4096];

        // The Chip 8 has 15 8-bit general purpose registers named V0, V1 up to VE.
        // The 16th register is used  for the ‘carry flag’.
        byte[] V = new byte[16];

        // The Index register which can have a value from 0x000 to 0xFFF
        ushort I;

        // Theis program counter which can have a value from 0x000 to 0xFFF
        ushort pc;

        // The systems memory map:
        // 0x000-0x1FF - Chip 8 interpreter (contains font set in emu)
        // 0x050-0x0A0 - Used for the built in 4x5 pixel font set(0-F)
        // 0x200-0xFFF - Program ROM and work RAM

        // The graphics system: The chip 8 has one instruction that draws sprite to the screen.
        // Drawing is done in XOR mode and if a pixel is turned off as a result of drawing, the VF register is set. 
        // This is used for collision detection.

        // The graphics of the Chip 8 are black and white and the screen has a total of 2048 pixels(64 x 32).
        // This can easily be implemented using an array that hold the pixel state(1 or 0)
        byte[] gfx = new byte[64 * 32];


        // Interupts and hardware registers.
        // The Chip 8 has none, but there are two timer registers that count at 60 Hz.
        // When set above zero they will count down to zero.
        byte delay_timer;

        // The system’s buzzer sounds whenever the sound timer reaches zero.
        byte sound_timer;

        // While the specification don’t mention a stack, you will need to implement one as part of the interpreter yourself.
        // The stack is used to remember the current location before a jump is performed.
        // So anytime you perform a jump or call a subroutine, store the program counter in the stack before proceeding.
        // The system has 16 levels of stack and in order to remember which level of the stack is used, you need to implement a stack pointer (sp).
        ushort[] stack = new ushort[16];
        ushort sp;

        // The Chip 8 has a HEX based keypad(0x0 - 0xF), you can use an array to store the current state of the key.
        byte[] key = new byte[16];
    }
}
