using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Settings : MonoBehaviour
{
    [SerializeField] TMP_Dropdown window;
    [SerializeField] TMP_InputField x;
    [SerializeField] TMP_InputField y;
    [SerializeField] TMP_Dropdown targetMonitor;

    private FullScreenMode screenMode;
    private Resolution resolution;

    void Start()
    {
        if (PlayerPrefs.HasKey("UnitySelectMonitor"))
        {
            StartCoroutine(TargetDisplayHack(PlayerPrefs.GetInt("UnitySelectMonitor")));
            Display.displays[PlayerPrefs.GetInt("UnitySelectMonitor")].Activate();
        }
    }

    void Update()
    {
        

        //Screen.SetResolution(int.Parse(x.text), int.Parse(y.text), Screen.fullScreen);
    }

    public void Save()
    {
        MonitorModeSet();
        ResolutionSet();
        SetScreen();
        Debug.Log("saving to file Failed: Implimentation not set");
    }

    void SetScreen()
    {
        Screen.SetResolution(resolution.width, resolution.height, screenMode, 60);
    }

    void ResolutionSet()
    {
        resolution.width = int.Parse(x.text);
        resolution.height = int.Parse(y.text);
    }

    void MonitorModeSet()
    {
        switch (window.value)
        {
            case 0:
                screenMode = FullScreenMode.FullScreenWindow;
                break;

            case 1:
                screenMode = FullScreenMode.ExclusiveFullScreen;
                break;

            case 2:
                screenMode = FullScreenMode.MaximizedWindow;
                break;

            case 3:
                screenMode = FullScreenMode.Windowed;
                break;
        }
    }



    public void SetDisplay()
    {
        print(targetMonitor.value);
        StartCoroutine(TargetDisplayHack(targetMonitor.value));
        Display.displays[1].Activate();
    }

    private IEnumerator TargetDisplayHack(int targetDisplay)
    {
        // Get the current screen resolution.
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        // Set the target display and a low resolution.
        PlayerPrefs.SetInt("UnitySelectMonitor", targetDisplay);
        Screen.SetResolution(800, 600, Screen.fullScreen);

        // Wait a frame.
        yield return null;

        // Restore resolution.
        Screen.SetResolution(screenWidth, screenHeight, Screen.fullScreen);
    }
}
