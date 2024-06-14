using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkRoomManager
{
	public override void Start()
	{
		base.Start();
	}

	public override void OnServerConnect(NetworkConnectionToClient conn)
	{
		base.OnServerConnect(conn);
	}

	public void ConnectToIPAdress()
	{
		string MyAttualIP = "86.10.14.161";
		NetworkClient.Connect(MyAttualIP);
	}
}
