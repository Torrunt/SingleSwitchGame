using SFML.Graphics;
using System.Collections.Generic;
using System;

namespace SingleSwitchGame
{
    public class TDrawable : Transformable, Drawable
    {
        public void Draw(RenderTarget Target, RenderStates states) { }
    }

    class Graphics
    {
        private static List<Texture> Textures = new List<Texture>();
        private static List<string> TextureFileNames = new List<string>();

        public static Sprite GetSprite(string filename)
        {
            return new Sprite(GetTexture(filename));
        }
        public static Texture GetTexture(string filename)
        {
            int index = TextureFileNames.IndexOf(filename);

            if (index >= 0)
            {
                // Texture Already Exists
                return Textures[index];
            }
            else
            {
                // New Texture
                Texture texture = new Texture(filename);
                Textures.Add(texture);
                TextureFileNames.Add(filename);

                return texture;
            }
        }
    }
}
