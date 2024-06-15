using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GeneratePlayer : MonoBehaviour
{
	public GameObject Player;
	public GameObject GhostPlayer;

	private bool _generatedPlayer = false;

	// Start is called before the first frame update
	void Start()
	{
		if (_generatedPlayer) return;

		if (NetworkClient.localPlayer.isLocalPlayer)
		{
			Instantiate(Player, transform);
		}
		else
		{
			Instantiate(GhostPlayer, transform);
		}
	}
}
