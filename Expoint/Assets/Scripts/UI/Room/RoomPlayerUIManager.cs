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

		foreach (NetworkRoomPlayer item in _netRoomManager.roomSlots)
		{
			PlayerRoomItem playerRoomItem = Instantiate(PlayerItemPrefab, Perent).GetComponent<PlayerRoomItem>();

			playerRoomItem.NetID = item.GetComponent<NetworkIdentity>();

			playerRoomItem.name = "succ";

			if ((isServer && item.index > 0) || isServerOnly)
			{
				playerRoomItem.KickVisible = true;
			}

			PlayerGameCards.Add(playerRoomItem.gameObject);
		}
	}
}
