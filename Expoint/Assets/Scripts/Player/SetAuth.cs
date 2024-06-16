using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SetAuth : NetworkBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	[ClientRpc]
	public void TargetSpawned()
	{
		print("Confirming I have control");

		if (isOwned)
		{
			print("I am local");
		}
		else
		{
			print("No control");
		}

		gameObject.GetComponent<LookController>().SetUp();
		gameObject.GetComponent<MovementController>().SetUp();
		gameObject.GetComponent<TestSystem>().SetUp();
	}
}
