using System;
using SDL2;

namespace Chip8
{
    public class SDLDriver
    {
        private static IntPtr windowPtr;
        private static IntPtr screenSurfacePtr;

        public bool Init(int width, int height)
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
                windowPtr = SDL.SDL_CreateWindow("SDL Tutorial", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

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
            if (windowPtr != IntPtr.Zero)
            {
                SDL.SDL_DestroyWindow(windowPtr);
                windowPtr = IntPtr.Zero;
            }

            SDL.SDL_Quit();
        }

        public bool DrawSurface(IntPtr surfacePtr)
        {
            bool result = true;

            //Apply the image
            result &= SDL.SDL_BlitSurface(surfacePtr, IntPtr.Zero, screenSurfacePtr, IntPtr.Zero) == 0;

            //Update the surface
            result &= SDL.SDL_UpdateWindowSurface(windowPtr) == 0;

            return result;
        }
    }
}
