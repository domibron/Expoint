using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
	private CustomNetworkManager _netManager;

	public GameObject PlayerCharacterPrefab;

	[SyncVar]
	public bool PlayerHasSpawned = false;

	// Start is called before the first frame update
	void Start()
	{
		_netManager = NetworkManager.singleton as CustomNetworkManager;

		CmdSpawnPlayer(netIdentity.connectionToClient);
	}

	// Update is called once per frame
	void Update()
	{

	}

	[Command]
	public void CmdSpawnPlayer(NetworkConnectionToClient conn)
	{
		GameObject player = Instantiate(PlayerCharacterPrefab);
		NetworkServer.Spawn(player, conn);


		player.GetComponent<CharacterController>().enabled = false;
		player.transform.position = Vector3.zero;
		player.GetComponent<CharacterController>().enabled = true;


		PlayerHasSpawned = true;

		player.GetComponent<SetAuth>().TargetSpawned();
	}

}
