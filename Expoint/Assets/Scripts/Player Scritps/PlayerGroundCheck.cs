using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController2 playerController2;

    void Awake()
    {
        playerController2 = GetComponentInParent<PlayerController2>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerController2.gameObject)
            return;

        playerController2.SetGroundedState(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerController2.gameObject)
            return;

        playerController2.SetGroundedState(false);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerController2.gameObject)
            return;

        playerController2.SetGroundedState(true);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == playerController2.gameObject)
            return;

        playerController2.SetGroundedState(true);
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject == playerController2.gameObject)
            return;

        playerController2.SetGroundedState(false);
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject == playerController2.gameObject)
            return;

        playerController2.SetGroundedState(true);
    }
}
