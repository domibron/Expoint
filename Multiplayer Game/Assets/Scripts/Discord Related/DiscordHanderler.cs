using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class DiscordHanderler : MonoBehaviourPunCallbacks
{
    public static DiscordHanderler Instance;

    public Discord.Discord discord;

    int retries = 0;
    bool disabled = true;

    //public string details = "None set";
    //public string state = "None set";

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


        if (SceneManager.GetActiveScene().buildIndex == 0 && PhotonNetwork.InRoom)
        {
            var activity = new Discord.Activity
            {
                Details = "In Room - Waiting to start",
                State = $"room name: {PhotonNetwork.CurrentRoom.Name} \n max players: {PhotonNetwork.CurrentRoom.MaxPlayers} \n players in room: {PhotonNetwork.CurrentRoom.PlayerCount}",
            };
            acrivityManager.UpdateActivity(activity, (res) =>
            {
                if (res == Discord.Result.Ok)
                    Debug.Log("Discord Set!");
                else
                    Debug.LogError("setting status failed!");
            });
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            var activity = new Discord.Activity
            {
                Details = "In Match",
                State = $"room name: {PhotonNetwork.CurrentRoom.Name} | max players: {PhotonNetwork.CurrentRoom.MaxPlayers}  players in match: {PhotonNetwork.CurrentRoom.PlayerCount}",
            };
            acrivityManager.UpdateActivity(activity, (res) =>
            {
                if (res == Discord.Result.Ok)
                    Debug.Log("Discord Set!");
                else
                    Debug.LogError("setting status failed!");
            });
        }
        else
        {
            var activity = new Discord.Activity
            {
                Details = "On the main menu",
                State = "chilling on main menu"
            };
            acrivityManager.UpdateActivity(activity, (res) =>
            {
                if (res == Discord.Result.Ok)
                    Debug.Log("Discord Set!");
                else
                    Debug.LogError("setting status failed!");
            });
        }
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
