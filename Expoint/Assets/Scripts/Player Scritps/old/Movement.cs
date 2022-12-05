using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform orientation;
    public Rigidbody rb;

    [Header("Keybinds")]
    public KeyCode jumpKey;

    [Header("Basic Movement")]
    public float airMultiplier;

    public bool IsMoving { get; private set; } = false;
    public bool IsGrounded { get; private set; } = false;
    public bool IsCrouching { get; private set; } = false; // will use this soon
    public float CurrentMovementSpeed { get; private set; } = 0f;

    private float horizontalMovement = 0f;
    private float verticalMovement = 0f;
    private float movementMultiplier = 20f;

    [Header("Sprinting")]
    public float walkSpeed;
    public float sprintSpeed;
    public float acceleration;

    [Header("Jumping")]
    public float jumpForce;

    [Header("Drag and Gravity")]
    public float groundDrag;
    public float airDrag;

    [Header("Ground Detection")]
    private Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 slopeMoveDirection = Vector3.zero;
    private RaycastHit slopeHit; // out
                                 //private Vector3 jumpMoveDirection; May need this in the future


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        orientation = transform.Find("Orientation").GetComponent<Transform>();
        groundCheck = transform.Find("Ground Check").GetComponent<Transform>();
    }

    private void Update()
    {
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //is grounded check

        //runs myinput function and control drag (more over this further down)
        SetMoveDirectionFromInput();
        ControlDrag();
        ControlSpeed();

        //allows the player to jump if they are on the ground and presses the jump key
        if (Input.GetKeyDown(jumpKey) && IsGrounded)
        {
            Jump();
        }

    }

    private void FixedUpdate()
    {
        MovePlayer();

    }

    private void SetMoveDirectionFromInput()
    {
        //grabs key inputs
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        //combines both inputs into one direction
        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    void ControlDrag()
    {
        //drag is controlled if in air or ground so the player is not slow falling
        if (IsGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    void ControlSpeed()
    {
        float ySpeed = rb.velocity.y;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // clamps to max speed
        if (rb.velocity.magnitude > sprintSpeed)
        {
            rb.velocity = rb.velocity.normalized * sprintSpeed;
        }

        rb.velocity = new Vector3(rb.velocity.x, ySpeed, rb.velocity.z);
        // end of clamp

        if (IsGrounded && Input.GetAxis("Vertical") > 0) // if moving forward
        {
            CurrentMovementSpeed = Mathf.Lerp(CurrentMovementSpeed, sprintSpeed, acceleration * Time.deltaTime / 2);

            IsMoving = true;
        }
        else if (IsGrounded && Mathf.Abs(Input.GetAxis("Vertical")) > 0 && Mathf.Abs(Input.GetAxis("Horizontal")) > 0) // else if moving another direction
        {
            CurrentMovementSpeed = Mathf.Lerp(CurrentMovementSpeed, walkSpeed, acceleration * Time.deltaTime / 2);

            IsMoving = true;
        }
        else if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
        {
            CurrentMovementSpeed = walkSpeed / 2;

            IsMoving = false;
        }
    }

    void Jump() //when called then the player will jump in the air
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); //add a jump force to the rigid body component.
    }

    private bool IsOnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 2f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    void MovePlayer()
    {
        if (IsGrounded && !IsOnSlope()) // if on the ground but not on a slope
        {
            // walk speed on flat ground
            rb.AddForce(moveDirection.normalized * CurrentMovementSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (IsGrounded && IsOnSlope()) // if the player is on the ground and on a slope
        {
            // walk speed on slope
            rb.AddForce(slopeMoveDirection.normalized * CurrentMovementSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (!IsGrounded) // if the player is not on the ground
        {
            // jumping in mid air force with a downwards force on a wall
            rb.AddForce(moveDirection.normalized * CurrentMovementSpeed * airMultiplier, ForceMode.Acceleration);
        }
    }
}
