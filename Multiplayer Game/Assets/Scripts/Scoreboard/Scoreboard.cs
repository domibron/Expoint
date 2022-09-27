using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardItemPrefab;
    [SerializeField] CanvasGroup canvasGroup;

    List<Player> players = new List<Player>();

    Dictionary<Player, ScoreboardItem> scoreboardItems = new Dictionary<Player, ScoreboardItem>();

    private void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
            players.Add(player);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
        players.Add(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
        players.Remove(otherPlayer);
    }

    void AddScoreboardItem(Player player)
    {
        ScoreboardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreboardItem>();
        item.Initialize(player);
        scoreboardItems[player] = item;
    }

    void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems[player].gameObject);
        scoreboardItems.Remove(player);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            canvasGroup.alpha = 1;
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            canvasGroup.alpha = 0;
        }
    }

    // quite usefull

    public Player GetPlayerWithMostKills()
    {

        int highestKills = 0;
        Player playerWithMostKills = players[0]; // might hurt might not

        foreach (Player _player in players)
        {
            ScoreboardItem _item = scoreboardItems[_player];

            string killsString = _item.killsText.text;
            int kills = int.Parse(killsString);

            if (kills <= highestKills) // will bite me in the ass need a tie exeption.s
            {
                playerWithMostKills = _player;
                highestKills = kills;
            }
        }

        return playerWithMostKills;
        //print(playerWithMostKills.NickName + " with " + highestKills);
    }
}
