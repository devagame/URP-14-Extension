#if  UNITY_EDITOR_
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ResetSubGameObject 
{
    [MenuItem("Tools/ResetSelectObjPos")]
    public static void ResetSelectObjPos()
    {
        var objs = Selection.gameObjects;
        foreach (var obj in objs)
        {
            CheckSubGameObject(obj);
        }
    }
    
    public static void CheckSubGameObject(GameObject root)
    {
        var subObjs = root.GetComponentsInChildren<Transform>();
        bool isSubPosAllSame = true;
        
        Vector3 pos = Vector3.zero;
        Vector3 roate = Vector3.zero;
        Vector3 scale = Vector3.one;

        int compareIndex = 0;
        Vector3 comparePos = Vector3.zero;
        foreach (var subObj in subObjs)
        {
            if (subObj == root.transform)
            {
                continue;
            }

            if (compareIndex++ == 0)
            {
                comparePos = subObj.position;   
            }
            
            if (!Mathf.Approximately(subObj.position.x,comparePos.x) ||
                !Mathf.Approximately(subObj.position.y,comparePos.y) ||
                !Mathf.Approximately(subObj.position.z,comparePos.z))
            {
                isSubPosAllSame = false;
                break;
            }
        }

        if (isSubPosAllSame)
        {
            foreach (var subObj in subObjs)
            {
                if (subObj == root.transform)
                {
                    continue;
                }
                
                pos = subObj.position;
                roate = subObj.eulerAngles;
                scale = subObj.localScale;
                
                subObj.localPosition = Vector3.zero;
                subObj.localEulerAngles = Vector3.zero;
                subObj.localScale = Vector3.one;
            }

            Vector3 rScale = root.transform.localScale;
            root.transform.position = pos;
            root.transform.eulerAngles = roate;
            root.transform.localScale = new Vector3(
                rScale.x * scale.x,
                rScale.y * scale.y,
                rScale.z * scale.z);
        }
    }
}
#endif