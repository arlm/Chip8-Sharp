using System;
using System.Runtime.InteropServices;
using SDL2;

namespace Chip8
{
    public class Texture : IDisposable
    {
        //The actual hardware texture
        private IntPtr texturePtr;
        private readonly IntPtr rendererPtr;
        private readonly IntPtr fontPtr;

        //Image dimensions
        private int width;
        private int height;

        //Gets image dimensions
        public int Width;
        public int Height;

        //Initializes variables
        public Texture(IntPtr rendererPtr, IntPtr fontPtr)
        {
            this.rendererPtr = rendererPtr;
            this.fontPtr = fontPtr;
        }

        //Loads image at specified path
        public bool LoadFromFile(string path)
        {
            //Get rid of preexisting texture
            Free();

            //The final texture
            IntPtr newTexturePtr = IntPtr.Zero;

            //Render text surface
            var loadedSurfacePtr = SDL_image.IMG_Load(path);

            if (loadedSurfacePtr == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load image {path}! SDL_image Error: {SDL.SDL_GetError()}"); //  SDL_image.IMG_GetError()
            }
            else
            {
                var loadedSurface = Marshal.PtrToStructure<SDL.SDL_Surface>(loadedSurfacePtr);

                //Color key image
                SDL.SDL_SetColorKey(loadedSurfacePtr, (int)SDL.SDL_bool.SDL_TRUE, SDL.SDL_MapRGB(loadedSurface.format, 0, 0xFF, 0xFF));

                //Create texture from surface pixels
                newTexturePtr = SDL.SDL_CreateTextureFromSurface(rendererPtr, loadedSurfacePtr);
                if (texturePtr == IntPtr.Zero)
                {
                    Console.WriteLine($"Unable to create texture from  {path}! SDL Error: {SDL.SDL_GetError()}");
                }
                else
                {
                    //Get image dimensions
                    width = loadedSurface.w;
                    height = loadedSurface.h;
                }

                //Get rid of old surface
                SDL.SDL_FreeSurface(loadedSurfacePtr);
            }

            //Return success
            texturePtr = newTexturePtr;
            return texturePtr != IntPtr.Zero;
        }

        //Creates image from font string
        public bool LoadFromRenderedText(string textureText, SDL.SDL_Color textColor)
        {
            //Get rid of preexisting texture
            Free();

            //Render text surface
            var textSurfacePtr = SDL_ttf.TTF_RenderText_Solid(fontPtr, textureText, textColor);

            if (textSurfacePtr == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to render text surface! SDL_ttf Error: {SDL.SDL_GetError()}"); //  SDL_ttf.TTF_GetError()
            }
            else
            {
                var textSurface = Marshal.PtrToStructure<SDL.SDL_Surface>(textSurfacePtr);

                //Create texture from surface pixels
                texturePtr = SDL.SDL_CreateTextureFromSurface(rendererPtr, textSurfacePtr);
                if (texturePtr == IntPtr.Zero)
                {
                    Console.WriteLine($"Unable to create texture from rendered text! SDL Error: {SDL.SDL_GetError()}");
                }
                else
                {
                    //Get image dimensions
                    width = textSurface.w;
                    height = textSurface.h;
                }

                //Get rid of old surface
                SDL.SDL_FreeSurface(textSurfacePtr);
            }

            //Return success
            return texturePtr != IntPtr.Zero;
        }

        //Set color modulation
        public void SetColor(byte red, byte green, byte blue)
        {
            //Modulate texture rgb
            SDL.SDL_SetTextureColorMod(texturePtr, red, green, blue);
        }

        //Set blending
        public void SetBlendMode(SDL.SDL_BlendMode blending)
        {
            //Set blending function
            SDL.SDL_SetTextureBlendMode(texturePtr, blending);
        }

        //Set alpha modulation
        public void SetAlpha(byte alpha)
        {
            //Modulate texture alpha
            SDL.SDL_SetTextureAlphaMod(texturePtr, alpha);
        }

        //Renders texture at given point
        public void Render(int x, int y, SDL.SDL_Rect? clip = null, double angle = 0.0, SDL.SDL_Point? center = null, SDL.SDL_RendererFlip flip = SDL.SDL_RendererFlip.SDL_FLIP_NONE)
        {
            //Set rendering space and render to screen
            var renderQuad = new SDL.SDL_Rect { x = x, y = y, w = width, h = height };

            //Set clip rendering dimensions
            if (clip.HasValue)
            {
                renderQuad.w = clip.Value.w;
                renderQuad.h = clip.Value.h;
            }

            //Render to screen
            if (clip.HasValue && center.HasValue)
            {
                var refClip = clip.Value;
                var refCenter = center.Value;

                SDL.SDL_RenderCopyEx(rendererPtr, texturePtr, ref refClip, ref renderQuad, angle, ref refCenter, flip);
            }
            else if (clip.HasValue)
            {
                var refClip = clip.Value;

                SDL.SDL_RenderCopyEx(rendererPtr, texturePtr, ref refClip, ref renderQuad, angle, IntPtr.Zero, flip);
            }
            else if (center.HasValue)
            {
                var refCenter = center.Value;

                SDL.SDL_RenderCopyEx(rendererPtr, texturePtr, IntPtr.Zero, ref renderQuad, angle, ref refCenter, flip);
            }
            else
            {
                SDL.SDL_RenderCopyEx(rendererPtr, texturePtr, IntPtr.Zero, ref renderQuad, angle, IntPtr.Zero, flip);
            }
        }

        private void Free()
        {
            //Free texture if it exists
            if (texturePtr != IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(texturePtr);
                texturePtr = IntPtr.Zero;
                width = 0;
                height = 0;
            }
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
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.
                Free();

                disposedValue = true;
            }
        }

        // Override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~Texture()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            // Uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
