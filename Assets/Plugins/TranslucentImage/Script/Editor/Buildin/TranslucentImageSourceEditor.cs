﻿using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LeTai.Asset.TranslucentImage.Editor
{
[CustomEditor(typeof(TranslucentImageSource))]
[CanEditMultipleObjects]
public class TranslucentImageSourceEditor : UnityEditor.Editor
{
    UnityEditor.Editor configEditor;

    EditorProperty blurConfig;
    EditorProperty downsample;
    EditorProperty blurRegion;
    EditorProperty maxUpdateRate;
    EditorProperty backgroundFill;
    EditorProperty preview;

    ScalableBlurConfigEditor ConfigEditor
    {
        get
        {
            if (configEditor == null)
            {
                var config = ((TranslucentImageSource)target).BlurConfig;
                if (config != null)
                    configEditor = CreateEditor(config);
            }

            return (ScalableBlurConfigEditor)configEditor;
        }
    }

    void OnEnable()
    {
        blurConfig     = new EditorProperty(serializedObject, nameof(TranslucentImageSource.BlurConfig));
        downsample     = new EditorProperty(serializedObject, nameof(TranslucentImageSource.Downsample));
        blurRegion     = new EditorProperty(serializedObject, nameof(TranslucentImageSource.BlurRegion));
        maxUpdateRate  = new EditorProperty(serializedObject, nameof(TranslucentImageSource.MaxUpdateRate));
        backgroundFill = new EditorProperty(serializedObject, nameof(TranslucentImageSource.BackgroundFill));
        preview        = new EditorProperty(serializedObject, nameof(TranslucentImageSource.Preview));
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(target, "Change Translucent Image Source property");
        PrefabUtility.RecordPrefabInstancePropertyModifications(target);

        using (var v = new EditorGUILayout.VerticalScope())
        {
            EditorGUILayout.Space();
            GUI.Box(v.rect, GUIContent.none);
            blurConfig.Draw();

            var curConfig = (ScalableBlurConfig)blurConfig.serializedProperty.objectReferenceValue;
            if (curConfig == null)
            {
                EditorGUILayout.HelpBox("Missing Blur Config", MessageType.Warning);
                if (GUILayout.Button("New Blur Config File"))
                {
                    ScalableBlurConfig newConfig = CreateInstance<ScalableBlurConfig>();

                    var path = AssetDatabase.GenerateUniqueAssetPath(
                        $"Assets/{SceneManager.GetActiveScene().name} {serializedObject.targetObject.name} Blur Config.asset");
                    AssetDatabase.CreateAsset(newConfig, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorGUIUtility.PingObject(newConfig);
                    blurConfig.serializedProperty.objectReferenceValue = newConfig;
                }
            }
            else
            {
                // EditorGUILayout.LabelField("Blur settings", EditorStyles.centeredGreyMiniLabel);
                ConfigEditor.Draw(curConfig);
            }
        }

        EditorGUILayout.Space();

        downsample.Draw();
        blurRegion.Draw();
        maxUpdateRate.Draw();
        backgroundFill.Draw();
        preview.Draw();

        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}
}
