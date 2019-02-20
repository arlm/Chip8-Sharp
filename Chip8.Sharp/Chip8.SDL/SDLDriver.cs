using System;
using System.Collections.Generic;
using SDL2;

namespace Chip8
{
    public class SDLDriver : IDisposable
    {
        private Texture videoTexture;

        internal IntPtr windowPtr;
        internal IntPtr rendererPtr;
        internal IntPtr fontPtr;

        internal int width;
        internal int height;

        public List<Texture> Textures { get; } = new List<Texture>();

        public bool Init(int width, int height)
        {
            //Initialization flag
            bool result = true;

            this.width = width;
            this.height = height;

            //Initialize SDL
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine($"SDL could not initialize! SDL_Error: {SDL.SDL_GetError()}");

                result = false;
            }
            else
            {
                //Set texture filtering to linear
                if (SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "1") == SDL.SDL_bool.SDL_FALSE)
                {
                    Console.WriteLine("Warning: Linear texture filtering not enabled!");
                }

                //Create window
                windowPtr = SDL.SDL_CreateWindow("SDL Tutorial", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

                if (windowPtr == IntPtr.Zero)
                {
                    Console.WriteLine($"Window could not be created! SDL_Error: {SDL.SDL_GetError()}");
                    result = false;
                }
                else
                {
                    //Create vsynced renderer for window
                    rendererPtr = SDL.SDL_CreateRenderer(windowPtr, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

                    if (rendererPtr == IntPtr.Zero)
                    {
                        Console.WriteLine($"Renderer could not be created! SDL_Error: {SDL.SDL_GetError()}");
                        result = false;
                    }
                    else
                    {
                        // Initialize renderer color
                        SDL.SDL_SetRenderDrawColor(rendererPtr, 0xFF, 0xFF, 0xFF, 0xFF);

                        videoTexture = new Texture(rendererPtr, fontPtr);
                    }
                }
            }

            //Initialize PNG loading
            if ((((SDL_image.IMG_InitFlags)SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG)) & SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0)
            {
                Console.WriteLine($"SDL_image could not initialize! SDL_image Error: {SDL.SDL_GetError()}");
                result = false;
            }

            //Initialize SDL_ttf
            if (SDL_ttf.TTF_Init() == -1)
            {
                Console.WriteLine($"SDL_ttf could not initialize! SDL_ttf Error: {SDL.SDL_GetError()}");
                result = false;
            }

            //Open the font
            fontPtr = SDL_ttf.TTF_OpenFont("assets/lazy.ttf", 18);

            if (fontPtr == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to load lazy font! SDL_ttf Error: {SDL.SDL_GetError()}");
                result = false;
            }

            return result;
        }

        public bool SetupGraphics(string fileName)
        {
            //Initialization flag
            bool result = true;

            IntPtr splashSurfacePtr = IntPtr.Zero;

            try
            {
                splashSurfacePtr = LoadSurface(fileName);
                result &= splashSurfacePtr != IntPtr.Zero;
                videoTexture.LoadFromSurface(splashSurfacePtr);

                if (result)
                {
                    const int INTERVAL = 250;
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
                                Dispose();
                                return false;
                            }
                        }

                        //Render current frame
                        videoTexture.Render(0, 0);

                        //Update screen
                        SDL.SDL_RenderPresent(rendererPtr);

                        SDL.SDL_Delay(INTERVAL);
                        counter -= INTERVAL;
                    }
                }
            }
            finally
            {
                if (splashSurfacePtr != IntPtr.Zero)
                {
                    SDL.SDL_FreeSurface(splashSurfacePtr);
                    splashSurfacePtr = IntPtr.Zero;
                }

                SDL.SDL_RenderClear(rendererPtr);
            }

            return result;
        }

        public bool SetupInput()
        {
            //Initialization flag
            bool result = true;

            while (SDL.SDL_PollEvent(out var evt) > 0)
            {
                switch (evt.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        Dispose();
                        result = false;
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

            return result;
        }

        public static IntPtr LoadSurface(string fileName)
        {
            //Load splash image
            var loadedSurface = SDL.SDL_LoadBMP(fileName);

            if (loadedSurface == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load image {fileName}! SDL Error: {SDL.SDL_GetError()}");
            }

            return loadedSurface;
        }

        public bool ClearScreen(byte r, byte g, byte b)
        {
            //Initialization flag
            bool result = true;

            SDL.SDL_SetRenderDrawColor(rendererPtr, r, g, b, 0xFF);
            SDL.SDL_RenderClear(rendererPtr);

            return result;
        }

        public static bool Render(Texture texture, int x = 0, int y = 0, in SDL.SDL_Rect? clip = null)
        {
            //Initialization flag
            bool result = true;

            //result &= ClearScreen(0xFF, 0xFF, 0xFF);

            //Render current frame
            texture.Render(0, 0, clip);

            return result;
        }

        public bool Present()
        {
            //Initialization flag
            bool result = true;

            //Update screen
            SDL.SDL_RenderPresent(rendererPtr);

            return result;
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).

                    videoTexture?.Dispose();
                    videoTexture = null;
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                if (fontPtr != IntPtr.Zero)
                {
                    SDL_ttf.TTF_CloseFont(fontPtr);
                    fontPtr = IntPtr.Zero;
                }

                if (rendererPtr != IntPtr.Zero)
                {
                    SDL.SDL_DestroyRenderer(rendererPtr);
                    rendererPtr = IntPtr.Zero;
                }

                if (windowPtr != IntPtr.Zero)
                {
                    SDL.SDL_DestroyWindow(windowPtr);
                    windowPtr = IntPtr.Zero;
                }

                //Quit SDL subsystems
                SDL_ttf.TTF_Quit();
                SDL_image.IMG_Quit();
                SDL.SDL_Quit();

                disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~SDLDriver()
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
