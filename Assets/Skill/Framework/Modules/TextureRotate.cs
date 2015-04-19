using UnityEngine;
using System.Collections;
using System.Linq;
namespace Skill.Framework.Modules
{
    public static class TextureRotate
    {
        public static Texture2D Rotate90(Texture2D texture)
        {
            Texture2D virtualPhoto = new Texture2D(texture.height, texture.width, TextureFormat.RGB24, false);

            Color[] origPixels = texture.GetPixels();
            Color[] rotPixels = new Color[origPixels.Length];

            for (var x = 0; x < texture.width; x++)
            {
                for (var y = 0; y < texture.height; y++)
                {
                    rotPixels[x * texture.height + y] = origPixels[(y * texture.width) + (texture.width - 1 - x)];
                }
            }

            virtualPhoto.SetPixels(rotPixels);
            virtualPhoto.Apply();
            return virtualPhoto;
        }

        public static Color[] Rotate90(Color[] origPixels, int width, int height)
        {
            Color[] rotPixels = new Color[origPixels.Length];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    rotPixels[x * height + y] = origPixels[(y * width) + (width - 1 - x)];
                }
            }
            return rotPixels;
        }
    }
}