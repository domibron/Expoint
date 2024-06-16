using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TestSystem : NetworkBehaviour
{
	[SyncVar]
	public float Health = 100f;

	public float Maxheath = 100f;

	public Transform CameraTransform;

	// Start is called before the first frame update
	void Start()
	{
		if (isLocalPlayer) CameraTransform = Camera.main.transform;
	}

	// Update is called once per frame
	void Update()
	{
		if (!isLocalPlayer) return;

		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			RaycastHit hit;
			if (Physics.Raycast(CameraTransform.position, CameraTransform.forward, out hit, 100f))
			{
				if (hit.collider.GetComponent<TestSystem>() == null) return;

				if (hit.collider.GetComponentInParent<NetworkIdentity>() == null) return;

				TakeDamage(hit.collider.GetComponentInParent<NetworkIdentity>().connectionToClient, 10);
			}

		}
	}

	[TargetRpc]
	public void TakeDamage(NetworkConnectionToClient target, float damage)
	{
		Health -= damage;
	}

	public void CmdShoot(GameObject target)
	{
		if (target.GetComponent<TestSystem>() != null)
		{
			target.GetComponent<TestSystem>().Health -= 10f;
		}
	}
}
