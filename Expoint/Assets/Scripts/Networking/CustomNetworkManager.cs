using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;

public class CustomNetworkManager : NetworkRoomManager
{
	public static event Action PlayerListChanged;

	public override void Start()
	{
		base.Start();
	}

	public override void OnServerConnect(NetworkConnectionToClient conn)
	{
		base.OnServerConnect(conn);
	}

	public override void OnRoomClientEnter()
	{
		base.OnRoomClientEnter();


		PlayerListChanged?.Invoke();

	}

	public override void OnRoomClientExit()
	{
		base.OnRoomClientExit();


		PlayerListChanged?.Invoke();

	}

	public void UpdatePlayersUI()
	{
		foreach (RoomPlayer player in roomSlots)
		{
			// update ui but not here
		}
	}
}
