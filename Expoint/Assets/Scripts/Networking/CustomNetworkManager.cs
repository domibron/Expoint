using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkRoomManager
{
	public override void Start()
	{
		base.Start();
	}

	public void ConnectToIPAdress()
	{
		string MyAttualIP = "86.10.14.161";
		NetworkClient.Connect(MyAttualIP);
	}
}
