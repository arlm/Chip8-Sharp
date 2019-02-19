using System;
using System.IO;
using Chip8.Core;
using SDL2;

namespace Chip8
{
    public enum KeyPressSurface
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
        private static ICPU myChip8;

        private const int WIDTH = 640;
        private const int HEIGHT = 480;

        private static readonly Texture[] textures = new Texture[(int)KeyPressSurface.Count];
        private static Texture currentTexture;

        //Set text color as black
        private static readonly SDL.SDL_Color black = new SDL.SDL_Color { r = 0x00, g = 0x00, b = 0x00, a = 0xFF };
        private static readonly SDL.SDL_Color white = new SDL.SDL_Color { r = 0xFF, g = 0xFF, b = 0xFF, a = 0xFF };

        private static readonly SDL.SDL_Color ambar = new SDL.SDL_Color { r = 0xFF, g = 0xB0, b = 0x00, a = 0xFF };
        private static readonly SDL.SDL_Color lightAmbar = new SDL.SDL_Color { r = 0xFF, g = 0xCC, b = 0x00, a = 0xFF };
        private static readonly SDL.SDL_Color green1 = new SDL.SDL_Color { r = 0x33, g = 0xFF, b = 0x00, a = 0xFF };
        private static readonly SDL.SDL_Color green2 = new SDL.SDL_Color { r = 0x00, g = 0xFF, b = 0x33, a = 0xFF };
        private static readonly SDL.SDL_Color green3 = new SDL.SDL_Color { r = 0x00, g = 0xFF, b = 0x66, a = 0xFF };
        private static readonly SDL.SDL_Color appleIIGreen = new SDL.SDL_Color { r = 0x33, g = 0xFF, b = 0x33, a = 0xFF };
        private static readonly SDL.SDL_Color appleIIcGreen = new SDL.SDL_Color { r = 0x66, g = 0xFF, b = 0x66, a = 0xFF };
        private static readonly SDL.SDL_Color gray = new SDL.SDL_Color { r = 0x28, g = 0x28, b = 0x28, a = 0xFF };

        private static string timerText;
        private static Texture fpsTextTexture;
        private static Texture pixelDebugTexture;
        private static Texture pixelTexture;
        private static bool debugKeys = false;
        private static bool debugPixels = false;

        private static float zoom = 9.5f;
        private static SDL.SDL_Point screenCenter;

        private static readonly Timer fpsTimer = new Timer();
        private static int countedFrames;

        private static readonly byte[] keys =
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
            currentTexture = textures[(int)KeyPressSurface.Default];
            Console.WriteLine("Loaded...");

            // Initialize the CHIP-8 system (Clear the memory, registers and screen)
            myChip8 = new CPU();
            myChip8.OnDraw += DrawGraphics;
            myChip8.OnStartSound += OnStartSound;
            myChip8.OnEndSound += OnEndSound;

            // Load (copy) the game into the memory
            myChip8.LoadGame($"progs{Path.DirectorySeparatorChar}demo.c8");

            //Rotation variables
            double angle = 0;

            screenCenter = new SDL.SDL_Point
            {
                x = WIDTH / 2,
                y = HEIGHT / 2
            };

            //Start counting frames per second
            countedFrames = 0;
            fpsTimer.Start();

