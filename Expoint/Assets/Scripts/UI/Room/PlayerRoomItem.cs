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

	public NetworkIdentity NetID;

	public void SetPlayerRoomItem(string name, bool kickVisible, NetworkIdentity netID)
	{
		Name = name;

		KickVisible = kickVisible;

		NetID = netID;
	}

	// Start is called before the first frame update
	void Start()
	{
		KickButton.onClick.AddListener(OnKick);
	}

	public void OnKick()
	{
		NetID.connectionToClient.Disconnect();
	}

	// Update is called once per frame
	void Update()
	{
		NameDisplay.text = Name;

		KickButton.gameObject.SetActive(KickVisible);
	}
}
