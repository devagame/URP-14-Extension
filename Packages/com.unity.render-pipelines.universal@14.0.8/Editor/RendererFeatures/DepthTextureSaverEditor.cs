using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DepthTextureSaver))]
public class DepthTextureSaverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DepthTextureSaver saver = (DepthTextureSaver)target;

        if (GUILayout.Button("Save Depth Texture"))
        {
            saver.SaveDepthTexture();
        }
    }
}