            //While application is running
            while (!quit)
            {
                if (!debugKeys && !debugPixels)
                {
                    // Emulate one cycle of the system
                    myChip8.EmulateCycle();
                }

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
                                    currentTexture = textures[(int)KeyPressSurface.Up];
                                    Console.WriteLine("UP");
                                    break;

                                case SDL.SDL_Keycode.SDLK_DOWN:
                                    currentTexture = textures[(int)KeyPressSurface.Down];
                                    Console.WriteLine("DOWN");
                                    break;

                                case SDL.SDL_Keycode.SDLK_LEFT:
                                    currentTexture = textures[(int)KeyPressSurface.Left];
                                    Console.WriteLine("LEFT");
                                    break;

                                case SDL.SDL_Keycode.SDLK_RIGHT:
                                    currentTexture = textures[(int)KeyPressSurface.Right];
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

                                case SDL.SDL_Keycode.SDLK_BACKSPACE:
                                    debugKeys = false;
                                    debugPixels = !debugPixels;

                                    if (debugPixels)
                                    {
                                        Console.WriteLine("Entering debug pixel mode");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Leaving debug pixel mode");
                                    }
                                    break;
                                case SDL.SDL_Keycode.SDLK_RETURN:
                                    debugKeys = !debugKeys;
                                    debugPixels = false;

                                    if (debugKeys)
                                    {
                                        Console.WriteLine("Entering debug keys mode");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Leaving debug keys mode");
                                    }
                                    break;

                                case SDL.SDL_Keycode.SDLK_PLUS:
                                    zoom += 0.5f;
                                    Console.WriteLine($"Zoom level: {zoom}x");
                                    break;
                                case SDL.SDL_Keycode.SDLK_MINUS:
                                    zoom -= 0.5f;
                                    Console.WriteLine($"Zoom level: {zoom}x");
                                    break;
                                case SDL.SDL_Keycode.SDLK_0:
                                    zoom = 1.0f;
                                    Console.WriteLine($"Zoom level: {zoom}x");
                                    break;

                                default:
                                    currentTexture = textures[(int)KeyPressSurface.Default];
                                    Console.WriteLine("Default Key Press");
                                    break;
                            }
                            break;

