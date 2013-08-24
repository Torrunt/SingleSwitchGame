using SFML.Graphics;
using System.Collections.Generic;
using System;

namespace SingleSwitchGame
{
    public class TDrawable : Transformable, Drawable
    {
        public void Draw(RenderTarget target, RenderStates states) { }
    }

    class Graphics
    {
        private static List<Texture> textures = new List<Texture>();
        private static List<string> textureFileNames = new List<string>();

        public static Sprite getSprite(string filename)
        {
            return new Sprite(getTexture(filename));
        }
        public static Texture getTexture(string filename)
        {
            int index = textureFileNames.IndexOf(filename);

            if (index >= 0)
            {
                // Texture Already Exists
                return textures[index];
            }
            else
            {
                // New Texture
                Texture texture = new Texture(filename);
                textures.Add(texture);
                textureFileNames.Add(filename);

                return texture;
            }
        }
    }
}
