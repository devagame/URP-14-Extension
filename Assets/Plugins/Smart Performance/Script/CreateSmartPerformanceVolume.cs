#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace JiRoEnt.Utils.SmartDynamicResolution
{
    public class CreateSmartPerformanceVolume : MonoBehaviour
    {
        [MenuItem("Tools/JiRo Ent/Smart Performance/New Volume")]
        public static void NewVolume()
        {
            // GameObject _smartPerformanceVolume = Resources.Load<GameObject>("Prefabs/Dynamic Resolution Volume");
            GameObject _smartPerformanceVolume;
            string[] guids = AssetDatabase.FindAssets("Dynamic Resolution Volume");

            if (guids.Length > 0)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                _smartPerformanceVolume = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                GameObject _spawnedVolume = Instantiate(_smartPerformanceVolume,
                                                        SceneView.lastActiveSceneView.camera.transform.position,
                                                        Quaternion.identity);

                SmartPerformance.Instance.AddVolume(_spawnedVolume.GetComponent<SmartPerformanceVolume>());
            }
            else
                Debug.Log("Dynamic Resolution Volume Prefab cannot be found!");
        }
    }
}
#endif