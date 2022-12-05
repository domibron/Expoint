using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailMove : MonoBehaviour
{
    public Vector3 goToPos;
    public Vector3 startPos;
    public float Speed;

    float time;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * Speed;
        transform.position = Vector3.Lerp(startPos, goToPos, time);
    }
}
