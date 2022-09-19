using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Net;

public class Rifle : Gun
{
    [SerializeField] Camera cam;
    [SerializeField] TMP_Text ammoText;

    PhotonView PV;

    int currentAmmo;
    int currentReserveAmmo;

    float TimeUntilNextFire;

    bool isReloading;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        currentAmmo = ((GunInfo)itemInfo).maxAmmoInClip;
        currentReserveAmmo = ((GunInfo)itemInfo).maxAmmoInReserve;
    }

    public override void UseRKey()
    {
        StartCoroutine(Reload());
    }


    private void Update()
    {
        if (((GunInfo)itemInfo).name == "Rifle")
        {
            ammoText.text = $"{currentAmmo} / {((GunInfo)itemInfo).maxAmmoInClip}\n   {currentReserveAmmo}";
        }
    }

    public override void UseMouse0()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("out off ammo reload!");
            return;
        }

        if (Time.time < TimeUntilNextFire) // fire rate
        {
            return;
        }

        Shoot();
    }

    IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(((GunInfo)itemInfo).reloadSpeed);

        if (currentReserveAmmo > 0)
        {
            int ammoToReload;
            ammoToReload = ((GunInfo)itemInfo).maxAmmoInClip - currentAmmo;

            if (ammoToReload <= currentReserveAmmo)
            {
                currentAmmo += ammoToReload;
                currentReserveAmmo -= ammoToReload;
            }
            else
            {
                currentAmmo += currentReserveAmmo;
                currentReserveAmmo -= currentReserveAmmo;
            }
        }

        isReloading = false;
    }

    void Shoot()
    {
        if (Time.time < TimeUntilNextFire || isReloading) // fire rate
        {
            return;
        }

        TimeUntilNextFire = Time.time + 1f / ((GunInfo)itemInfo).fireRate;

        currentAmmo--;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            PV.RPC(nameof(RPC_Shoot), RpcTarget.All, hit.point, hit.normal);

            Vector3 start = cam.transform.position;
            PV.RPC(nameof(RPC_DrawTracer), RpcTarget.All, start, hit.point);
        }
        else
        {
            Physics.Raycast(ray, 100);
            Vector3 end = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);

            Vector3 start = cam.transform.position;
            PV.RPC(nameof(RPC_DrawTracer), RpcTarget.All, start, ray.GetPoint(100f));
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

    [PunRPC]
    void RPC_DrawTracer(Vector3 start, Vector3 end)
    {
        GameObject trail = Instantiate(tracerPrefab, start, Quaternion.identity);
        TrailMove trailMove = trail.GetComponent<TrailMove>();

        trailMove.startPos = start;
        trailMove.goToPos = end;
        trailMove.Speed = 1f;

        Destroy(trail, 2f);

        
    }
}
