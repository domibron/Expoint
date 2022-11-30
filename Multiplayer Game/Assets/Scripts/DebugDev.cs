using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DebugDev : MonoBehaviour
{
    [HideInInspector]
    public static DebugDev instance;

    [ContextMenuItem("RUN THE RUN", "RUN")]
    public string text;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;

        }
    }

    [ContextMenu("RUN")]
    public void RUN()
    {
        print("RAN");
    }
}
