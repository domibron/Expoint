using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWASDInput
{
    void W_Key_Held(bool b);
    void A_Key_Held(bool b);
    void S_Key_Held(bool b);
    void D_Key_Held(bool b);
}
