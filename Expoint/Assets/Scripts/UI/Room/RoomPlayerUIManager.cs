using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class RoomPlayerUIManager : NetworkBehaviour
{


	public GameObject PlayerItemPrefab;

	public Transform Perent;

	private CustomNetworkManager _netRoomManager;


	private List<GameObject> PlayerGameCards = new List<GameObject>();


	void Start()
	{
		_netRoomManager = NetworkManager.singleton as CustomNetworkManager;

		UpdatePlayers();
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnEnable()
	{
		CustomNetworkManager.PlayerListChanged += UpdatePlayers;
	}

	void OnDisable()
	{
		CustomNetworkManager.PlayerListChanged -= UpdatePlayers;
	}

	public void UpdatePlayers()
	{
		foreach (GameObject item in PlayerGameCards.ToList<GameObject>())
		{
			Destroy(item);
		}
		try
		{
			foreach (NetworkRoomPlayer item in _netRoomManager.roomSlots)
			{
				PlayerRoomItem playerRoomItem = Instantiate(PlayerItemPrefab, Perent).GetComponent<PlayerRoomItem>();

				playerRoomItem.NetID = item.GetComponent<NetworkIdentity>();

				playerRoomItem.Player = item;

				playerRoomItem.Name = $"player {item.index}";

				if ((isServer && item.index > 0) || isServerOnly)
				{
					playerRoomItem.KickVisible = true;
				}

				if (item.isLocalPlayer)
				{
					playerRoomItem.ReadyVisisble = true;
				}

				PlayerGameCards.Add(playerRoomItem.gameObject);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Caught: " + ex.Message, this);
		}
	}

	public void DisconnectPlayer()
	{
		if (NetworkServer.active && NetworkClient.isConnected)
		{
			NetworkManager.singleton.StopHost();
		}
		else if (NetworkClient.isConnected)
		{
			NetworkManager.singleton.StopClient();
		}
	}
}
