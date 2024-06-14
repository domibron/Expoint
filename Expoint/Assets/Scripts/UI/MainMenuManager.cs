using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MainMenuManager : MonoBehaviour
{
	public NetworkManager NetManager;

	// Start is called before the first frame update
	void Start()
	{
		NetManager = NetworkManager.singleton;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void StartHost()
	{
		NetManager.StartHost();
	}
}
