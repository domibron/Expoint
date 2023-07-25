using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using UnityEngine.SceneManagement;

public class DiscordHanderler : MonoBehaviour
{
    public static DiscordHanderler Instance;

    public Discord.Discord discord;

    int retries = 0;
    bool disabled = true;

    public string details = "None set";
    public string state = "None set";

    void Awake()
    {
        if (disabled)
            return;

        Instance = this;
        if (Instance != this)
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (disabled)
            return;

        DontDestroyOnLoad(gameObject);
        discord = new Discord.Discord(1023313648053133513, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);

        var acrivityManager = discord.GetActivityManager();
        acrivityManager.ClearActivity((result) =>
        {
            if (result == Discord.Result.Ok)
            {
                print("yes");
            }
            else
            {
                print("no");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (disabled)
            return;

        if (discord == null)
        {
            if (retries < 3)
                discord = new Discord.Discord(1023313648053133513, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
            return;
        }

        var acrivityManager = discord.GetActivityManager();
        acrivityManager.ClearActivity((result) =>
        {
            if (result == Discord.Result.Ok)
            {
                print("yes");
            }
            else
            {
                print("no");
            }
        });

        discord.RunCallbacks();

        // var activity = new Discord.Activity
        // {
        //     Details = "In Room - Waiting to start",
        //     State = $"room name: {PhotonNetwork.CurrentRoom.Name} \n max players: {PhotonNetwork.CurrentRoom.MaxPlayers} \n players in room: {PhotonNetwork.CurrentRoom.PlayerCount}",
        // };
        // acrivityManager.UpdateActivity(activity, (res) =>
        // {
        //     if (res == Discord.Result.Ok)
        //         Debug.Log("Discord Set!");
        //     else
        //         Debug.LogError("setting status failed!");
        // });


    }

    void OnApplicationQuit()
    {
        if (disabled)
            return;

        var acrivityManager = discord.GetActivityManager();
        acrivityManager.ClearActivity((result) =>
        {
            if (result == Discord.Result.Ok)
            {
                print("yes");
            }
            else
            {
                print("no");
            }
        });
    }
}
