using SFML.Graphics;
using System.Collections.Generic;
using System;

namespace SingleSwitchGame
{
    class Graphics
    {
        public static string ASSETS_SPRITES = "assets/sprites/";

        private static List<Texture> Textures = new List<Texture>();
        private static List<string> TextureFileNames = new List<string>();

        private static List<AnimatedSpriteData> AnimatedSpriteDatas = new List<AnimatedSpriteData>();
        private static List<string> AnimatedSpriteDataFileNames = new List<string>();

        public static Sprite GetSprite(string filename)
        {
            Sprite sprite = new Sprite(GetTexture(filename));
            sprite.Texture.Smooth = true;

            return sprite;
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

        /// <param name="xml">The XML file for the Animated Sprite.</param>
        public static AnimatedSprite GetAnimatedSprite(Game game, string xml)
        {
            AnimatedSprite sprite = new AnimatedSprite(game, GetAnimatedSpriteData(xml));

            return sprite;
        }
        /// <param name="xml">The XML file for the Animated Sprite.</param>
        public static AnimatedSpriteData GetAnimatedSpriteData(string xml)
        {
            int index = AnimatedSpriteDataFileNames.IndexOf(xml);

            if (index >= 0)
            {
                // Texture Already Exists
                return AnimatedSpriteDatas[index];
            }
            else
            {
                // New Texture
                AnimatedSpriteData spriteData = new AnimatedSpriteData(xml);
                AnimatedSpriteDatas.Add(spriteData);
                AnimatedSpriteDataFileNames.Add(xml);

                return spriteData;
            }
        }

    }
}
