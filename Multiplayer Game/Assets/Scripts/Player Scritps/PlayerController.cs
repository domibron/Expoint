using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;
    [SerializeField] GameObject pauseMenu;

    [SerializeField] GameObject cameraHolder;

    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    [SerializeField] Item[] items;

    int itemIndex;
    int previousItemIndex = -1;

    float verticalLookRotation;
    bool grounded = false;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;

    PhotonView PV;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager playerManager;

    RaycastHit slopeHit;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);
            Destroy(pauseMenu);
        }
    }

    void Update()
    {
        if (!PV.IsMine) return;

        Look();
        Move();
        Jump();

        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }

        if (Input.GetMouseButton(0))
        {
            items[itemIndex].UseMouse0();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            items[itemIndex].UseRKey();
        }

        if (transform.position.y < -20f) // dies if below y value in the world
        {
            Die();
        }

    }

    void FixedUpdate()
    {
        if (!PV.IsMine) return;

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    private bool IsOnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, gameObject.GetComponent<CapsuleCollider>().height / 2f)) // might need to make this value scale with player later
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

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded && !IsOnSlope()) // other than onslope, this is defualt jumping.
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce);
        }
        //else if (Input.GetKeyDown(KeyCode.Space) && grounded && IsOnSlope()) // inside is temp. Will rework or change
        //{
        //    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        //    rb.AddForce(slopeHit.normal * jumpForce); // will jump the up vector of the slope.
        //}
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex) return;

        itemIndex = _index;

        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("ItemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) // this player properties has changed.
    {
        if (changedProps.ContainsKey("ItemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["ItemIndex"]);
        }
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void HandleGravity()
    {
        float currentVerticalSpeed = rb.velocity.y;

        if (grounded)
        {
            if (currentVerticalSpeed < 0f)
                currentVerticalSpeed = 0f;
            rb.velocity = new Vector3(rb.velocity.x, currentVerticalSpeed, rb.velocity.z);
        }
        //else if (!grounded)
        //{
        //    currentVerticalSpeed -= 1f * Time.deltaTime;
        //}

        //rb.velocity = new Vector3(rb.velocity.x, currentVerticalSpeed, rb.velocity.z);
    }

    void Move()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);

        //HandleGravity(); // MOVE TO FIXED UPDATE - posibly??

        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        print(rb.velocity.magnitude + "vm | v" + rb.velocity);

        if (grounded)
        {
            if (IsOnSlope())
            {
                Vector3 moveDir2 = (Vector3.back * verticalMovement + Vector3.left * horizontalMovement).normalized;
                Vector3 slopeMoveDir = Vector3.ProjectOnPlane(moveDir2, slopeHit.normal);

                print(moveDir);
                print(slopeMoveDir);

                moveAmount = Vector3.SmoothDamp(moveAmount, -slopeMoveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
            }
            else
            {
                moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
            }

            // moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
        }
        else
        {
            moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed / 1.5f : walkSpeed / 1.5f), ref smoothMoveVelocity, smoothTime);
            // want to half the speed when in the air to stop  mad air strafing but it should be a thing tho, that's why I am not removeing it but reducing it.
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;

        healthbarImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    void Die() // function to call to kill player
    {
        playerManager.Die();
    }
}
