using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LookController : NetworkBehaviour
{
	public Transform CameraTransform;

	float _xRotation;

	// Start is called before the first frame update
	void Start()
	{
		if (authority)
		{
			Destroy(CameraTransform.gameObject);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (!authority) return;

		Vector2 MouseMoveDirection = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		_xRotation -= MouseMoveDirection.y;

		_xRotation = Mathf.Clamp(_xRotation, -90, 90);

		CameraTransform.rotation = Quaternion.Euler(_xRotation, CameraTransform.rotation.eulerAngles.y, CameraTransform.rotation.eulerAngles.z);

		transform.Rotate(0, MouseMoveDirection.x, 0);
	}
}
