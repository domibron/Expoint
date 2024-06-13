using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{
    private Camera cam;
    private Movement movementController;
    //public PauseMenuScript pauseMenuScript;
    private Transform headPosition;
    private Transform orientation;
    private Transform cameraPosition;

    [Header("Mouse Sensitivity")]
    public float sensitivityMouse = 1f; //DO NOT HARD SET THIS VALUE - USE A SCRIPT TO MANAGE SETTINGS - SOON BROTHER, SOON!
    public float multiplier = 0.01f;

    [Header("Head Bob")]
    public float headBobHorizontalAmplitude;
    public float headBobVerticalAmplitude;
    [Range(0, 1)] public float headBobSmoothing;

    private float mouseX;
    private float mouseY;
    private float xRotation;
    private float yRotation;
    private float headBobFrequency;
    private float walkingTime;
    private Vector3 targetCameraPosition;

    private void Start()
    {
        movementController = GetComponent<Movement>();
        cam = transform.Find("Camera Holder").GetComponentInChildren<Camera>();
        headPosition = transform.Find("Camera Holder").Find("Head").GetComponent<Transform>();
        orientation = transform.Find("Orientation").GetComponent<Transform>();
        cameraPosition = transform.Find("Camera Position").GetComponent<Transform>();

        Cursor.visible = false; // this hide the cursor
    }

    private void Update()
    {
        //if (pauseMenuScript.isPaused) return;

        mouseX = Input.GetAxisRaw("Mouse X"); // this gets the X (left and right) mouse input
        mouseY = Input.GetAxisRaw("Mouse Y"); // this gets the Y (up and down) mouse input

        yRotation += mouseX * sensitivityMouse * multiplier; // this set the sensitvity and other values to the total mouse rotation
        xRotation -= mouseY * sensitivityMouse * multiplier; // this set the sensitvity and other values to the total mouse rotation

        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // this clamps the up down so you cannot flip the camera


        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0); // this rotates the camera seperatly (up and down)
        cam.transform.position = cameraPosition.position;
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0); // this rotates the orientation (left and right)

        headPosition.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0); // TEMP

        //MainHeadBobbing();
    }

    private void MainHeadBobbing()
    {
        if (movementController.IsMoving && movementController.IsGrounded) walkingTime += Time.deltaTime;
        else walkingTime = 0f;

        headBobFrequency = (movementController.CurrentMovementSpeed > 4.5f) ? 1f * movementController.CurrentMovementSpeed : 4.5f;

        targetCameraPosition = headPosition.position + CalculateHeadBobOffset(walkingTime);

        cameraPosition.position = Vector3.Lerp(cameraPosition.transform.position, targetCameraPosition, headBobSmoothing);

        if ((cameraPosition.position - targetCameraPosition).magnitude <= 0.001) cameraPosition.position = targetCameraPosition;
    }

    private Vector3 CalculateHeadBobOffset(float t)
    {
        float horOffset = 0f;
        float vertOffset = 0f;
        Vector3 Offset = Vector3.zero;

        if (t > 0)
        {
            horOffset = Mathf.Cos(t * headBobFrequency) * headBobHorizontalAmplitude;
            vertOffset = Mathf.Sin(t * headBobFrequency * 2f) * headBobVerticalAmplitude;

            Offset = orientation.right * horOffset + orientation.up * vertOffset;
        }

        return Offset;
    }
}
