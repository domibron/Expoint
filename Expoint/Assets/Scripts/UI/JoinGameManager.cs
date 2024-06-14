using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class JoinGameManager : MonoBehaviour
{
	public NetworkManager NetManager;

	public TMP_InputField IPInputField;
	public TMP_InputField PortInputField;

	// Start is called before the first frame update
	void Start()
	{
		NetManager = NetworkManager.singleton;



		IPInputField.text = NetManager.networkAddress;

		if (Transport.active is PortTransport portTransport)
		{
			PortInputField.text = portTransport.Port.ToString();
		}
		else
		{
			// we disable the port input field as we dont have a port transport.
			PortInputField.enabled = false;
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void JoinGame()
	{
		if (NetworkClient.active)
		{
			Debug.LogError("You're in a game already!");

			return;
		}

		NetManager.StartClient();

		NetManager.networkAddress = IPInputField.text;

		if (Transport.active is PortTransport portTransport)
		{
			if (ushort.TryParse(PortInputField.text, out ushort port))
				portTransport.Port = port;
		}
	}
}
