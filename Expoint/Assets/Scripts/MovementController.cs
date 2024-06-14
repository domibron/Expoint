using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MovementController : NetworkBehaviour
{
	public float Speed = 5f;

	private CharacterController _characterController;

	// Start is called before the first frame update
	void Start()
	{
		if (authority)
		{
			_characterController = GetComponent<CharacterController>();

			print("I am client");
		}
	}

	// Update is called once per frame
	void Update()
	{

		if (!authority) return;



		Vector3 InputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

		Vector3 MovementDirection = transform.right * InputDirection.x + transform.forward * InputDirection.z;

		_characterController.Move(InputDirection.normalized * Speed * Time.deltaTime);
	}
}
