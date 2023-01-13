using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSettingsManager : MonoBehaviour
{
    public static RoomSettingsManager Instance;

    float durationOfMatch;
    int maxKills;
    string teamName;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
