using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MovementController : NetworkBehaviour
{
	public float Speed = 5f;

	public float Gravity = 9.81f;

	public float JumpHeight = 2f;

	private CharacterController _characterController;

	private Vector3 _velocity;

	private bool _isGrounded = false;

	private bool _isSetUpComplete = false;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

		if (!isOwned || !_isSetUpComplete) return;

		GroundCheck();

		HandleGravity();

		_characterController.Move(_velocity * Time.deltaTime);

		Vector3 MovementDirection = HandleMovement();

		_characterController.Move(MovementDirection.normalized * Speed * Time.deltaTime);

		HandleJumping();
	}

	private void GroundCheck()
	{
		bool checkSphereResult = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z), 0.1f);

		if (checkSphereResult || _characterController.isGrounded)
		{
			_isGrounded = true;
		}
		else
		{
			_isGrounded = false;
		}
	}

	private void HandleGravity()
	{

		if (_isGrounded && _velocity.y < -2)
		{
			_velocity = new Vector3(0, -2, 0);
		}
		else
		{
			_velocity.y += (-Gravity) * Time.deltaTime;
		}
	}

	private Vector3 HandleMovement()
	{
		Vector3 InputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

		Vector3 MovementDirection = transform.right * InputDirection.x + transform.forward * InputDirection.z;

		return MovementDirection;
	}

	private void HandleJumping()
	{
		if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
		{
			Vector3 jumpDir = new Vector3(0, Mathf.Sqrt(JumpHeight * -2f * (-Gravity)) - _velocity.y, 0);

			_velocity += jumpDir;
		}
	}

	[ClientRpc]
	public void SetUp()
	{
		if (isOwned)
		{
			_characterController = GetComponent<CharacterController>();

			print("I am client");
		}

		_isSetUpComplete = true;
	}
}
