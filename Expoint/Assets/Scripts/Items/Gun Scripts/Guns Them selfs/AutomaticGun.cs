using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Net;

public class AutomaticGun : Gun
{
    [SerializeField] Camera cam;
    [SerializeField] PlayerController2 playerController2;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] GameObject gunModel;

    [SerializeField] GameObject HipPos;
    [SerializeField] GameObject ADSPos;

    [SerializeField] AudioSource audioSource;

    [SerializeField, Range(1, 1000)] float SPEED;

    PhotonView PV;

    int currentAmmo;
    int currentReserveAmmo;

    float TimeUntilNextFire;

    bool isReloading;

    float reloadTimeLeft;

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
        if (currentAmmo < ((GunInfo)itemInfo).maxAmmoInClip)
        {
            StartCoroutine(Reload());
        }
    }


    private void Update()
    {
        if (!PV.IsMine)
            return;

        if (itemGameObject.activeSelf == false)
            return;


        if (Input.GetMouseButton(1)) // FOV VALUES ARE HARD SET AND ARE THE SAME FOR ALL WEAPONS
        {
            gunModel.transform.localPosition = Vector3.Lerp(gunModel.transform.localPosition, ADSPos.transform.localPosition, 10f * Time.deltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 30f, 10f * Time.deltaTime);
            playerController2.mouseSensitivityMultiplyer = 0.2f; // THIS IS NOT MENT TO BE HARD SET, FIX SOON!
        }
        else
        {
            gunModel.transform.localPosition = Vector3.Lerp(gunModel.transform.localPosition, HipPos.transform.localPosition, 10f * Time.deltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, 10f * Time.deltaTime);
            playerController2.mouseSensitivityMultiplyer = 1;
        }


        if (!isReloading)
        {
            ammoText.text = $"{currentAmmo} / {((GunInfo)itemInfo).maxAmmoInClip}<br>   {currentReserveAmmo}";
        }
        else
        {
            reloadTimeLeft -= Time.deltaTime;
            ammoText.text = $"<mspace=0.75em>{(Mathf.Round(reloadTimeLeft * 100f) / 100f).ToString("N2")}</mspace><br>## / {((GunInfo)itemInfo).maxAmmoInClip}<br>   {currentReserveAmmo}";
        }
    }

    //private void setfox(float value) // make a fov hander
    //{
    //	cam.fov = value;
    //	weaponCam.fov = value;
    //}

    public override void UseMouse0()
    {
        if (currentAmmo <= 0)
        {
            //Debug.Log("out off ammo reload!"); // create a hint box not a f****** log
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

        reloadTimeLeft = ((GunInfo)itemInfo).reloadSpeed;

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
        StopCoroutine(Reload());

        if (Time.time < TimeUntilNextFire || isReloading) // fire rate.
        {
            return;
        }

        TimeUntilNextFire = Time.time + 1f / ((GunInfo)itemInfo).fireRate;

        currentAmmo--;

        PV.RPC(nameof(RPC_SoundPlay), RpcTarget.All);

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            IDamageable ID = hit.collider.gameObject.GetComponentInParent<IDamageable>();

            if (ID != null) // try using tags instead.
            {
                PhotonView PVTarg = hit.transform.GetComponentInParent<PhotonView>();

                if ((int)PVTarg.Owner.CustomProperties["team"] != (int)PV.Owner.CustomProperties["team"])
                {
                    print($"targ : {PVTarg.Owner.CustomProperties["team"]} | {PV.Owner.CustomProperties["team"]}");

                    hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage); // CAN SIMPLIFY THIS.

                }
            }

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
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal) // universal function for all clients.
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
    void RPC_DrawTracer(Vector3 start, Vector3 end) // universal function for all clients.
    {
        GameObject trail = Instantiate(tracerPrefab, start, Quaternion.identity); // creates the trail object at start pos.
        TrailMove trailMove = trail.GetComponent<TrailMove>(); // grabs the component so i don't need to call get component.

        float distance = Vector3.Distance(end, start); // Gets the distance between the two points for speed calc.
        float speed = SPEED; // speed the projectile should travel at. Change this value.
        float trailSpeed = speed / distance; // trail speed it needs to go at from speed. done as vector3.Lerp will make far trails faster and closer ones slower.

        trailMove.startPos = start; // starting pos.
        trailMove.goToPos = end; // target pos.
        trailMove.Speed = trailSpeed; // for 100f make a varible base distance - put in trail script.

        Destroy(trail, 1f); // keeps it for a second.


    }

    [PunRPC]
    void RPC_SoundPlay()
    {
        audioSource.Play();
    }
}
