using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class LookController : MonoBehaviour, IMouseInput
{

    public GameObject ObjectToRotate;

    private float yRotation;
    //private float xRotation;

    float mouseX;
    float mouseY;

    void Awake()
    {
        //ObjectToRotate = Camera.main.gameObject;
    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; // ! remove
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
        ObjectToRotate.transform.localRotation = Quaternion.Euler(yRotation, 0, 0);
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
