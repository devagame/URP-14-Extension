using System;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
public class SceneBoundsCalculator : MonoBehaviour
{
    /// <summary>
    /// 需要拍摄的场景节点
    /// </summary>
    public GameObject CaptureRoot;
    
    /// <summary>
    /// 拍摄用的正交相机
    /// </summary>
    public Camera heightMapCamera;
    
    
    /// <summary>
    /// 高度图
    /// </summary>
    public RenderTexture heightMap;

    public Vector2 MapSize;
    /// <summary>
    /// 拍摄区域的最小值
    /// </summary>
    public Vector2 mapMinPos;
    /// <summary>
    /// 拍摄区域的最大图
    /// </summary>
    public Vector2 mapMaxPos;

    /// <summary>
    /// 地图的Bounds信息
    /// </summary>
    public Bounds mapBounds;

    public Vector3 worldMin;
    public Vector3 worldMax;
    public Vector3 viewMin;
    public Vector3  viewMax;
    public Vector4 _ViewMinMaxHeightZ;
    
    //计算使用
    public Vector4 _WorldMinMaxHeightZ;
    public Vector4 _MinMaxMapPos;
    void Start()
    {
        CalculateSceneBounds();
    }

    private void Update()
    {
        Shader.SetGlobalVector("_MinMaxHeightZ",_ViewMinMaxHeightZ);
        
        Shader.SetGlobalVector("_WorldMinMaxHeightZ",_WorldMinMaxHeightZ);
        Shader.SetGlobalVector("_MinMaxMapPos",_MinMaxMapPos);
    }

    [Button("设置数据")]
    void CalculateSceneBounds()
    {
        Renderer[] renderers = CaptureRoot.GetComponentsInChildren<Renderer>();
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

        mapBounds = bounds;
        //heightMapMaxZ = mapBounds.max.y;
        //heightMapMinZ = mapBounds.min.y;
        //需要转到相机空间
        
        worldMin = mapBounds.min;
        worldMax = mapBounds.max;
        viewMin = heightMapCamera.WorldToViewportPoint(mapBounds.min);
        viewMax = heightMapCamera.WorldToViewportPoint(mapBounds.max);
        
        _WorldMinMaxHeightZ = new Vector4(worldMin.y, worldMax.y, 0, 0);
        _ViewMinMaxHeightZ = new Vector4(viewMin.z, viewMax.z, 0, 0);
        
        //设置相机位置等
        heightMapCamera.orthographic = true;
        heightMapCamera.transform.rotation = Quaternion.Euler(new Vector3(90,0,0));
        
        if(heightMapCamera != null && heightMapCamera.orthographic)
        {
            float height = 2f * heightMapCamera.orthographicSize;
            float width = height * heightMapCamera.aspect;
            MapSize = new Vector2(width, height);
            Vector3 camera_pos = heightMapCamera.transform.position;

            mapMinPos = new Vector2(camera_pos.x - width / 2, camera_pos.z - height / 2);
            mapMaxPos = new Vector2(camera_pos.x - width / 2, camera_pos.z - height / 2);
            _MinMaxMapPos = new Vector4(mapMinPos.x, mapMinPos.y, mapMaxPos.x, mapMaxPos.y);
            
            Debug.Log("Orthographic Camera bounds: " + width + " x " + height);
        }
        else
        {
            Debug.LogError("The script is not attached to an orthographic camera.");
        }
    }
}