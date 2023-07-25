using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    MasterInputSystem input;

    // this basically allows the script to only run detection for keys that are registered to scripts.
#nullable enable
    IWASDInput? _IWASDInput;
    ISpaceInput? _ISpaceInput;
    ILeftShiftInput? _ILeftShiftInput;
    IMouseInput? _IMouseInput;
    IFKeyInput? _IFKeyInput;
#nullable restore


    void Awake()
    {
        // very inportant, this is how we check for input.
        input = new MasterInputSystem();

        _IWASDInput = GetComponent<IWASDInput>();
        _ISpaceInput = GetComponent<ISpaceInput>();
        _ILeftShiftInput = GetComponent<ILeftShiftInput>();
        _IMouseInput = GetComponent<IMouseInput>();
        _IFKeyInput = GetComponent<IFKeyInput>();
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        Debug.LogWarningFormat("Disabled the Input Manager");
        input.Disable();
    }

    void Update()
    {
        if (_IWASDInput != null)
        {
            Key_W_Detection_Hold();
            Key_A_Detection_Hold();
            Key_S_Detection_Hold();
            Key_D_Detection_Hold();
        }

        if (_ISpaceInput != null)
        {
            Key_Space_Detection_Press();
        }

        if (_ILeftShiftInput != null)
        {
            Key_Left_Shift_Detection_Hold();
        }

        if (_IMouseInput != null)
        {
            Mouse_X_Move(input.Player.MouseX.ReadValue<float>() / 20); // devided by 20 because its the same balues as Input.GetAxisRaw("Mouse X") and Y.
            Mouse_Y_Move(input.Player.MouseY.ReadValue<float>() / 20); // Its beautiful. ^
        }

        if (_IFKeyInput != null)
        {
            Key_F_Detection_Pressed();
        }
    }

    private void Key_F_Detection_Pressed()
    {
        if (input.Player.F.WasPressedThisFrame()) _IFKeyInput.F_Key_Pressed();
    }

    private void Mouse_X_Move(float x)
    {
        _IMouseInput.mouseX(x);
    }

    private void Mouse_Y_Move(float y)
    {
        _IMouseInput.mouseY(y);
    }

    private void Key_Left_Shift_Detection_Hold() // this could be done better, might re do, but for now, it's a better path than directly checking inside one script. ? ~ this was posted before new input system.
    {
        if (input.Player.LeftShift.IsPressed()) _ILeftShiftInput.Left_Shift_Key_Held(true);
        else _ILeftShiftInput.Left_Shift_Key_Held(false);
    }

    private void Key_Space_Detection_Press()
    {
        if (input.Player.Space.WasPressedThisFrame()) _ISpaceInput.Space_Key_Pressed();
    }

    // this seems wrong, IsPressed feels out of place, but I will still continue.
    private void Key_D_Detection_Hold()
    {
        if (input.Player.D.IsPressed()) _IWASDInput.D_Key_Held(true);
        else _IWASDInput.D_Key_Held(false);
    }

    private void Key_S_Detection_Hold()
    {
        if (input.Player.S.IsPressed()) _IWASDInput.S_Key_Held(true);
        else _IWASDInput.S_Key_Held(false);
    }

    private void Key_A_Detection_Hold()
    {
        if (input.Player.A.IsPressed()) _IWASDInput.A_Key_Held(true);
        else _IWASDInput.A_Key_Held(false);
    }

    public void Key_W_Detection_Hold()
    {
        if (input.Player.W.IsPressed()) _IWASDInput.W_Key_Held(true);
        else _IWASDInput.W_Key_Held(false);
    }
}
