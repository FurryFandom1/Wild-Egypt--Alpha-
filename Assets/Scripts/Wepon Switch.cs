using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject assaultRifle;
    public GameObject pistol;

    [Header("Стартовое оружие (True = Пистолет, False = Автомат)")]
    public bool startWithPistol = false;

    void Start()
    {
        switch (true)
        {
            case bool _ when assaultRifle != null:
                assaultRifle.SetActive(false);
                break;
        }

        switch (true)
        {
            case bool _ when pistol != null:
                pistol.SetActive(false);
                break;
        }

        switch (true)
        {
            case bool _ when startWithPistol && pistol != null:
                pistol.SetActive(true);
                break;

            case bool _ when !startWithPistol && assaultRifle != null:
                assaultRifle.SetActive(true);
                break;
        }
    }

    void Update()
    {
        GameObject currentActive = null;

        switch (true)
        {
            case bool _ when assaultRifle != null && assaultRifle.activeSelf:
                currentActive = assaultRifle;
                break;
            case bool _ when pistol != null && pistol.activeSelf:
                currentActive = pistol;
                break;
        }

        switch (true)
        {
            case bool _ when currentActive != null:
                Gun gunScript = currentActive.GetComponentInChildren<Gun>();
                switch (true)
                {
                    case bool _ when gunScript != null && gunScript.IsReloading:
                        return;
                }
                break;
        }

        switch (true)
        {
            case bool _ when (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) && assaultRifle != null:
                SwitchWeapon(assaultRifle, pistol);
                break;
        }

        switch (true)
        {
            case bool _ when (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) && pistol != null:
                SwitchWeapon(pistol, assaultRifle);
                break;
        }
    }

    void SwitchWeapon(GameObject weaponToEnable, GameObject weaponToDisable)
    {
        switch (true)
        {
            case bool _ when weaponToDisable != null:
                weaponToDisable.SetActive(false);
                break;
        }

        switch (true)
        {
            case bool _ when weaponToEnable != null:
                weaponToEnable.SetActive(true);
                break;
        }
    }
}