using UnityEngine;
using UnityEditor;

namespace AEG.FSR
{
    [CustomEditor(typeof(FSR3_BASE), editorForChildClasses: true)]
    public class FSR3_Editor : Editor
    {
        public override void OnInspectorGUI() {
            FSR3_BASE fsrScript = target as FSR3_BASE;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("FSR Settings", EditorStyles.boldLabel);
            FSR_Quality fsrQuality = (FSR_Quality)EditorGUILayout.EnumPopup(Styles.qualityText, fsrScript.FSRQuality);
            float AntiGhosting = EditorGUILayout.Slider(Styles.antiGhosting, fsrScript.antiGhosting, 0.0f, 1.0f);

            bool sharpening = EditorGUILayout.Toggle(Styles.sharpeningText, fsrScript.sharpening);
            float sharpness = fsrScript.sharpness;
            if(fsrScript.sharpening) {
                EditorGUI.indentLevel++;
                sharpness = EditorGUILayout.Slider(Styles.sharpnessText, fsrScript.sharpness, 0.0f, 1.0f);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Transparency Settings", EditorStyles.boldLabel);
            bool generateReactiveMask = EditorGUILayout.Toggle(Styles.reactiveMaskText, fsrScript.generateReactiveMask);

            float autoReactiveScale = fsrScript.autoReactiveScale;
            float autoReactiveThreshold = fsrScript.autoReactiveThreshold;
            float autoReactiveBinaryValue = fsrScript.autoReactiveBinaryValue;

            bool generateTCMask = false;

            float autoTcThreshold = fsrScript.autoTcThreshold;
            float autoTcScale = fsrScript.autoTcScale;
            float autoTcReactiveScale = fsrScript.autoTcReactiveScale;
            float autoTcReactiveMax = fsrScript.autoTcReactiveMax;

            if(fsrScript.generateReactiveMask) {
                EditorGUI.indentLevel++;
                autoReactiveScale = EditorGUILayout.Slider(Styles.reactiveScaleText, fsrScript.autoReactiveScale, 0.0f, 1.0f);
                autoReactiveThreshold = EditorGUILayout.Slider(Styles.reactiveThresholdText, fsrScript.autoReactiveThreshold, 0.0f, 1.0f);
                autoReactiveBinaryValue = EditorGUILayout.Slider(Styles.reactiveBinaryValueText, fsrScript.autoReactiveBinaryValue, 0.0f, 1.0f);
                EditorGUI.indentLevel--;
            }


            EditorGUILayout.Space();

#if UNITY_BIRP
            EditorGUILayout.LabelField("MipMap Settings", EditorStyles.boldLabel);
            bool autoTextureUpdate = EditorGUILayout.Toggle(Styles.autoTextureUpdateText, fsrScript.autoTextureUpdate);
            float mipMapUpdateFrequency = fsrScript.mipMapUpdateFrequency;
            if(fsrScript.autoTextureUpdate) {
                EditorGUI.indentLevel++;
                mipMapUpdateFrequency = EditorGUILayout.FloatField(Styles.autoUpdateFrequencyText, fsrScript.mipMapUpdateFrequency);
                EditorGUI.indentLevel--;
            }
            float mipmapBiasOverride = EditorGUILayout.Slider(Styles.mipmapBiasText, fsrScript.mipmapBiasOverride, 0.0f, 1.0f);
#endif
            if(EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(fsrScript);

                Undo.RecordObject(target, "Changed Area Of Effect");
                fsrScript.FSRQuality = fsrQuality;
                fsrScript.antiGhosting = AntiGhosting;
                fsrScript.sharpening = sharpening;
                fsrScript.sharpness = sharpness;

                fsrScript.generateReactiveMask = generateReactiveMask;
                fsrScript.autoReactiveThreshold = autoReactiveThreshold;
                fsrScript.autoReactiveScale = autoReactiveScale;
                fsrScript.autoReactiveBinaryValue = autoReactiveBinaryValue;

                fsrScript.generateTCMask = generateTCMask;
                fsrScript.autoTcThreshold = autoTcThreshold;
                fsrScript.autoTcScale = autoTcScale;
                fsrScript.autoTcReactiveScale = autoTcReactiveScale;
                fsrScript.autoTcReactiveMax = autoTcReactiveMax;

#if UNITY_BIRP
                fsrScript.autoTextureUpdate = autoTextureUpdate;
                fsrScript.mipMapUpdateFrequency = mipMapUpdateFrequency;
                fsrScript.mipmapBiasOverride = mipmapBiasOverride;
#endif
            }
        }

        private static class Styles
        {
            public static GUIContent qualityText = new GUIContent("Quality", "Quality 1.5, Balanced 1.7, Performance 2, Ultra Performance 3");
            public static GUIContent antiGhosting = new GUIContent("Anti Ghosting", "The Anti Ghosting value between 0 and 1, where 0 is no Anti Ghosting and 1 is the maximum amount.");
            public static GUIContent sharpeningText = new GUIContent("Sharpening", "Enable an additional (RCAS) sharpening pass in the fsr algorithm.");
            public static GUIContent sharpnessText = new GUIContent("Sharpness", "The sharpness value between 0 and 1, where 0 is no additional sharpness and 1 is maximum additional sharpness.");
            public static GUIContent hdrText = new GUIContent("HDR", "Instructs FSR to use HDR in the algorithm, for better quality.");
            public static GUIContent reactiveMaskText = new GUIContent("Reactive Mask", "");
            public static GUIContent reactiveThresholdText = new GUIContent("Reactive Threshold", "Setting this value too small will cause visual instability. Larger values can cause ghosting. Recommended default value is 0.9f.");
            public static GUIContent reactiveScaleText = new GUIContent("Reactive Scale", "Larger values result in more reactive pixels. Recommended default value is 0.2f");
            public static GUIContent reactiveBinaryValueText = new GUIContent("Reactive Binary Value", "Recommended default value is 0.5f.");


            public static GUIContent tcMaskText = new GUIContent("Transparency and Composition Mask", "");

            public static GUIContent autoTcThresholdText = new GUIContent("T&C Threshold", "Setting this value too small will cause visual instability. Larger values can cause ghosting. Recommended default value is 0.05f.");
            public static GUIContent autotcScaleText = new GUIContent("T&C Scale", "Smaller values will increase stability at hard edges of translucent objects. Recommended default value is 1.0f.");
            public static GUIContent autoTcReactiveScaleText = new GUIContent("T&C Reactive Scale", "Maximum value reactivity can reach. Recommended default value is 5.0f.");
            public static GUIContent autoTcReactiveMaxText = new GUIContent("T&C Max", "Maximum value reactivity can reach. Recommended default value is 0.9f.");

            public static GUIContent mipmapBiasText = new GUIContent("Mipmap Bias Override", "An extra mipmap bias override for if AMD's recommended MipMap Bias values give artifacts");
            public static GUIContent autoTextureUpdateText = new GUIContent("Auto Texture Update", "Wether the mipmap biases of all textures in the scene should automatically be updated");
            public static GUIContent autoUpdateFrequencyText = new GUIContent("Update Frequency", "Interval in seconds in which the mipmap biases should be updated");
            public static GUIContent debugText = new GUIContent("Debug", "Enables debugging in the FSR algorithm, which can help catch certain errors.");
        }
    }
}
