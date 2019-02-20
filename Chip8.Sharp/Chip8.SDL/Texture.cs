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
        private IntPtr pixelsPtr;
        private byte[] pixels;
        private int pitch;

        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public SDL.SDL_Rect? Clip { get; set; }

        //Initializes variables
        public Texture(IntPtr rendererPtr, IntPtr fontPtr)
        {
            this.rendererPtr = rendererPtr;
            this.fontPtr = fontPtr;
        }

        public Texture(SDLDriver driver)
        {
            this.rendererPtr = driver.rendererPtr;
            this.fontPtr = driver.fontPtr;
        }

        //Loads image at specified path
        public bool LoadFromSurface(IntPtr loadedSurfacePtr)
        {
            if (texturePtr != IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(texturePtr);
                texturePtr = IntPtr.Zero;
            }

            //The final texture
            var newTexturePtr = IntPtr.Zero;

            if (loadedSurfacePtr == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load surface! SDL_image Error: {SDL.SDL_GetError()}");
            }
            else
            {
                var loadedSurface = Marshal.PtrToStructure<SDL.SDL_Surface>(loadedSurfacePtr);

                //Color key image
                SDL.SDL_SetColorKey(loadedSurfacePtr, (int)SDL.SDL_bool.SDL_TRUE, SDL.SDL_MapRGB(loadedSurface.format, 0, 0xFF, 0xFF));

                //Create texture from surface pixels
                newTexturePtr = SDL.SDL_CreateTextureFromSurface(rendererPtr, loadedSurfacePtr);
                if (newTexturePtr == IntPtr.Zero)
                {
                    Console.WriteLine($"Unable to create texture from surface! SDL Error: {SDL.SDL_GetError()}");
                }
                else
                {
                    //Get image dimensions
                    Width = loadedSurface.w;
                    Height = loadedSurface.h;
                }
            }

            //Return success
            texturePtr = newTexturePtr;
            return texturePtr != IntPtr.Zero;
        }

        //Loads image at specified path
        public bool LoadFromFile(string path)
        {
            if (texturePtr != IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(texturePtr);
                texturePtr = IntPtr.Zero;
            }

            //The final texture
            var newTexturePtr = IntPtr.Zero;

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
                if (newTexturePtr == IntPtr.Zero)
                {
                    Console.WriteLine($"Unable to create texture from  {path}! SDL Error: {SDL.SDL_GetError()}");
                }
                else
                {
                    //Get image dimensions
                    Width = loadedSurface.w;
                    Height = loadedSurface.h;
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
            if (texturePtr != IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(texturePtr);
                texturePtr = IntPtr.Zero;
            }

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
                    Width = textSurface.w;
                    Height = textSurface.h;
                }

                //Get rid of old surface
                SDL.SDL_FreeSurface(textSurfacePtr);
            }

            //Return success
            return texturePtr != IntPtr.Zero;
        }

        public bool CreateBlank(int width, int height, SDL.SDL_TextureAccess access)
        {
            //Create uninitialized texture
            texturePtr = SDL.SDL_CreateTexture(rendererPtr, SDL.SDL_PIXELFORMAT_RGBA8888, (int)access, width, height);

            if (texturePtr == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to create blank texture! SDL Error: {SDL.SDL_GetError()}\n");
            }
            else
            {
                Width = width;
                Height = height;
            }

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
        public void Render(int? x = null, int? y = null, in SDL.SDL_Rect? clip = null, double angle = 0.0, in SDL.SDL_Point? center = null, SDL.SDL_RendererFlip flip = SDL.SDL_RendererFlip.SDL_FLIP_NONE)
        {
            //Set rendering space and render to screen
            var renderQuad = new SDL.SDL_Rect
            {
                x = x.HasValue ? x.Value : X,
                y = y.HasValue ? y.Value : Y,
                w = Width,
                h = Height
            };

            //Set clip rendering dimensions
            if (clip.HasValue)
            {
                renderQuad.w = clip.Value.w;
                renderQuad.h = clip.Value.h;
            }
            else if (this.Clip.HasValue)
            {
                renderQuad.w = this.Clip.Value.w;
                renderQuad.h = this.Clip.Value.h;
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

        //Set self as render target
        public void SetAsRenderTarget()
        {
            //Make self render target
            SDL.SDL_SetRenderTarget(rendererPtr, texturePtr);
        }

        public bool Lock()
        {
            var success = true;

            //Texture is already locked
            if (pixelsPtr != IntPtr.Zero)
            {
                Console.WriteLine("Texture is already locked!\n");
                success = false;
            }
            //Lock texture
            else
            {
                if (SDL.SDL_LockTexture(texturePtr, IntPtr.Zero, out pixelsPtr, out pitch) != 0)
                {
                    Console.WriteLine($"Unable to lock texture! {SDL.SDL_GetError()}\n");
                    success = false;
                }
                else
                {
                    pixels = new byte[pitch * Height * 4];
                    Marshal.Copy(pixelsPtr, pixels, 0, pitch * Height * 4);
                }
            }

            return success;
        }

        public bool Unlock()
        {
            var success = true;

            //Texture is not locked
            if (pixelsPtr == IntPtr.Zero)
            {
                Console.WriteLine("Texture is not locked!\n");
                success = false;
            }
            //Unlock texture
            else
            {
                Marshal.Copy(pixels, 0, pixelsPtr, pitch * Height * 4);

                SDL.SDL_UnlockTexture(texturePtr);
                pixelsPtr = IntPtr.Zero;
                pixels = null;
                pitch = 0;
            }

            return success;
        }

        public byte[] Pixels => pixels;

        public void CopyPixels(byte[] pixels)
        {
            //Texture is locked
            if (pixelsPtr != IntPtr.Zero)
            {
                //Copy to locked pixels
                Buffer.BlockCopy(pixels, 0, this.pixels, 0, pitch * Height);
            }
        }

        public int Pitch => pitch;

        //Get the pixel requested
        public uint GetPixel32(uint x, uint y)
        {
            var pixels32 = new uint[pitch * Height];
            Buffer.BlockCopy(pixels, 0, pixels32, 0, pitch * Height * 4);

            return pixels32[(y * (pitch / 4)) + x];
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

                if (texturePtr != IntPtr.Zero)
                {
                    SDL.SDL_DestroyTexture(texturePtr);
                    texturePtr = IntPtr.Zero;
                }

                pixelsPtr = IntPtr.Zero;
                pixels = null;
                pitch = 0;

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
