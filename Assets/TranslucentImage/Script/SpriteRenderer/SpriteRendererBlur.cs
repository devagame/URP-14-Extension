using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace LeTai.Asset.TranslucentImage
{
    /// <summary>
    /// Dynamic blur-behind UI element
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [ExecuteAlways]
    public class SpriteRendererBlur : MonoBehaviour
    {
        private Material spriteDefult;
        private Material spriteBackground;
        
        private void Start()
        {
            Debug.Log("SpriteRendererBlur Start Add");
            TranslucentImageSource.RegisterSpriteRenderer(this);
        }

        private void OnDestroy()
        {
            TranslucentImageSource.UnRegisterSpriteRenderer(this);
        }

        public void SwtichDefualt()
        {
            if (spriteDefult == null)
            {
                spriteDefult = new Material(Shader.Find("Sprites/Default"));
            }

            transform.GetComponent<SpriteRenderer>().sharedMaterial = spriteDefult;
        }
        
        public void SwtichBackground()
        {
            if (spriteBackground == null)
            {
                spriteBackground = new Material(Shader.Find("Sprites/Background-Default"));
            }

            transform.GetComponent<SpriteRenderer>().sharedMaterial = spriteBackground;
        }
    }
}