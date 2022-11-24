using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text text;
    [SerializeField] Canvas canvas;

    [SerializeField] GameObject container;
    [SerializeField] TMP_Text TopMidInfo;

    [SerializeField] GameObject winningPanel;
    [SerializeField] TMP_Text winnerText;

    public float matchDuration;
    public int maxKills;

    public float matchTime;

    float minutes;
    float seconds;
    string textHolder;

    bool isGameOver;
    bool ranPrimary = false;

    PhotonView PV;
    Room currentRoom;
    Player masterPlayer;

    GameObject controller;

    int kills;
    int deaths;

    float timeLeft;
    float respawnTime = 3f;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    IEnumerator Start()
    {
        panel.SetActive(false);
        currentRoom = PhotonNetwork.CurrentRoom;

        foreach (Player _player in PhotonNetwork.PlayerList)
        {
            if (_player.IsMasterClient)
            {
                masterPlayer = _player;
            }
        }

        if (PV.IsMine)
        {
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    matchTime = durationOfMatch * 60;
            //    Hashtable hash = new Hashtable();
            //    hash.Add("MasterTime", matchTime);
            //    hash.Add("MasterKills", maxKills);
            //    PhotonNetwork.NetworkingClient.OpSetCustomPropertiesOfRoom(hash);
            //}

            CreateController();
            SetVaribles();

            yield return new WaitForSeconds(0.5f);

            // after the varibles are set it will set match duration - host will manage the time.
            // this is to pervent d-sync and anyone trying to trigger a endgame.
            matchTime = matchDuration; // converts minuets to seconds.

            while (!PhotonNetwork.IsConnectedAndReady)
            {
                yield return new WaitForSeconds(0.1f);
            }
            SetVaribles();
            matchTime = matchDuration;
        }
        else
        {
            Destroy(container);
            //Destroy(panel);
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (!ranPrimary)
            SetVaribles();
        else if (!PhotonNetwork.IsMasterClient) // add master client check
        {
            matchTime = (float)PhotonNetwork.CurrentRoom.CustomProperties["MasterCT"];

            minutes = Mathf.Floor(matchTime / 60);
            seconds = matchTime % 60;
            textHolder = $"{minutes}:{Mathf.RoundToInt(seconds)}"; // time left display - currently for mins and secs.
        }
    }

    void SetVaribles()
    {
        ranPrimary = true;

        matchDuration = (float)PhotonNetwork.CurrentRoom.CustomProperties["MasterTime"] * 60;
        maxKills = (int)PhotonNetwork.CurrentRoom.CustomProperties["MasterKills"];
        matchTime = (float)PhotonNetwork.CurrentRoom.CustomProperties["MasterCT"]; // DO NOT REMOVE THIS - the time does not set to matchDuration.

        matchTime = matchDuration;

        //print(matchTime + " match time");
        //print(maxKills);
        //print(matchDuration);
    }

    void Update()
    {

        if (PV.IsMine)
        {

            if (!isGameOver && PhotonNetwork.IsMasterClient) // change the time - yes i need to point this out.
            {
                matchTime -= Time.deltaTime;
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "MasterCT", matchTime } }); // set the global value

                minutes = Mathf.Floor(matchTime / 60);
                seconds = matchTime % 60;
                textHolder = $"{minutes}:{Mathf.RoundToInt(seconds)}";
            }
            else if (!isGameOver)
            {
                matchTime = (float)PhotonNetwork.CurrentRoom.CustomProperties["MasterCT"]; // syncing off hosts.
            }


            //float timeHolder = matchTime / 60f; // match time is getting set faster and before this update is called so temp is here to help with that.

            //string textHolder = $"{minutes}:{Mathf.RoundToInt(seconds)}"; // time left display - currently for mins and secs.

            TopMidInfo.text = $"Max kills: {maxKills} | time remaining: <mspace=30>{textHolder}";

            // ==== end match logic ====
            if (PhotonNetwork.IsMasterClient && matchTime <= 0)
            {
                // GameOver send RPC event
                //Scoreboard.Instance.GetPlayerKills(Scoreboard.Instance.GetPlayerWithMostKills());

                PV.RPC(nameof(RPC_SendWinner), RpcTarget.All, Scoreboard.Instance.GetPlayerWithMostKills(), Scoreboard.Instance.GetMostKillsInGame());

                // handle kill count
            }

            if (kills >= maxKills)
            {
                // game over
                // this player wins
                PV.RPC(nameof(RPC_SendWinner), RpcTarget.All, PV.Owner, kills);
            }
        }

        if (panel.activeSelf)
        {
            timeLeft -= Time.deltaTime;
            text.text = $"<b>You Died lol!</b><br>Respawning in:<br><mspace=0.75em>{(Mathf.Round(timeLeft * 100f) / 100f).ToString("N2")}</mspace>";
        }
    }

    void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerPrefab"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });

    }

    IEnumerator Respawn()
    {
        panel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        timeLeft = respawnTime;
        yield return new WaitForSeconds(respawnTime);

        panel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        CreateController();
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);

        deaths++;

        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        StartCoroutine(Respawn());
    }

    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
    }

    [PunRPC]
    void RPC_GetKill()
    {
        kills++;

        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }

    // ====================== eeee

    [PunRPC]
    void RPC_SendWinner(Player winnerPlayer, int kills)
    {
        winnerText.text = $"Player [{winnerPlayer.NickName}] Won the game!<br>Kills: {Scoreboard.Instance.GetMostKillsInGame()}";

        if (controller != null)
            PhotonNetwork.Destroy(controller);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        StopCoroutine(Respawn());

        if (controller != null)
            PhotonNetwork.Destroy(controller);

        if (container != null)
            container.SetActive(false);

        winningPanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;


        // need to do this
    }


    // ====================== leave room management ======================

    public void leaveRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = false;

        Destroy(RoomManager.Instance.gameObject);

        //Destroy(DiscordHanderler.Instance.gameObject);

        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        //PhotonNetwork.Disconnect();
        PhotonNetwork.LeaveRoom();
        //while(PhotonNetwork.InRoom)
        while (PhotonNetwork.InRoom)
            yield return null;
        SceneManager.LoadScene(1);
    }
}