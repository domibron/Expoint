using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    bool active;

    // extarnlly set vars

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        // make a swich statement.


        // make into fuction to  instanciate. please make a list of scenes this is just bad.
        if (scene.buildIndex == 2) // game scene
        {
            InstaciatePlayerManager();
        }
        else if (scene.buildIndex == 3) // game scene
        {
            InstaciatePlayerManager();
        }
        else if (scene.buildIndex == 4) // game scene
        {
            InstaciatePlayerManager();
        }
    }

    void InstaciatePlayerManager()
    {
        GameObject playerRe = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity); // create function
        PlayerManager playerManager = playerRe.GetComponent<PlayerManager>();
        //playerManager.gaw = 69; // set adjofnwrasoufanw
    }
}
