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
        private static IntPtr screenSurfacePtr;
        private static IntPtr helloWorldPtr;

        static int Main(string[] args)
        {
            if (!Init())
            {
                Console.WriteLine("Failed to initialize!");
                return -1;
            }

            // Setup the graphics (window size, display mode, etc) 
            if (!SetupGraphics())
            {
                Console.WriteLine("Failed to load media!");
                return -2;
            }

            //Main loop flag
            bool quit = false;

            //Event handler
            SDL.SDL_Event evt;

            //While application is running
            while (!quit)
            {
                //Handle events on queue
                while (SDL.SDL_PollEvent(out evt) != 0)
                {
                    //User requests quit
                    quit |= evt.type == SDL.SDL_EventType.SDL_QUIT;
                }

                //Apply the image
                SDL.SDL_BlitSurface(helloWorldPtr, IntPtr.Zero, screenSurfacePtr, IntPtr.Zero);

                //Update the surface
                SDL.SDL_UpdateWindowSurface(windowPtr);

            }

            Quit();

            return 0;

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
        }

        private static bool Init()
        {
            //Initialization flag
            bool result = true;

            //Initialize SDL
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine($"SDL could not initialize! SDL_Error: {SDL.SDL_GetError()}");

                result = false;
            }
            else
            {
                //Create window
                windowPtr = SDL.SDL_CreateWindow("SDL Tutorial", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, WIDTH, HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

                if (windowPtr == IntPtr.Zero)
                {
                    Console.WriteLine("Window could not be created! SDL_Error: {SDL.SDL_GetError()}");
                    result = false;
                }
                else
                {
                    //Get window surface
                    screenSurfacePtr = SDL.SDL_GetWindowSurface(windowPtr);
                }
            }

            return result;
        }

        private static bool SetupGraphics()
        {
            //Initialization flag
            bool result = true && LoadMedia($"assets{Path.DirectorySeparatorChar}CHIP-8.logo.bmp");

            const int INTERVAL = 150;
            int counter = 2500;
            SDL.SDL_Event evt;

            //While application is running
            while (counter > 0)
            {
                //Handle events on queue
                while (SDL.SDL_PollEvent(out evt) != 0)
                {
                    //User requests quit
                    if (evt.type == SDL.SDL_EventType.SDL_QUIT)
                    {
                        Quit();
                        return false;
                    }
                }

                //Apply the image
                SDL.SDL_BlitSurface(helloWorldPtr, IntPtr.Zero, screenSurfacePtr, IntPtr.Zero);

                //Update the surface
                SDL.SDL_UpdateWindowSurface(windowPtr);

                SDL.SDL_Delay(INTERVAL);
                counter -= INTERVAL;
            }

            return result;
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
            SDL.SDL_FreeSurface(helloWorldPtr);
            SDL.SDL_DestroyRenderer(rendererPtr);
            SDL.SDL_DestroyWindow(windowPtr);
            SDL.SDL_Quit();
        }

        private static void DrawGraphics()
        {
            LoadMedia($"assets{Path.DirectorySeparatorChar}CHIP-8.logo.bmp");
        }

        private static bool LoadMedia(string fileName)
        {
            //Loading success flag
            bool result = true;

            //Load splash image
            helloWorldPtr = SDL.SDL_LoadBMP(fileName);

            if (helloWorldPtr == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load image {fileName}! SDL Error: {SDL.SDL_GetError()}");
                result = false;
            }

            return result;
        }
    }
}
