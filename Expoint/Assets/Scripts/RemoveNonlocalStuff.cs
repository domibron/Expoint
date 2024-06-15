using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class RemoveNonlocalStuff : NetworkBehaviour
{
	[SerializeField]
	public List<GameObject> RemoveIfNotLocal = new List<GameObject>();

	private bool _removedAllItems = false;

	// Start is called before the first frame update
	void Start()
	{
		if (isLocalPlayer) return;

		if (_removedAllItems) return;

		for (int i = 0; i < RemoveIfNotLocal.Count; i++)
		{
			Destroy(RemoveIfNotLocal[0]);
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
