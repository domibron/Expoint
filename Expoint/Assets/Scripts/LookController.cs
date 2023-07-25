using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class LookController : MonoBehaviour, IMouseInput
{

    GameObject cam;

    private float yRotation;
    //private float xRotation;

    float mouseX;
    float mouseY;

    void Awake()
    {
        cam = Camera.main.gameObject;
    }

    void Update()
    {
        LookContols();
    }

    void LookContols()
    {
        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);


        transform.Rotate(Vector3.up * mouseX);
        cam.transform.localRotation = Quaternion.Euler(yRotation, 0, 0);
    }



    void IMouseInput.mouseX(float x)
    {
        mouseX = x;
    }

    void IMouseInput.mouseY(float y)
    {
        mouseY = y;
    }
}
