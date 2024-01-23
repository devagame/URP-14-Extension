using UnityEngine;

[ExecuteAlways]
public class SceneBoundsCalculator : MonoBehaviour
{
    public 
    void Start()
    {
        CalculateSceneBounds();
    }

    void CalculateSceneBounds()
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.Log("No renderers found in the scene.");
            return;
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        Debug.Log("Scene Bounds: ");
        Debug.Log("Min: " + min);
        Debug.Log("Max: " + max);
    }
}