// Copyright (c) 2023 Nico de Poel
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;

namespace FidelityFX
{
    /// <summary>
    /// A collection of callbacks required by the FSR3 process.
    /// This allows some customization by the game dev on how to integrate FSR3 into their own game setup.
    /// </summary>
    public interface IFsr3Callbacks
    {
        Shader LoadShader(string name);
        void UnloadShader(Shader shader);
        ComputeShader LoadComputeShader(string name);
        void UnloadComputeShader(ComputeShader shader);

        /// <summary>
        /// Apply a mipmap bias to in-game textures to prevent them from becoming blurry as the internal rendering resolution lowers.
        /// This will need to be customized on a per-game basis, as there is no clear universal way to determine what are "in-game" textures.
        /// The default implementation will simply apply a mipmap bias to all 2D textures, which will include things like UI textures and which might miss things like terrain texture arrays.
        /// 
        /// Depending on how your game organizes its assets, you will want to create a filter that more specifically selects the textures that need to have this mipmap bias applied.
        /// You may also want to store the bias offset value and apply it to any assets that are loaded in on demand.
        /// </summary>
        void OnMipMapAllTextures(float biasOffset);

        void OnResetAllMipMaps();
    }

    /// <summary>
    /// Default implementation of IFsr3Callbacks using simple Resources calls.
    /// These are fine for testing but a proper game will want to extend and override these methods.
    /// </summary>
    public class Fsr3CallbacksBase : IFsr3Callbacks
    {
        protected float CurrentBiasOffset = 0;
        protected Texture[] m_allTextures;

        public virtual Shader LoadShader(string name) {
            return Resources.Load<Shader>(name);
        }

        public virtual void UnloadShader(Shader shader) {
            Resources.UnloadAsset(shader);
        }

        public virtual ComputeShader LoadComputeShader(string name) {
            return Resources.Load<ComputeShader>(name);
        }

        public virtual void UnloadComputeShader(ComputeShader shader) {
            Resources.UnloadAsset(shader);
        }

        public void OnMipMapAllTextures(float _mipMapBias) {
            m_allTextures = Resources.FindObjectsOfTypeAll(typeof(Texture)) as Texture[];
            for(int i = 0; i < m_allTextures.Length; i++) {
                if(m_allTextures[i].mipmapCount <= 1)
                    continue;
                m_allTextures[i].mipMapBias = _mipMapBias;
            }
        }

        public void OnResetAllMipMaps() {
            m_allTextures = Resources.FindObjectsOfTypeAll(typeof(Texture)) as Texture[];
            for(int i = 0; i < m_allTextures.Length; i++) {
                m_allTextures[i].mipMapBias = 0;
            }
            m_allTextures = null;
        }
    }
}
