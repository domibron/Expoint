using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
	public NetworkManager NetManager;

	public GameObject MainUI;
	public GameObject LoadingScreen;

	public TMP_Text TimeElapsedUI;

	private bool _isCounting;

	private float _count;

	// Start is called before the first frame update
	void Start()
	{
		NetManager = NetworkManager.singleton;

		MainUI.SetActive(true);

		LoadingScreen.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		if (!NetworkClient.active)
		{
			MainUI.SetActive(true);

			LoadingScreen.SetActive(false);

			_isCounting = false;
		}
		else
		{
			MainUI.SetActive(false);

			LoadingScreen.SetActive(true);

			_isCounting = true;
		}

		if (_isCounting)
		{
			_count += Time.deltaTime;

			TimeElapsedUI.text = _count.ToString("F1");
		}
		else
		{
			_count = 0f;
		}
	}

	public void StartHost()
	{
		NetManager.StartHost();
	}

	public void StopConnection()
	{
		NetManager.StopClient();
	}
}
