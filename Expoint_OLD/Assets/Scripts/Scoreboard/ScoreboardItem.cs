using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.ComponentModel.Design;

public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    public TMP_Text usernameText;
    public TMP_Text killsText;
    public TMP_Text deathsText;
    public TMP_Text scoreText;
    public TMP_Text teamText;

    Player player;

    public int playerKills;

    public void Initialize(Player player)
    {
        this.player = player;
        if (player.NickName == "domiborn")
            usernameText.text = "<color=#FFD700>domibron";
        else if (player.NickName != string.Empty)
            usernameText.text = player.NickName;
        else
            usernameText.text = "NAME IS BLANK";


        teamText.text = player.CustomProperties["team"].ToString();
        scoreText.text = "0";
        deathsText.text = "0";
        killsText.text = "0";

        UpdateStats();
    }

    void UpdateStats()
    {
        if (player.CustomProperties.TryGetValue("kills", out object kills))
        {
            playerKills = (int)kills;
            killsText.text = kills.ToString();
        }
        if (player.CustomProperties.TryGetValue("deaths", out object deaths))
        {
            deathsText.text = deaths.ToString();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == player)
        {
            if (changedProps.ContainsKey("kills") || changedProps.ContainsKey("deaths"))
            {
                UpdateStats();
            }
        }
    }
}
