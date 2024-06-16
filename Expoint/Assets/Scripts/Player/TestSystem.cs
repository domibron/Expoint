using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TestSystem : NetworkBehaviour
{
	public float Health = 100f;

	public float Maxheath = 100f;

	public Transform CameraTransform;

	private bool _isSetUpCompleate = false;

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
	}

	[TargetRpc]
	public void TargetTakeDamage(NetworkConnectionToClient target, float damage)
	{
		print($"Taken damage {damage}");
	}

	public void CmdTakeDamage(GameObject target, int damage)
	{
		target.GetComponent<TestSystem>().Health -= damage;

		print($"Delt damage");

		NetworkIdentity netID = target.GetComponentInParent<NetworkIdentity>();

		TargetTakeDamage(netID.connectionToClient, damage);
	}

	[ClientRpc]
	public void SetUp()
	{
		if (isOwned)
		{
			CameraTransform = Camera.main.transform;
		}

		_isSetUpCompleate = true;
	}
}
