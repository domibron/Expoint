using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MouseLockManager : NetworkBehaviour
{
	public static MouseLockManager Instance;

	public static bool IsLocked = false;

	public void Awake()
	{
		if ((Instance != null && Instance != this) || !isLocalPlayer)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (IsLocked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}


}
