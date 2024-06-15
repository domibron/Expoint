using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerRoomItem : MonoBehaviour
{
	public string Name = "NONAME";

	public TMP_Text NameDisplay;

	public bool KickVisible = false;

	public Button KickButton;

	public Button ReadyButton;

	public TMP_Text ReadyButtonText;

	public bool ReadyVisisble = false;

	public NetworkIdentity NetID;

	public NetworkRoomPlayer Player;

	private bool _isReady = false;


	public void SetPlayerRoomItem(string name, bool kickVisible, NetworkIdentity netID)
	{
		Name = name;

		KickVisible = kickVisible;

		NetID = netID;
	}

	// Start is called before the first frame update
	void Start()
	{
		KickButton.onClick.AddListener(Kick);

		ReadyButton.onClick.AddListener(Ready);
	}

	public void Kick()
	{
		NetID.connectionToClient.Disconnect();
	}


	public void Ready()
	{
		_isReady = !_isReady;

		Player.CmdChangeReadyState(_isReady);
	}

	// Update is called once per frame
	void Update()
	{
		NameDisplay.text = Name;

		KickButton.gameObject.SetActive(KickVisible);

		if (ReadyButton.gameObject.activeSelf)
		{
			ReadyButtonText.text = (_isReady) ? "UnReady" : "Ready";
		}
	}
}
