using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightThingf : MonoBehaviour
{
    public GameObject lightObject;
    private Light lightLight;

    public float startAngle = 0;
    public float endAngle = 180;

    public float speed = 2f;

    public float delay = 1f;

    private float time;
    private float holdingTime = 0;

    private bool DontRun = true;

    // Start is called before the first frame update
    void Start()
    {
        lightObject = this.gameObject;
        lightLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit, 200f);
        lightLight.range = hit.distance + 20f;

        if (time >= holdingTime && DontRun)
        {
            lightObject.transform.localRotation = Quaternion.Euler(startAngle, 0, 0);

            DontRun = false;

            holdingTime += delay;
        }

        if (lightObject.transform.localRotation.x < 0.98f && !DontRun)
        {
            //lightObject.transform.Rotate(Time.deltaTime * speed, 0f, 0f);

            lightLight.enabled = true;
            lightObject.transform.Rotate(Time.deltaTime * speed, 0f, 0f);



        }
        else
        {
            lightLight.enabled = false;
            DontRun = true;
        }
    }
}
