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
        private static readonly SDL.SDL_Color textColor = new SDL.SDL_Color { r = 0, g = 0, b = 0, a = 255 };
        private static string timerText;
        private static Texture fpsTextTexture;

        private static readonly Timer fpsTimer = new Timer();
        private static int countedFrames;

        private static byte[] keys = 
        {
             0x00, 0x00, 0x00, 0x00,
             0x00, 0x00, 0x00, 0x00,
             0x00, 0x00, 0x00, 0x00,
             0x00, 0x00, 0x00, 0x00
         };

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

            // Initialize the CHIP-8 system (Clear the memory, registers and screen)
            myChip8 = new CPU();

            // Load (copy) the game into the memory
            myChip8.LoadGame($"progs{Path.DirectorySeparatorChar}pong2.c8");

            //Start counting frames per second
            countedFrames = 0;
            fpsTimer.Start();

            //While application is running
            while (!quit)
            {

                // Emulate one cycle of the system
                myChip8.EmulateCycle();

                // If the draw flag is set, update the screen
                // Because the system does not draw every cycle, we should set a draw flag when we need to update our screen.
                // Only two opcodes should set this flag:
                //    0x00E0 – Clears the screen
                //    0xDXYN – Draws a sprite on the screen

                //if (myChip8.DrawFlag)
                //{
                //    DrawGraphics();
                //}

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
                                    break;

                                case SDL.SDL_Keycode.SDLK_DOWN:
                                    currentTexture = textures[(int)KeyPressSurfaces.Down];
                                    Console.WriteLine("DOWN");
                                    break;

                                case SDL.SDL_Keycode.SDLK_LEFT:
                                    currentTexture = textures[(int)KeyPressSurfaces.Left];
                                    Console.WriteLine("LEFT");
                                    break;

                                case SDL.SDL_Keycode.SDLK_RIGHT:
                                    currentTexture = textures[(int)KeyPressSurfaces.Right];
                                    Console.WriteLine("RIGHT");
                                    break;

                                case SDL.SDL_Keycode.SDLK_1:
                                    keys[0x0] = 1;
                                    Console.WriteLine("1");
                                    break;
                                case SDL.SDL_Keycode.SDLK_2:
                                    keys[0x1] = 1;
                                    Console.WriteLine("2");
                                    break;
                                case SDL.SDL_Keycode.SDLK_3:
                                    keys[0x2] = 1;
                                    Console.WriteLine("3");
                                    break;
                                case SDL.SDL_Keycode.SDLK_4:
                                    keys[0x3] = 1;
                                    Console.WriteLine("4");
                                    break;

                                case SDL.SDL_Keycode.SDLK_q:
                                    keys[0x4] = 1;
                                    Console.WriteLine("q");
                                    break;
                                case SDL.SDL_Keycode.SDLK_w:
                                    keys[0x5] = 1;
                                    Console.WriteLine("w");
                                    break;
                                case SDL.SDL_Keycode.SDLK_e:
                                    keys[0x6] = 1;
                                    Console.WriteLine("e");
                                    break;
                                case SDL.SDL_Keycode.SDLK_r:
                                    keys[0x7] = 1;
                                    Console.WriteLine("r");
                                    break;

                                case SDL.SDL_Keycode.SDLK_a:
                                    keys[0x8] = 1;
                                    Console.WriteLine("a");
                                    break;
                                case SDL.SDL_Keycode.SDLK_s:
                                    keys[0x9] = 1;
                                    Console.WriteLine("s");
                                    break;
                                case SDL.SDL_Keycode.SDLK_d:
                                    keys[0xA] = 1;
                                    Console.WriteLine("d");
                                    break;
                                case SDL.SDL_Keycode.SDLK_f:
                                    keys[0xA] = 1;
                                    Console.WriteLine("f");
                                    break;

                                case SDL.SDL_Keycode.SDLK_z:
                                    keys[0xB] = 1;
                                    Console.WriteLine("z");
                                    break;
                                case SDL.SDL_Keycode.SDLK_y:
                                    keys[0xB] = 1;
                                    Console.WriteLine("y");
                                    break;
                                case SDL.SDL_Keycode.SDLK_x:
                                    keys[0xC] = 1;
                                    Console.WriteLine("x");
                                    break;
                                case SDL.SDL_Keycode.SDLK_c:
                                    keys[0xD] = 1;
                                    Console.WriteLine("c");
                                    break;
                                case SDL.SDL_Keycode.SDLK_v:
                                    keys[0xE] = 1;
                                    Console.WriteLine("v");
                                    break;

                                default:
                                    currentTexture = textures[(int)KeyPressSurfaces.Default];
                                    Console.WriteLine("Default Key Press");
                                    break;
                            }
                            break;

                        case SDL.SDL_EventType.SDL_KEYUP:
                            //Select surfaces based on key press
                            switch (evt.key.keysym.sym)
                            {
                                case SDL.SDL_Keycode.SDLK_UP:
                                    currentTexture = textures[(int)KeyPressSurfaces.Default];
                                    break;

                                case SDL.SDL_Keycode.SDLK_DOWN:
                                    currentTexture = textures[(int)KeyPressSurfaces.Default];
                                    break;

                                case SDL.SDL_Keycode.SDLK_LEFT:
                                    currentTexture = textures[(int)KeyPressSurfaces.Default];
                                    break;

                                case SDL.SDL_Keycode.SDLK_RIGHT:
                                    currentTexture = textures[(int)KeyPressSurfaces.Default];
                                    break;

                                default:
                                    currentTexture = textures[(int)KeyPressSurfaces.Default];
                                    break;
                            }
                            break;
                    }
                }

                if (!driver.Render(currentTexture))
                {
                    Console.WriteLine("Failed to render texture!");
                }

                // Store key press state (Press and Release)
                // If we press or release a key, we should store this state in the part that emulates the keypad
                myChip8.SetKeys(keys);

                // 60 Hz = 1000 ms /60 = 16.666 ms  => approx. 17 ms (17 ms * 60 = 1020 ms)
                //SDL.SDL_Delay(17);

                //Calculate and correct fps
                var avgFPS = countedFrames / (fpsTimer.Ticks / 1000.0);

                // It is possible for there to be a very small amount of time passed
                // for the first frame and have it give us a really high fps.
                // This is why we correct the value if it is really high.
                if (avgFPS > 2000000)
                {
                    avgFPS = 0;
                }

                //Set text to be rendered
                timerText = $"Average {avgFPS:0.00} Frames Per Second";

                //Render text
                if (!fpsTextTexture.LoadFromRenderedText(timerText, textColor))
                {
                    Console.WriteLine("Unable to render FPS texture!");
                }

                //Clear screen
                //SDL.SDL_SetRenderDrawColor(driver.rendererPtr, 0xFF, 0xFF, 0xFF, 0xFF);
                //SDL.SDL_RenderClear(driver.rendererPtr);

                //Render textures
                fpsTextTexture.Render((WIDTH - fpsTextTexture.Width) / 2, (HEIGHT - (int)(fpsTextTexture.Height * 1.75)));

                //Update screen
                if (!driver.Present())
                {
                    Console.WriteLine("Failed to present render!");
                }

                ++countedFrames;
            }

            return 0;
        }

        static bool LoadMedia()
        {
            //Loading success flag
            bool success = true;
            IntPtr surface;

            fpsTextTexture = new Texture(driver);

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

                    fpsTextTexture?.Dispose();
                    fpsTextTexture = null;

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
