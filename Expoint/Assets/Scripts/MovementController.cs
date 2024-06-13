using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	public float Speed = 5f;

	private CharacterController _characterController;

	// Start is called before the first frame update
	void Start()
	{
		_characterController = GetComponent<CharacterController>();
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 InputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

		_characterController.Move(InputDirection.normalized * Speed * Time.deltaTime);
	}
}
