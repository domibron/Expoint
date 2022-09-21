using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEditor;
using UnityEngine.UI;

public class Luancher : MonoBehaviourPunCallbacks
{
    public List<RoomInfo> Rooms = new List<RoomInfo>();

    public static Luancher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] Slider maxPlayersSlider;
    [SerializeField] TMP_Text maxPlayerSliderText;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text playersOutOfMaxPlayersText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject PlayerListItemPrefab;
    [SerializeField] GameObject startGameButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Debug.Log("connecting to Lobby");
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        if (PhotonNetwork.InRoom)
            playersOutOfMaxPlayersText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";

        if (MenuManager.Instance.ReturnIsOpenMenuName("create room"))
            maxPlayerSliderText.text = $"Maxplayers:\n{maxPlayersSlider.value} / {maxPlayersSlider.maxValue}";
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)maxPlayersSlider.value;

        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        playersOutOfMaxPlayersText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");

    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> p_list)
    {
        base.OnRoomListUpdate(p_list);

        foreach (var room in p_list)
        {
            for (int i = 0; i < p_list.Count; i++)
            {
                if (room.Name == p_list[i].Name)
                {
                    Rooms.Remove(room);
                }
            }

            if (room.RemovedFromList)
            {
                Rooms.Remove(room);
                continue;
            }

            if (room.IsVisible == false)
            {
                Rooms.Remove(room);
                continue;
            }

            if (room.IsOpen == false)
            {
                Rooms.Remove(room);
                continue;
            }

            Rooms.Add(room);
        }

        //foreach (Transform trans in roomListContent)
        //{
        //    Destroy(trans.gameObject);
        //}

        //for (int i = 0; i < roomList.Count; i++)
        //{
        //    if (roomList[i].RemovedFromList)
        //        continue;
        //    else
        //        Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        //}

        ClearRoomList();

        foreach (var room in Rooms)
        {
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(room);
        }
    }

    public void ClearRoomList()
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }





    public void QuitGame()
    {
        Application.Quit();
    }
}
