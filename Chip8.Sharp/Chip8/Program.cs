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

    class Program : IDisposable
    {
        // In this example we assume you will create a separate class to handle the opcodes.
        private static CPU myChip8;

        private const int WIDTH = 640;
        private const int HEIGHT = 480;

        private static readonly Texture[] textures = new Texture[(int)KeyPressSurfaces.Count];
        private static Texture currentTexture;

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
            currentTexture = textures[(int)KeyPressSurfaces.Default];
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
                                    currentTexture = textures[(int)KeyPressSurfaces.Up];
                                    Console.WriteLine("UP");
                                    Task.Delay(1500).ContinueWith(_ => currentTexture = textures[(int)KeyPressSurfaces.Default]);
                                    break;

                                case SDL.SDL_Keycode.SDLK_DOWN:
                                    currentTexture = textures[(int)KeyPressSurfaces.Down];
                                    Console.WriteLine("DOWN");
                                    Task.Delay(1500).ContinueWith(_ => currentTexture = textures[(int)KeyPressSurfaces.Default]);
                                    break;

                                case SDL.SDL_Keycode.SDLK_LEFT:
                                    currentTexture = textures[(int)KeyPressSurfaces.Left];
                                    Console.WriteLine("LEFT");
                                    Task.Delay(1500).ContinueWith(_ => currentTexture = textures[(int)KeyPressSurfaces.Default]);
                                    break;

                                case SDL.SDL_Keycode.SDLK_RIGHT:
                                    currentTexture = textures[(int)KeyPressSurfaces.Right];
                                    Console.WriteLine("RIGHT");
                                    Task.Delay(1500).ContinueWith(_ => currentTexture = textures[(int)KeyPressSurfaces.Default]);
                                    break;

                                default:
                                    currentTexture = textures[(int)KeyPressSurfaces.Default];
                                    Console.WriteLine("Default Key Press");
                                    break;
                            }
                            break;
                    }
                }

                if (!driver.Render(currentTexture))
                {
                    Console.WriteLine("Failed to render texture!");
                }

                if (!driver.Present())
                {
                    Console.WriteLine("Failed to present render!");
                }
            }

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

        static bool LoadMedia()
        {
            //Loading success flag
            bool success = true;
            IntPtr surface;

            //Load default surface
            textures[(int)KeyPressSurfaces.Default] = new Texture(driver);
            surface = driver.LoadSurface("assets/press.bmp");
            if (surface == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load default image!");
                success = false;
            }
            else
            {
                textures[(int)KeyPressSurfaces.Default].LoadFromSurface(surface);

                SDL.SDL_FreeSurface(surface);
                surface = IntPtr.Zero;
            }

            //Load up surface
            textures[(int)KeyPressSurfaces.Up] = new Texture(driver);
            surface = driver.LoadSurface("assets/up.bmp");
            if (surface == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load up image!");
                success = false;
            }
            else
            {
                textures[(int)KeyPressSurfaces.Up].LoadFromSurface(surface);

                SDL.SDL_FreeSurface(surface);
                surface = IntPtr.Zero;
            }

            //Load down surface
            textures[(int)KeyPressSurfaces.Down] = new Texture(driver);
            surface = driver.LoadSurface("assets/down.bmp");
            if (surface == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load down image!");
                success = false;
            }
            else
            {
                textures[(int)KeyPressSurfaces.Down].LoadFromSurface(surface);

                SDL.SDL_FreeSurface(surface);
                surface = IntPtr.Zero;

            }

            //Load left surface
            textures[(int)KeyPressSurfaces.Left] = new Texture(driver);
            surface = driver.LoadSurface("assets/left.bmp");
            if (surface == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load left image!");
                success = false;
            }
            else
            {
                textures[(int)KeyPressSurfaces.Left].LoadFromSurface(surface);

                SDL.SDL_FreeSurface(surface);
                surface = IntPtr.Zero;

            }

            //Load right surface
            textures[(int)KeyPressSurfaces.Right] = new Texture(driver);
            surface = driver.LoadSurface("assets/right.bmp");
            if (surface == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load right image!");
                success = false;
            }
            else
            {
                textures[(int)KeyPressSurfaces.Right].LoadFromSurface(surface);

                SDL.SDL_FreeSurface(surface);
                surface = IntPtr.Zero;

            }

            return success;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    currentTexture = null;

                    for (int index = 0; index < (int)KeyPressSurfaces.Count; ++index)
                    {
                        textures[index]?.Dispose();
                        textures[index] = null;
                    }
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
         ~Program() {
           // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
           Dispose(false);
         }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
             GC.SuppressFinalize(this);
        }
        #endregion
    }
}
