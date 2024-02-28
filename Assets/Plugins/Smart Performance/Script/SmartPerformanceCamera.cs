using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JiRoEnt.Utils.SmartDynamicResolution
{
    public class SmartPerformanceCamera : MonoBehaviour
    {
        private void Start()
        {
            this.gameObject.AddComponent<SphereCollider>().radius = 0.1f;
            this.gameObject.GetComponent<SphereCollider>().isTrigger = false;
        }
    }
}