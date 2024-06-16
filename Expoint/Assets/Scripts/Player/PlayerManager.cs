using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
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
		// dunno
		_netManager = NetworkManager.singleton as CustomNetworkManager;

		// very impiortnat we regenerate the prefab in the spawnable list so we can spawn.
		NetworkClient.RegisterPrefab(PlayerCharacterPrefab);

		// we call the function that will run on the server to spawn the player, the server is a master scene.
		CmdSpawnPlayer(netIdentity.connectionToClient);

		DontDestroyOnLoad(this.gameObject);
	}

	// we need this if we reload the scene or go back to the main menu.
	void OnDestroy()
	{
		NetworkClient.UnregisterPrefab(PlayerCharacterPrefab);
	}

	// spawn player.
	[Command]
	public void CmdSpawnPlayer(NetworkConnectionToClient conn)
	{
		GameObject player = Instantiate(PlayerCharacterPrefab, Vector3.zero, quaternion.identity);
		// used to spawn the prefab with authority.
		NetworkServer.Spawn(player, conn);

		// we make sure we only spawn one player.
		PlayerHasSpawned = true;

		// calls setup player rpc on server so the players on the clients can do their thing.
		player.GetComponent<SetUpPlayer>().TargetSpawned(this);
	}

	[Command]
	public void CmdRemovePlayer(GameObject player)
	{
		ClientRemovePlayer(player);
	}

	[ClientRpc]
	public void ClientRemovePlayer(GameObject player)
	{
		Destroy(player);
	}

}
