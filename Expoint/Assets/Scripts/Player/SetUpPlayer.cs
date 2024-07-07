using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SetUpPlayer : NetworkBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}


	// we call this function on every client so we can set them up.
	[ClientRpc]
	public void TargetSpawned(PlayerManager pManager)
	{
		print("Confirming I have control");

		if (isOwned) // checking
		{
			print("I am local");
		}
		else
		{
			print("No control");
		}

		// we call the set up functions on the main player scripts so they can destory cameras or set up varibles
		// depending if the player is owned. 
		// FYI: isOwmed is like is local but used to check if the this player's connection to the server owns it.
		gameObject.GetComponent<LookController>().SetUp();
		gameObject.GetComponent<MovementController>().SetUp();
		gameObject.GetComponent<TestSystem>().SetUp(pManager);
	}
}
