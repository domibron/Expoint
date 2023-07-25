using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputManager))]
public class CCMovementController : MonoBehaviour, IWASDInput, ISpaceInput, ILeftShiftInput
{
    CharacterController CC;

    Dictionary<bool[], string> fourAxisOfMovement;

    // ordered W, A, S, D
    bool[] CurrentInput = new bool[4];

    Vector3 moveDir = Vector3.zero;

    Vector3 velocity;

    [SerializeField] private float gravity = -9.81f;

    [SerializeField] private float jumpHeight = 1.4f;

    [SerializeField] float SprintSpeed = 10;

    [SerializeField] float WalkSpeed = 5;

    float MovementSpeedAditive;

    void Awake()
    {
        CC = GetComponent<CharacterController>();
    }

    void Start()
    {
        InitInputDictionary();
    }

    void Update()
    {
        HandleMovement();

        HandleGravity();
    }

    private void HandleMovement()
    {
        try // be quiet, this stops a NRE when the editor reloads.
        {
            CC.Move(MoveDir() * MovementSpeedAditive * Time.smoothDeltaTime);
        }
        catch (NullReferenceException) { }
    }

    private void HandleGravity()
    {
        if (CC.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        CC.Move(velocity * Time.deltaTime);
    }

    private Vector3 MoveDir()
    {

        bool[] _bool = CurrentInput;
        if (_bool[0] && _bool[2])
        {
            _bool[0] = false;
            _bool[2] = false;
        }

        if (_bool[1] && _bool[3])
        {
            _bool[1] = false;
            _bool[3] = false;
        }

        if (_bool[0] && _bool[1] && _bool[2] && _bool[3])
        {
            _bool[0] = false;
            _bool[1] = false;
            _bool[2] = false;
            _bool[3] = false;
        }


        string inputType = GetInputType(fourAxisOfMovement, _bool);


        Vector3 _dir = Vector3.zero;

        if (true)
        {
            switch (inputType)
            {
                default:
                    _dir = Vector3.zero;
                    break;

                case "No Input":
                    _dir = Vector3.zero;
                    break;

                case "Forward":
                    _dir = new Vector3(0, 0, 1);
                    break;

                case "Left":
                    _dir = new Vector3(-1, 0, 0);
                    break;

                case "Backward":
                    _dir = new Vector3(0, 0, -1);
                    break;

                case "Right":
                    _dir = new Vector3(1, 0, 0);
                    break;

                case "Forward Left":
                    _dir = new Vector3(-1, 0, 1);
                    break;

                case "Forward Right":
                    _dir = new Vector3(1, 0, 1);
                    break;

                case "Backward Left":
                    _dir = new Vector3(-1, 0, -1);
                    break;

                case "Backward Right":
                    _dir = new Vector3(1, 0, -1);
                    break;

            }
        }

        _dir = transform.right * _dir.x + transform.forward * _dir.z;

        return _dir.normalized; // ah, stops speed boost for strafing.

    }

    private string GetInputType(Dictionary<bool[], string> dic, bool[] boolArray)
    {
        string inputType = "";
        for (int keyIndex = 0; keyIndex < dic.Count; keyIndex++)
        {
            if (dic.Keys.ElementAt(keyIndex).SequenceEqual(boolArray))
            {
                dic.TryGetValue(dic.Keys.ElementAt(keyIndex), out inputType);
                return inputType;
            }
        }
        return inputType;
    }

    private void InitInputDictionary() // ordered W, A, S, D
    {
        fourAxisOfMovement = new Dictionary<bool[], string>();
        fourAxisOfMovement.Add(new bool[] { false, false, false, false }, "No Input");
        fourAxisOfMovement.Add(new bool[] { true, false, false, false }, "Forward");
        fourAxisOfMovement.Add(new bool[] { false, true, false, false }, "Left");
        fourAxisOfMovement.Add(new bool[] { false, false, true, false }, "Backward");
        fourAxisOfMovement.Add(new bool[] { false, false, false, true }, "Right");
        fourAxisOfMovement.Add(new bool[] { true, true, false, false }, "Forward Left");
        fourAxisOfMovement.Add(new bool[] { true, false, false, true }, "Forward Right");
        fourAxisOfMovement.Add(new bool[] { false, true, true, false }, "Backward Left");
        fourAxisOfMovement.Add(new bool[] { false, false, true, true }, "Backward Right");
        fourAxisOfMovement.Add(new bool[] { true, false, true, false }, "Forward Backward");
        fourAxisOfMovement.Add(new bool[] { false, true, false, true }, "Left Right");
        fourAxisOfMovement.Add(new bool[] { true, true, true, false }, "Forward Backward Left");
        fourAxisOfMovement.Add(new bool[] { true, false, true, true }, "Forward Backward Right");
        fourAxisOfMovement.Add(new bool[] { true, true, false, true }, "Left Right Forward");
        fourAxisOfMovement.Add(new bool[] { false, true, true, true }, "Left Right Backward");
        fourAxisOfMovement.Add(new bool[] { true, true, true, true }, "All Inputs");
    }

    void IWASDInput.W_Key_Held(bool b)
    {
        CurrentInput[0] = b;
    }

    void IWASDInput.A_Key_Held(bool b)
    {
        CurrentInput[1] = b;
    }

    void IWASDInput.S_Key_Held(bool b)
    {
        CurrentInput[2] = b;
    }

    void IWASDInput.D_Key_Held(bool b)
    {
        CurrentInput[3] = b;
    }

    void ISpaceInput.Space_Key_Pressed()
    {
        if (CC.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ILeftShiftInput.Left_Shift_Key_Held(bool b)
    {
        if (b)
        {
            MovementSpeedAditive = SprintSpeed;
        }
        else
        {
            MovementSpeedAditive = WalkSpeed;
        }
    }
}
