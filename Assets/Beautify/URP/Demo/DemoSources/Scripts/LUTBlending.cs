using UnityEngine;

namespace Beautify.Universal {

    [ExecuteInEditMode]
    public class LUTBlending : MonoBehaviour {

        public Texture2D LUT1, LUT2;
        [Range(0, 1)]
        public float phase;
        public Shader lerpShader;

        float oldPhase = -1;
        RenderTexture rt;
        Material lerpMat;

        static class ShaderParams {
            public static int LUT2 = Shader.PropertyToID("_LUT2");
            public static int Phase = Shader.PropertyToID("_Phase");
        }

        void OnEnable() {
            UpdateBeautifyLUT();
        }

        void OnValidate() {
            UpdateBeautifyLUT();
        }

        private void OnDestroy() {
            if (rt != null) {
                rt.Release();
            }
        }

        void LateUpdate() {
            UpdateBeautifyLUT();
        }

        void UpdateBeautifyLUT() {
            if (oldPhase == phase || LUT1 == null || LUT2 == null || lerpShader == null) return;
            oldPhase = phase;

            if (rt == null) {
                rt = new RenderTexture(LUT1.width, LUT1.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                rt.filterMode = FilterMode.Point;
            }
            if (lerpMat == null) {
                lerpMat = new Material(lerpShader);
            }
            lerpMat.SetTexture(ShaderParams.LUT2, LUT2);
            lerpMat.SetFloat(ShaderParams.Phase, phase);
            Graphics.Blit(LUT1, rt, lerpMat);
            BeautifySettings.settings.lut.Override(true);
            BeautifySettings.settings.lutIntensity.Override(1f);
            BeautifySettings.settings.lutTexture.Override(rt);
        }
    }
}
