using System;
using System.IO;
using System.Threading.Tasks;
using Chip8.Core;
using SDL2;

namespace Chip8
{
    public enum KeyPressSurfaces
    {
        Default,
        Up,
        Down,
        Left,
        Right,
        Count
    }

    class Program
    {
        // In this example we assume you will create a separate class to handle the opcodes.
        private static CPU myChip8;

        private const int WIDTH = 640;
        private const int HEIGHT = 480;

        private static readonly IntPtr[] surfacesPtr = new IntPtr[(int)KeyPressSurfaces.Count];
        private static IntPtr currentSurfacePtr;

        //Set text color as black
        SDL.SDL_Color textColor = new SDL.SDL_Color { r = 0, g = 0, b = 0, a = 255 };

        readonly Timer timer = new Timer();

        private static SDLDriver driver = new SDLDriver();

        static int Main(string[] args)
        {
            if (!driver.Init(WIDTH, HEIGHT))
            {
                Console.WriteLine("Failed to initialize!");
                return -1;
            }

            // Setup the graphics (window size, display mode, etc) 
            if (!driver.SetupGraphics($"assets{Path.DirectorySeparatorChar}CHIP-8.logo.bmp"))
            {
                Console.WriteLine("Failed to setup graphics!");
                return -2;
            }

            // Setup the input system (register input callbacks)
            if (!driver.SetupInput())
            {
                Console.WriteLine("Failed to setup input!");
                return -3;
            }

            if (!LoadMedia())
            {
                Console.WriteLine("Failed to load media!");
                return -4;
            }

            //Main loop flag
            bool quit = false;

            //Event handler
            SDL.SDL_Event evt;

            //Set default current surface
            currentSurfacePtr = surfacesPtr[(int)KeyPressSurfaces.Default];
            Console.WriteLine("Loaded...");

            //While application is running
            while (!quit)
            {
                //Handle events on queue
                while (SDL.SDL_PollEvent(out evt) != 0)
                {
                    //User requests quit
                    switch (evt.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            Console.WriteLine("Quitting...");
                            quit = true;
                            break;

                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            //Select surfaces based on key press
                            switch (evt.key.keysym.sym)
                            {
                                case SDL.SDL_Keycode.SDLK_ESCAPE:
                                    Console.WriteLine("ESCAPE");
                                    Console.WriteLine("Quitting...");
                                    quit = true;
                                    break;

                                case SDL.SDL_Keycode.SDLK_UP:
                                    currentSurfacePtr = surfacesPtr[(int)KeyPressSurfaces.Up];
                                    Console.WriteLine("UP");
                                    Task.Delay(1500).ContinueWith(_ => currentSurfacePtr = surfacesPtr[(int)KeyPressSurfaces.Default]);
                                    break;

                                case SDL.SDL_Keycode.SDLK_DOWN:
                                    currentSurfacePtr = surfacesPtr[(int)KeyPressSurfaces.Down];
                                    Console.WriteLine("DOWN");
                                    Task.Delay(1500).ContinueWith(_ => currentSurfacePtr = surfacesPtr[(int)KeyPressSurfaces.Default]);
                                    break;

                                case SDL.SDL_Keycode.SDLK_LEFT:
                                    currentSurfacePtr = surfacesPtr[(int)KeyPressSurfaces.Left];
                                    Console.WriteLine("LEFT");
                                    Task.Delay(1500).ContinueWith(_ => currentSurfacePtr = surfacesPtr[(int)KeyPressSurfaces.Default]);
                                    break;

                                case SDL.SDL_Keycode.SDLK_RIGHT:
                                    currentSurfacePtr = surfacesPtr[(int)KeyPressSurfaces.Right];
                                    Console.WriteLine("RIGHT");
                                    Task.Delay(1500).ContinueWith(_ => currentSurfacePtr = surfacesPtr[(int)KeyPressSurfaces.Default]);
                                    break;

                                default:
                                    currentSurfacePtr = surfacesPtr[(int)KeyPressSurfaces.Default];
                                    Console.WriteLine("Default Key Press");
                                    break;
                            }
                            break;
                    }
                }

                if (!driver.DrawSurface(currentSurfacePtr))
                {
                    Console.WriteLine("Failed to draw surface!");
                }

                if (!driver.Render())
                {
                    Console.WriteLine("Failed to draw surface!");
                }
            }

            Quit();

            return 0;

            /*


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
            */
        }

        private static void Quit()
        {
            currentSurfacePtr = IntPtr.Zero;

            for (int index = 0; index < (int)KeyPressSurfaces.Count; ++index)
            {
                if (surfacesPtr[index] != IntPtr.Zero)
                {
                    SDL.SDL_FreeSurface(surfacesPtr[index]);
                    surfacesPtr[index] = IntPtr.Zero;
                }
            }

            driver.Quit();
        }

        static bool LoadMedia()
        {
            //Loading success flag
            bool success = true;

            //Load default surface
            surfacesPtr[(int)KeyPressSurfaces.Default] = driver.LoadSurface("assets/press.bmp");
            if (surfacesPtr[(int)KeyPressSurfaces.Default] == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load default image!");
                success = false;
            }

            //Load up surface
            surfacesPtr[(int)KeyPressSurfaces.Up] = driver.LoadSurface("assets/up.bmp");
            if (surfacesPtr[(int)KeyPressSurfaces.Up] == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load up image!");
                success = false;
            }

            //Load down surface
            surfacesPtr[(int)KeyPressSurfaces.Down] = driver.LoadSurface("assets/down.bmp");
            if (surfacesPtr[(int)KeyPressSurfaces.Down] == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load down image!");
                success = false;
            }

            //Load left surface
            surfacesPtr[(int)KeyPressSurfaces.Left] = driver.LoadSurface("assets/left.bmp");
            if (surfacesPtr[(int)KeyPressSurfaces.Left] == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load left image!");
                success = false;
            }

            //Load right surface
            surfacesPtr[(int)KeyPressSurfaces.Right] = driver.LoadSurface("assets/right.bmp");
            if (surfacesPtr[(int)KeyPressSurfaces.Right] == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load right image!");
                success = false;
            }

            return success;
        }
    }
}
