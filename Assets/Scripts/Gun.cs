using System;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GunData gunData;
    [SerializeField] private Transform  muzzle;
    [SerializeField] private Camera playerCamera; 
    [SerializeField] KeyCode keyReload = KeyCode.R;
    float timeSinceLastShot;
    [SerializeField] public ParticleSystem muzzleFlash;
    [SerializeField] public GameObject gun;
    [SerializeField] public AudioClip shotFX;
    [SerializeField] public AudioSource _audioSource;
    [SerializeField] public GameObject hitEffect;
    [SerializeField] public float maxDistance = 1000f;

    
    private void Start() {
       gunData.currentAmmo = gunData.magSize;
       gunData.reloading = false;
       
    }
    public void StartReload()
    {
        if (!gunData.reloading)
        {
            StartCoroutine(Reload());        
        }
    }
    
    private IEnumerator Reload()
    {
        gunData.reloading = true;
        gun.GetComponent<Animator>().Play("reloading");
//        Debug.Log("Reloading");
        yield return new WaitForSeconds(gunData.reloadTime);
        
        gun.GetComponent<Animator>().Play("New State");
        gunData.currentAmmo = gunData.magSize;
        gunData.reloading = false;
//        Debug.Log("Reloaded");
    }
    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);
    public void Shoot()
    {
        if (gunData.currentAmmo > 0)
        {
            if (CanShoot())
            {
                

                Vector3 origin = playerCamera.transform.position;
                Vector3 dir = playerCamera.transform.forward;

                bool hasHit = Physics.Raycast(
                    origin,
                    dir,
                    out RaycastHit hitInfo,
                    gunData.maxDistance
                );

                gunData.currentAmmo--;
                timeSinceLastShot = 0;

                if (muzzleFlash != null)
                    muzzleFlash.Play();

                if (_audioSource != null && shotFX != null)
                    _audioSource.PlayOneShot(shotFX);

                StartCoroutine(startRecoil());

                if (hasHit)
                {
                    Debug.DrawRay(origin, dir * 100, Color.blue, 1f);

                    if (hitInfo.collider != null && hitInfo.transform.CompareTag("Enemy"))
                    {
                        Enemy enemy = hitInfo.transform.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            enemy.InflictDamage(gunData.damage);
                        }
                    }

                    if (hitEffect != null)
                    {
                        GameObject impact = Instantiate(
                            hitEffect,
                            hitInfo.point,
                            Quaternion.LookRotation(hitInfo.normal)
                        );
                        Destroy(impact, 1f);
                    }
                }
            }
        }
    }

    private void Update() {
        timeSinceLastShot += Time.deltaTime;
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
        if (Input.GetKeyDown(keyReload))
        {
            StartReload();
        }
    }
    
    IEnumerator startRecoil()
    {
        gun.GetComponent<Animator>().Play("recoil");
        yield return new WaitForSeconds(0.2f);
        gun.GetComponent<Animator>().Play("New State");
    }
}
