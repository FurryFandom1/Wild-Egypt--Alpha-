using System;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GunData gunData;
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
    [SerializeField] private Transform recoilPivot;

    [Header("Fire Mode Settings")]
    [SerializeField] private bool isAutomatic = false;
    [SerializeField] private KeyCode keyFireMode = KeyCode.Q;

    public bool IsReloading => gunData != null && gunData.reloading;

    private Vector3 currentRecoil = Vector3.zero;
    private Vector3 targetRecoil = Vector3.zero;

    private Vector3 startGunLocalPos;
    private Quaternion startGunLocalRot;
    private Animator cachedAnimator;

    private void Awake()
    {
        switch (true)
        {
            case bool _ when playerCamera == null:
                playerCamera = Camera.main;
                break;
        }

        switch (true)
        {
            case bool _ when gun == null:
                gun = gameObject;
                break;
        }

        gun.TryGetComponent(out cachedAnimator);

        switch (true)
        {
            case bool _ when gunData != null:
                gunData.currentAmmo = gunData.magSize;
                gunData.reloading = false;
                break;
        }

        switch (true)
        {
            case bool _ when gun != null:
                startGunLocalPos = gun.transform.localPosition;
                startGunLocalRot = gun.transform.localRotation;
                break;
        }
    }

    private void OnEnable()
    {
        StopAllCoroutines();

        switch (true)
        {
            case bool _ when gunData != null:
                gunData.reloading = false;
                break;
        }

        switch (true)
        {
            case bool _ when gun != null && cachedAnimator != null:
                cachedAnimator.enabled = false;
                gun.transform.localPosition = startGunLocalPos;
                gun.transform.localRotation = startGunLocalRot;
                cachedAnimator.enabled = true;
                cachedAnimator.Rebind();
                cachedAnimator.Update(0f);
                cachedAnimator.Play("New State", 0, 0f);
                break;
        }

        currentRecoil = Vector3.zero;
        targetRecoil = Vector3.zero;

        switch (true)
        {
            case bool _ when recoilPivot != null:
                recoilPivot.transform.localRotation = Quaternion.identity;
                break;
        }
    }

    public void StartReload()
    {
        switch (true)
        {
            case bool _ when gunData != null && !gunData.reloading:
                StartCoroutine(Reload());
                break;
        }
    }

    private IEnumerator Reload()
    {
        gunData.reloading = true;

        switch (true)
        {
            case bool _ when cachedAnimator != null:
                cachedAnimator.Play("reloading");
                break;
        }

        yield return new WaitForSeconds(gunData.reloadTime);

        switch (true)
        {
            case bool _ when cachedAnimator != null:
                cachedAnimator.Play("New State");
                break;
        }

        gunData.currentAmmo = gunData.magSize;
        gunData.reloading = false;
    }

    private bool CanShoot() => gunData != null && !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);

    public void Shoot()
    {
        switch (true)
        {
            case bool _ when gunData != null && gunData.currentAmmo > 0 && CanShoot():
                ExecuteShoot();
                break;
        }
    }

    private void ExecuteShoot()
    {
        switch (true)
        {
            case bool _ when playerCamera == null:
                return;
        }

        Vector3 origin = playerCamera.transform.position;
        Vector3 dir = playerCamera.transform.forward;
        int layerMask = ~LayerMask.GetMask("Player");

        bool hasHit = Physics.Raycast(origin, dir, out RaycastHit hitInfo, gunData.maxDistance, layerMask);

        gunData.currentAmmo--;
        timeSinceLastShot = 0;

        muzzleFlash?.Play();

        switch (true)
        {
            case bool _ when _audioSource != null && shotFX != null:
                _audioSource.PlayOneShot(shotFX);
                break;
        }

        StartCoroutine(startRecoil());
        ApplyRecoil();

        switch (true)
        {
            case bool _ when hasHit:
                HandleHit(hitInfo);
                break;
        }
    }

    private void HandleHit(RaycastHit hitInfo)
    {
        EnemyAI enemy = hitInfo.transform.GetComponent<EnemyAI>() ?? hitInfo.transform.GetComponentInParent<EnemyAI>();

        switch (true)
        {
            case bool _ when enemy != null:
                enemy.TakeDamage(gunData.damage);
                break;
        }

        switch (true)
        {
            case bool _ when hitEffect != null:
                GameObject impact = Instantiate(hitEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(impact, 1f);
                break;
        }
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        switch (true)
        {
            case bool _ when Input.GetKeyDown(keyFireMode):
                isAutomatic = !isAutomatic;
                break;
        }

        bool fireInput = isAutomatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

        switch (true)
        {
            case bool _ when fireInput:
                Shoot();
                break;
        }

        switch (true)
        {
            case bool _ when Input.GetKeyDown(keyReload):
                StartReload();
                break;
        }

        UpdateRecoil();
    }

    IEnumerator startRecoil()
    {
        switch (true)
        {
            case bool _ when cachedAnimator != null:
                cachedAnimator.Play("recoil");
                break;
        }

        yield return new WaitForSeconds(0.2f);

        switch (true)
        {
            case bool _ when gameObject.activeInHierarchy && cachedAnimator != null:
                cachedAnimator.Play("New State");
                break;
        }
    }

    void ApplyRecoil()
    {
        switch (true)
        {
            case bool _ when gunData == null:
                return;
        }

        float vertical = UnityEngine.Random.Range(-gunData.recoilVertical * 0.5f, gunData.recoilVertical);
        float horizontal = UnityEngine.Random.Range(-gunData.recoilHorizontal, gunData.recoilHorizontal);

        targetRecoil += new Vector3(-vertical, horizontal, 0);
        targetRecoil.x = Mathf.Clamp(targetRecoil.x, -gunData.maxRecoil, 0);
        targetRecoil.y = Mathf.Clamp(targetRecoil.y, -gunData.maxRecoil, gunData.maxRecoil);
    }

    void UpdateRecoil()
    {
        switch (true)
        {
            case bool _ when gunData == null:
                return;
        }

        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, gunData.recoilSpeed * Time.deltaTime);
        currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, gunData.recoilSpeed * Time.deltaTime);

        switch (true)
        {
            case bool _ when playerCamera != null && recoilPivot != null:
                recoilPivot.transform.localRotation = Quaternion.Euler(currentRecoil);
                break;
        }
    }
}