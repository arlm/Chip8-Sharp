using System;
using SDL2;

namespace Chip8
{
    public class SDLDriver
    {
        private IntPtr windowPtr;
        private IntPtr screenSurfacePtr;
        private IntPtr rendererPtr;
        private IntPtr fontPtr;
        private Texture textTexture;

        private int width;
        private int height;

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

                        //Get window surface
                        screenSurfacePtr = SDL.SDL_GetWindowSurface(windowPtr);
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
                                Quit();
                                return false;
                            }
                        }

                        //Apply the image
                        SDL.SDL_BlitSurface(splashSurfacePtr, IntPtr.Zero, screenSurfacePtr, IntPtr.Zero);

                        //Update the surface
                        SDL.SDL_UpdateWindowSurface(windowPtr);

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

            //Open the font
            fontPtr = SDL_ttf.TTF_OpenFont("assets/lazy.ttf", 18);

            if (fontPtr == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to load lazy font! SDL_ttf Error: {SDL.SDL_GetError()}");
                result = false;
            }
            else
            {
                textTexture = new Texture(rendererPtr, fontPtr);

                //Render text
                SDL.SDL_Color textColor = new SDL.SDL_Color { r = 0, g = 0, b = 0 };
                if (!textTexture.LoadFromRenderedText("The quick brown fox jumps over the lazy dog", textColor))
                {
                    Console.WriteLine("Failed to render text texture!\n");
                    result = false;
                }
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
                        Quit();
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

        public IntPtr LoadSurface(string fileName)
        {
            //Load splash image
            var loadedSurface = SDL.SDL_LoadBMP(fileName);

            if (loadedSurface == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load image {fileName}! SDL Error: {SDL.SDL_GetError()}");
            }

            return loadedSurface;
        }

        public void Quit()
        {
            textTexture?.Dispose();
            textTexture = null;

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
        }

        public bool DrawSurface(IntPtr surfacePtr)
        {
            //Initialization flag
            bool result = true;

            //Apply the image
            result &= SDL.SDL_BlitSurface(surfacePtr, IntPtr.Zero, screenSurfacePtr, IntPtr.Zero) == 0;

            //Update the surface
            result &= SDL.SDL_UpdateWindowSurface(windowPtr) == 0;

            return result;
        }

        public bool Render()
        {
            //Initialization flag
            bool result = true;

            //Clear screen
            SDL.SDL_SetRenderDrawColor(rendererPtr, 0xFF, 0xFF, 0xFF, 0xFF);
            SDL.SDL_RenderClear(rendererPtr);

            //Render current frame
            textTexture.Render(48, height - 32);

            //Update screen
            SDL.SDL_RenderPresent(rendererPtr);
            return result;
        }
    }
}
