using System;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GunData gunData;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Camera playerCamera;
    [SerializeField] KeyCode keyReload = KeyCode.R;
    float timeSinceLastShot;
    [SerializeField] public ParticleSystem muzzleFlash;
    [SerializeField] public GameObject gun;
    [SerializeField] public AudioClip shotFX;
    [SerializeField] public AudioSource _audioSource;
    [SerializeField] public GameObject hitEffect;
    [SerializeField] public float maxDistance = 1000f;

    private Vector3 currentRecoil = Vector3.zero;
    private Vector3 targetRecoil = Vector3.zero;

    private void Start()
    {
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
        yield return new WaitForSeconds(gunData.reloadTime);

        gun.GetComponent<Animator>().Play("New State");
        gunData.currentAmmo = gunData.magSize;
        gunData.reloading = false;
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

                int layerMask = ~LayerMask.GetMask("Player");

                bool hasHit = Physics.Raycast(
                    origin,
                    dir,
                    out RaycastHit hitInfo,
                    gunData.maxDistance,
                    layerMask
                );

                gunData.currentAmmo--;
                timeSinceLastShot = 0;

                if (muzzleFlash != null)
                    muzzleFlash.Play();

                if (_audioSource != null && shotFX != null)
                    _audioSource.PlayOneShot(shotFX);

                StartCoroutine(startRecoil());
                ApplyRecoil();

                if (hasHit)
                {
                    EnemyAI enemy = hitInfo.transform.GetComponent<EnemyAI>();
                    if (enemy == null)
                    {
                        enemy = hitInfo.transform.GetComponentInParent<EnemyAI>();
                    }

                    if (enemy != null)
                    {
                        enemy.TakeDamage(gunData.damage);
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

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
        if (Input.GetKeyDown(keyReload))
        {
            StartReload();
        }

        UpdateRecoil();
    }

    IEnumerator startRecoil()
    {
        gun.GetComponent<Animator>().Play("recoil");
        yield return new WaitForSeconds(0.2f);
        gun.GetComponent<Animator>().Play("New State");
    }

    void ApplyRecoil()
    {
        float vertical = UnityEngine.Random.Range(-gunData.recoilVertical * 0.5f, gunData.recoilVertical);
        float horizontal = UnityEngine.Random.Range(-gunData.recoilHorizontal, gunData.recoilHorizontal);

        targetRecoil += new Vector3(-vertical, horizontal, 0);
        targetRecoil.x = Mathf.Clamp(targetRecoil.x, -gunData.maxRecoil, 0);
        targetRecoil.y = Mathf.Clamp(targetRecoil.y, -gunData.maxRecoil, gunData.maxRecoil);
    }

    void UpdateRecoil()
    {
        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, gunData.recoilSpeed * Time.deltaTime);
        currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, gunData.recoilSpeed * Time.deltaTime);

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(currentRecoil);
        }
    }
}