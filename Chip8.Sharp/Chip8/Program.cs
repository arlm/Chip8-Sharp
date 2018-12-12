using System;
using System.IO;
using Chip8.Core;
using SDL2;

namespace Chip8
{
    class Program
    {
        // In this example we assume you will create a separate class to handle the opcodes.
        private static CPU myChip8;

        private const int WIDTH = 1024;
        private const int HEIGHT = 768;
        private static Random r = new Random();

        private static IntPtr rendererPtr;
        private static IntPtr windowPtr;

        static void Main(string[] args)
        {

            var result = SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);

            // Setup the graphics (window size, display mode, etc) 
            SetupGraphics();

            // Setup the input system (register input callbacks)
            SetupInput();

            // Initialize the CHIP-8 system (Clear the memory, registers and screen)
            myChip8 = new CPU();

            // Load (copy) the game into the memory
            myChip8.LoadGame($"progs{Path.DirectorySeparatorChar}pong2.c8");

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

                // 60 Hz = 1000 ms /60 = 16.666 ms  => approx. 17 ms (17 ms * 60 = 1020 ms)
                SDL.SDL_Delay(17);
            }

            Quit();
        }

        private static void SetupGraphics()
        {
            windowPtr = SDL.SDL_CreateWindow("CHIP-8", 0, 0, WIDTH, HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI);
            SDL.SDL_ShowWindow(windowPtr);

            rendererPtr = SDL.SDL_GetRenderer(windowPtr);

            SDL.SDL_SetRenderDrawColor(rendererPtr, 0, 0, 0, 255);
            SDL.SDL_RenderClear(rendererPtr);

            SDL.SDL_RenderPresent(rendererPtr);
        }

        private static void SetupInput()
        {
            while (SDL.SDL_PollEvent(out var evt) > 0)
            {
                switch (evt.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        Quit();
                        break;

                    case SDL.SDL_EventType.SDL_SENSORUPDATE:
                        break;

                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        break;
                    case SDL.SDL_EventType.SDL_KEYUP:
                        break;

                    case SDL.SDL_EventType.SDL_MOUSEMOTION:
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                        break;

                    case SDL.SDL_EventType.SDL_FINGERMOTION:
                        break;
                    case SDL.SDL_EventType.SDL_FINGERDOWN:
                        break;
                    case SDL.SDL_EventType.SDL_FINGERUP:
                        break;
                    case SDL.SDL_EventType.SDL_MULTIGESTURE:
                        break;

                    case SDL.SDL_EventType.SDL_JOYDEVICEADDED:
                        break;
                    case SDL.SDL_EventType.SDL_JOYDEVICEREMOVED:
                        break;
                    case SDL.SDL_EventType.SDL_JOYBUTTONDOWN:
                        break;
                    case SDL.SDL_EventType.SDL_JOYBUTTONUP:
                        break;
                    case SDL.SDL_EventType.SDL_JOYHATMOTION:
                        break;
                    case SDL.SDL_EventType.SDL_JOYAXISMOTION:
                        break;
                    case SDL.SDL_EventType.SDL_JOYBALLMOTION:
                        break;

                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMAPPED:
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERBUTTONUP:
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERAXISMOTION:
                        break;
                }
            }
        }

        private static void Quit()
        {
            SDL.SDL_DestroyRenderer(rendererPtr);
            SDL.SDL_DestroyWindow(windowPtr);
            SDL.SDL_Quit();
        }

        private static void DrawGraphics()
        {
            SDL.SDL_SetRenderDrawColor(rendererPtr, 255, 255, 255, 255);

            var rect = new SDL.SDL_Rect
            {
                x = 10,
                y = 10,
                w = 10,
                h = 10
            };

            SDL.SDL_RenderDrawRect(rendererPtr, ref rect);
            SDL.SDL_RenderPresent(rendererPtr);
        }
    }
}
