using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LookController : NetworkBehaviour
{
	public Transform CameraTransform;

	float _xRotation;

	private bool _setUpCompleate = false;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (!isOwned || !_setUpCompleate) return;

		Vector2 MouseMoveDirection = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		_xRotation -= MouseMoveDirection.y;

		_xRotation = Mathf.Clamp(_xRotation, -90, 90);

		CameraTransform.rotation = Quaternion.Euler(_xRotation, CameraTransform.rotation.eulerAngles.y, CameraTransform.rotation.eulerAngles.z);

		transform.Rotate(0, MouseMoveDirection.x, 0);
	}


	public void SetUp()
	{
		if (!isOwned)
		{
			Destroy(CameraTransform.gameObject);
		}

		_setUpCompleate = true;
	}
}
