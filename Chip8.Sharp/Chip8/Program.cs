using System;
using Chip8.Core;

namespace Chip8
{
    class Program
    {
        // In this example we assume you will create a separate class to handle the opcodes.
        static CPU myChip8;

        static void Main(string[] args)
        {
            // Setup the graphics (window size, display mode, etc) 
            SetupGraphics();

            // Setup the input system (register input callbacks)
            SetupInput();

            // Initialize the CHIP-8 system (Clear the memory, registers and screen)
            myChip8 = new CPU();

            // Load (copy) the game into the memory
            myChip8.LoadGame("pong");

            // Emulation loop
            for (; ; )
            {
                // Emulate one cycle of the system
                myChip8.EmulateCycle();

                // If the draw flag is set, update the screen
                // Because the system does not draw every cycle, we should set a draw flag when we need to update our screen.
                // Only two opcodes should set this flag:
                //    0x00E0 – Clears the screen
                //    0xDXYN – Draws a sprite on the screen

                if (myChip8.DrawFlag)
                {
                    DrawGraphics();
                }

                // Store key press state (Press and Release)
                // If we press or release a key, we should store this state in the part that emulates the keypad
                myChip8.SetKeys();
            }
        }

        private static void DrawGraphics()
        {
            throw new NotImplementedException();
        }

        private static void SetupGraphics()
        {
            throw new NotImplementedException();
        }

        private static void SetupInput()
        {
            throw new NotImplementedException();
        }
    }
}
