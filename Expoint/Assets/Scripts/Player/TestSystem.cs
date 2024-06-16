using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TestSystem : NetworkBehaviour
{
	[SyncVar(hook = nameof(OnHealthUpdated))]
	public float Health = 100f;

	public Transform CameraTransform;

	private bool _isSetUpCompleate = false;

	public PlayerManager playerManager;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (!isOwned || !_isSetUpCompleate) return;

		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			RaycastHit hit;
			if (Physics.Raycast(CameraTransform.position, CameraTransform.forward, out hit, 100f))
			{
				if (hit.collider.GetComponent<TestSystem>() == null) return;

				if (hit.collider.GetComponentInParent<NetworkIdentity>() == null) return;

				if (hit.collider.tag == "Player")
				{
					print("hitting self");
					return;
				}

				CmdTakeDamage(hit.collider.gameObject, 10);
			}
		}

		if (Health <= 0)
		{
			playerManager.CmdRemovePlayer(this.gameObject);
		}
	}

	[TargetRpc]
	public void TargetTakeDamage(NetworkConnectionToClient target, float damage)
	{
		print($"Taken damage {damage}");

		Health -= damage;
	}

	[Command]
	public void CmdTakeDamage(GameObject target, int damage)
	{


		print($"Delt damage");

		NetworkIdentity netID = target.GetComponent<NetworkIdentity>();

		TargetTakeDamage(netID.connectionToClient, damage);
	}

	public void SetUp(PlayerManager pManager)
	{
		if (isOwned)
		{
			CameraTransform = Camera.main.transform;
		}

		playerManager = pManager;

		_isSetUpCompleate = true;
	}

	public void OnHealthUpdated(float OldHealth, float NewHealth)
	{
		Health = NewHealth;
	}
}
