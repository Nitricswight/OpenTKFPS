using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace OpenTKFPS.engine.graphics
{
    public static class TextureLoader
    {
        private static List<int> textureIDs = new List<int>();

        public static void CleanUp(){
            GL.DeleteTextures(textureIDs.Count, textureIDs.ToArray());
        }

        public static int loadEmptyTexture(int width, int height, TextureMinFilter minFilter = TextureMinFilter.Nearest, TextureMagFilter magFilter = TextureMagFilter.Nearest, TextureWrapMode wrapMode = TextureWrapMode.Repeat, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Rgba){
            int texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat,width, height, 0, format, PixelType.UnsignedByte, (nint)null);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)wrapMode);

            textureIDs.Add(texID);

            return texID;
        }

        public static int loadTexture(string path, bool flip_y = true, TextureMinFilter minFilter = TextureMinFilter.Nearest, TextureMagFilter magFilter = TextureMagFilter.Nearest, TextureWrapMode wrapMode = TextureWrapMode.Repeat){
            int texID = GL.GenTexture();

            StbImage.stbi_set_flip_vertically_on_load(flip_y? 0 : 1);
            ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);

            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)wrapMode);

            textureIDs.Add(texID);

            return texID;
        }
    }
}