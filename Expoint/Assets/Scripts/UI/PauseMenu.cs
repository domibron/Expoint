using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PauseMenu : NetworkBehaviour
{
	public GameObject UI;

	NetworkRoomManager _networkRoomManager;

	private bool _paused = false;

	public GameObject EventManager;

	// Start is called before the first frame update
	void Start()
	{
		_networkRoomManager = NetworkManager.singleton as NetworkRoomManager;

		if (!isLocalPlayer)
		{
			Destroy(EventManager);
			Destroy(this.gameObject);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			_paused = !_paused;
		}


		UI.SetActive(_paused);

	}

	public void Resume()
	{
		_paused = false;
	}

	public void ReturnToRoom()
	{
		NetworkManager.singleton.ServerChangeScene(_networkRoomManager.RoomScene);
	}

	public void ExitToMainMenu()
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
