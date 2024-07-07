using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RoomPlayer : NetworkRoomPlayer
{
	public NetworkIdentity NetID;

	public override void Start()
	{
		base.Start();

		NetID = GetComponent<NetworkIdentity>();
	}

	void Update()
	{

	}
}
