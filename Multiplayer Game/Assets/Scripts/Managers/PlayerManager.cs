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

    public float durationOfMatch;
    public int maxKills;

    public float matchTime;

    bool isGameOver;

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

    void Start()
    {
        panel.SetActive(false);
        currentRoom = PhotonNetwork.CurrentRoom;

        foreach (Player _player in PhotonNetwork.PlayerList)
        {
            if (_player.IsMasterClient)
            {
                masterPlayer = _player;
                print("got em");
            }
        }

        if (PV.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                matchTime = durationOfMatch * 60;
                Hashtable hash = new Hashtable();
                hash.Add("MasterTime", matchTime);
                hash.Add("MasterKills", maxKills);
                PhotonNetwork.NetworkingClient.OpSetCustomPropertiesOfRoom(hash);
            }

            CreateController();
        }
        else
        {
            Destroy(container);
            //Destroy(panel);
        }
        print(" ; " + matchTime);
    }

    void Update()
    {




        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable hash = new Hashtable();
            hash.Add("MasterTime", matchTime);
            PhotonNetwork.NetworkingClient.OpSetCustomPropertiesOfRoom(hash);
        }

        if (PV.IsMine)
        {



            if (!isGameOver && matchTime != 0)
                matchTime -= Time.deltaTime;

            string holder = (Mathf.Round((matchTime / 60f) * 100f) / 100f).ToString("N2"); // time left display
            TopMidInfo.text = $"Max kills: {maxKills} | time remaining: <mspace=30>{holder}";

            if (matchTime <= 0)
            {
                // GameOver send RPC event


                // handle kills count
            }

            if (kills >= maxKills)
            {
                // game over
                // this player wins
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
    void RPC_SendWinner()
    {
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