using System.Collections.Generic;
using UnityEngine;

namespace LeTai.Asset.TranslucentImage
{
    public partial class TranslucentImageSource
    {
        private static List<SpriteRendererBlur> spriteRenderer = new List<SpriteRendererBlur>();

        public static void RegisterSpriteRenderer(SpriteRendererBlur sprite)
        {
            if(sprite == null)
                return;
            if (!spriteRenderer.Contains(sprite))
            {
                spriteRenderer.Add(sprite);
            }
        }

        public static void UnRegisterSpriteRenderer(SpriteRendererBlur sprite)
        {
            if(sprite == null)
                return;
            if (spriteRenderer.Contains(sprite))
            {
                spriteRenderer.Remove(sprite);
            }
        }
    }
}