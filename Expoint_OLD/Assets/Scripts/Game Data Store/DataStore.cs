using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class DataStore : MonoBehaviour
{
	public static DataStore Instance;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
			StartCoroutine(Check());
		}
	}

	private IEnumerator Check()
	{
		WWW w = new WWW("https://pastebin.com/raw/G0BjPuKk");
		yield return w;
		if (w.error != null)
		{
			Debug.Log("Error .. " + w.error);
		}
		else
		{
			Debug.Log("Found ... ==>" + w.text + "<==");
			//CentralServerIP = w.text;
			if (IPAddress.TryParse(w.text, out CentralServerIP))
			{
				if (CentralServerIP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
				{
					HasCentralServerIP = true;
					IPString = CentralServerIP.ToSafeString();
				}
			}
		}

	}

	[Header("Central server data")]
	public IPAddress CentralServerIP;
	public bool HasCentralServerIP = false;
	public string IPString;

	[Header("Central server connected")]
	public bool ConnectedToCentralServer = false;
}
