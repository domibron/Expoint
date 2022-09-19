using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Net;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    PhotonView PV;

    //fire Rate stuff
    float TimeUntilNextFire;


    //..

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void UseRKey()
    {
        throw new System.NotImplementedException();
    }

    public override void UseMouse0()
    {
           Shoot();
    }

    void Shoot()
    {
        if (Time.time < TimeUntilNextFire) // fire rate
        {
            Debug.Log("Failed " + Time.time + " : ttnf " + TimeUntilNextFire);
            return;
        }

        TimeUntilNextFire = Time.time + 1f / ((GunInfo)itemInfo).fireRate;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
            GameObject bulletImpactObject = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObject, 10f);
            bulletImpactObject.transform.SetParent(colliders[0].transform);
        }
    }
}