                        case SDL.SDL_EventType.SDL_KEYUP:
                            //Select surfaces based on key press
                            switch (evt.key.keysym.sym)
                            {
                                case SDL.SDL_Keycode.SDLK_UP:
                                    currentTexture = textures[(int)KeyPressSurface.Default];
                                    break;

                                case SDL.SDL_Keycode.SDLK_DOWN:
                                    currentTexture = textures[(int)KeyPressSurface.Default];
                                    break;

                                case SDL.SDL_Keycode.SDLK_LEFT:
                                    currentTexture = textures[(int)KeyPressSurface.Default];
                                    break;

                                case SDL.SDL_Keycode.SDLK_RIGHT:
                                    currentTexture = textures[(int)KeyPressSurface.Default];
                                    break;

                                case SDL.SDL_Keycode.SDLK_1:
                                    keys[0x0] = 0;
                                    Console.WriteLine("1");
                                    break;
                                case SDL.SDL_Keycode.SDLK_2:
                                    keys[0x1] = 0;
                                    Console.WriteLine("2");
                                    break;
                                case SDL.SDL_Keycode.SDLK_3:
                                    keys[0x2] = 0;
                                    Console.WriteLine("3");
                                    break;
                                case SDL.SDL_Keycode.SDLK_4:
                                    keys[0x3] = 0;
                                    Console.WriteLine("4");
                                    break;

                                case SDL.SDL_Keycode.SDLK_q:
                                    keys[0x4] = 0;
                                    Console.WriteLine("q");
                                    break;
                                case SDL.SDL_Keycode.SDLK_w:
                                    keys[0x5] = 0;
                                    Console.WriteLine("w");
                                    break;
                                case SDL.SDL_Keycode.SDLK_e:
                                    keys[0x6] = 0;
                                    Console.WriteLine("e");
                                    break;
                                case SDL.SDL_Keycode.SDLK_r:
                                    keys[0x7] = 0;
                                    Console.WriteLine("r");
                                    break;

                                case SDL.SDL_Keycode.SDLK_a:
                                    keys[0x8] = 0;
                                    Console.WriteLine("a");
                                    break;
                                case SDL.SDL_Keycode.SDLK_s:
                                    keys[0x9] = 0;
                                    Console.WriteLine("s");
                                    break;
                                case SDL.SDL_Keycode.SDLK_d:
                                    keys[0xA] = 0;
                                    Console.WriteLine("d");
                                    break;
                                case SDL.SDL_Keycode.SDLK_f:
                                    keys[0xA] = 0;
                                    Console.WriteLine("f");
                                    break;

                                case SDL.SDL_Keycode.SDLK_z:
                                    keys[0xB] = 0;
                                    break;
                                case SDL.SDL_Keycode.SDLK_y:
                                    keys[0xB] = 0;
                                    break;
                                case SDL.SDL_Keycode.SDLK_x:
                                    keys[0xC] = 0;
                                    break;
                                case SDL.SDL_Keycode.SDLK_c:
                                    keys[0xD] = 0;
                                    break;
                                case SDL.SDL_Keycode.SDLK_v:
                                    keys[0xE] = 0;
                                    break;

                                default:
                                    currentTexture = textures[(int)KeyPressSurface.Default];
                                    break;
                            }
                            break;
                    }
                }

                if (debugKeys)
                {
                    if (!driver.Render(currentTexture))
                    {
                        Console.WriteLine("Failed to render texture!");
                    }
                }

                if (debugPixels)
                {
                    angle = RotateRectangle(angle);
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

                SDL.SDL_Color color = white;

                if (debugKeys) { color = black; }
                if (debugPixels) { color = green1; }

                //Render text
                if (!fpsTextTexture.LoadFromRenderedText(timerText, color))
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

        private static double RotateRectangle(double angle)
        {
            //rotate
            angle += 2;
            if (angle > 360)
            {
                angle -= 360;
            }

            //Set self as render target
            pixelDebugTexture.SetAsRenderTarget();

            //Clear screen
            SDL.SDL_SetRenderDrawColor(driver.rendererPtr, 0xFF, 0xFF, 0xFF, 0xFF);
            SDL.SDL_RenderClear(driver.rendererPtr);

            //Render red filled quad
            SDL.SDL_Rect fillRect = new SDL.SDL_Rect
            {
                x = WIDTH / 4,
                y = HEIGHT / 4,
                w = WIDTH / 2,
                h = HEIGHT / 2
            };

            SDL.SDL_SetRenderDrawColor(driver.rendererPtr, 0xFF, 0x00, 0x00, 0xFF);
            SDL.SDL_RenderFillRect(driver.rendererPtr, ref fillRect);

            //Render green outlined quad
            SDL.SDL_Rect outlineRect = new SDL.SDL_Rect
            {
                x = WIDTH / 6,
                y = HEIGHT / 6,
                w = WIDTH * 2 / 3,
                h = HEIGHT * 2 / 3
            };

            SDL.SDL_SetRenderDrawColor(driver.rendererPtr, 0x00, 0xFF, 0x00, 0xFF);
            SDL.SDL_RenderDrawRect(driver.rendererPtr, ref outlineRect);

            //Draw blue horizontal line
            SDL.SDL_SetRenderDrawColor(driver.rendererPtr, 0x00, 0x00, 0xFF, 0xFF);
            SDL.SDL_RenderDrawLine(driver.rendererPtr, 0, HEIGHT / 2, WIDTH, HEIGHT / 2);

            //Draw vertical line of yellow dots
            SDL.SDL_SetRenderDrawColor(driver.rendererPtr, 0xFF, 0xFF, 0x00, 0xFF);
            for (int i = 0; i < HEIGHT; i += 4)
            {
                SDL.SDL_RenderDrawPoint(driver.rendererPtr, WIDTH / 2, i);
            }

            //Reset render target
            SDL.SDL_SetRenderTarget(driver.rendererPtr, IntPtr.Zero);

            //Show rendered to texture
            pixelDebugTexture.Render(0, 0, null, angle, screenCenter);
            return angle;
        }


        // SDL.SDL_Point screenCenter, float scale
        private static void DrawGraphics(byte[] graphics)
        {
            //Set self as render target
            pixelDebugTexture.SetAsRenderTarget();

            SDL.SDL_Color color = black;

            SDL.SDL_SetRenderDrawColor(driver.rendererPtr, color.r, color.g, color.b, color.a);
            SDL.SDL_RenderClear(driver.rendererPtr);
            SDL.SDL_RenderSetScale(driver.rendererPtr, zoom, zoom);

            color = gray;

            //Render red filled quad
            SDL.SDL_Rect fillRect = new SDL.SDL_Rect
            {
                x = 0,
                y = 0,
                w = CPU.WIDTH,
                h = CPU.HEIGHT
            };

            SDL.SDL_SetRenderDrawColor(driver.rendererPtr, color.r, color.g, color.b, color.a);
            SDL.SDL_RenderFillRect(driver.rendererPtr, ref fillRect);

            color = appleIIGreen;

            SDL.SDL_SetRenderDrawColor(driver.rendererPtr, color.r, color.g, color.b, color.a);

            for(int index = 0; index < graphics.Length; index++)
            {
                if (graphics[index] == 1)
                {
                    var y = index / CPU.WIDTH;
                    var x = index % CPU.WIDTH;

                    SDL.SDL_RenderDrawPoint(driver.rendererPtr, x, y);
                }
            }

            //Reset render target
            SDL.SDL_SetRenderTarget(driver.rendererPtr, IntPtr.Zero);

            //Show rendered to texture
            pixelDebugTexture.Render(0, 0, null, 0, screenCenter);
        }

        static bool LoadMedia()
        {
            //Loading success flag
            bool success = true;
            IntPtr surface;

            fpsTextTexture = new Texture(driver);

            //Load default surface
            textures[(int)KeyPressSurface.Default] = new Texture(driver);
            surface = driver.LoadSurface("assets/press.bmp");
            if (surface == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load default image!");
                success = false;
            }
            else
            {
                textures[(int)KeyPressSurface.Default].LoadFromSurface(surface);

                SDL.SDL_FreeSurface(surface);
                surface = IntPtr.Zero;
            }

            //Load up surface
            textures[(int)KeyPressSurface.Up] = new Texture(driver);
            surface = driver.LoadSurface("assets/up.bmp");
            if (surface == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load up image!");
                success = false;
            }
            else
            {
                textures[(int)KeyPressSurface.Up].LoadFromSurface(surface);

                SDL.SDL_FreeSurface(surface);
                surface = IntPtr.Zero;
            }

            //Load down surface
            textures[(int)KeyPressSurface.Down] = new Texture(driver);
            surface = driver.LoadSurface("assets/down.bmp");
            if (surface == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load down image!");
                success = false;
            }
            else
            {
                textures[(int)KeyPressSurface.Down].LoadFromSurface(surface);

                SDL.SDL_FreeSurface(surface);
                surface = IntPtr.Zero;

            }

            //Load left surface
            textures[(int)KeyPressSurface.Left] = new Texture(driver);
            surface = driver.LoadSurface("assets/left.bmp");
            if (surface == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load left image!");
                success = false;
            }
            else
            {
                textures[(int)KeyPressSurface.Left].LoadFromSurface(surface);

                SDL.SDL_FreeSurface(surface);
                surface = IntPtr.Zero;

            }

            //Load right surface
            textures[(int)KeyPressSurface.Right] = new Texture(driver);
            surface = driver.LoadSurface("assets/right.bmp");
            if (surface == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load right image!");
                success = false;
            }
            else
            {
                textures[(int)KeyPressSurface.Right].LoadFromSurface(surface);

                SDL.SDL_FreeSurface(surface);
                surface = IntPtr.Zero;

            }

            pixelDebugTexture = new Texture(driver);
            if (!pixelDebugTexture.CreateBlank(WIDTH, HEIGHT, SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET))
            {
                Console.WriteLine("Failed to create target debug texture!");
                success = false;
            }

            pixelTexture = new Texture(driver);
            if (!pixelTexture.CreateBlank(WIDTH, HEIGHT, SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET))
            {
                Console.WriteLine("Failed to create target texture!");
                success = false;
            }

            return success;
        }

        private static void OnStartSound(int channel, int frequency, int duration)
        {
            Console.Beep(frequency, duration);
        }

        private static void OnEndSound(int channel)
        {
            Console.WriteLine("BEEP!");
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

                    for (int index = 0; index < (int)KeyPressSurface.Count; ++index)
                    {
                        textures[index]?.Dispose();
                        textures[index] = null;
                    }

                    pixelTexture?.Dispose();
                    pixelTexture = null;

                    pixelDebugTexture?.Dispose();
                    pixelDebugTexture = null;
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~Program()
        {
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
