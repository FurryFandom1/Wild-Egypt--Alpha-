using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject assaultRifle;
    public GameObject pistol;

    void Start()
    {
        SwitchToWeapon(0);
    }

    void Update()
    {
        GameObject activeWeapon = assaultRifle.activeSelf ? assaultRifle : pistol;
        Gun activeGunScript = activeWeapon.GetComponentInChildren<Gun>();

        switch (true)
        {
            case bool _ when activeGunScript != null && activeGunScript.IsReloading:
                return;
        }

        switch (true)
        {
            case bool _ when Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1):
                SwitchToWeapon(0);
                break;

            case bool _ when Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2):
                SwitchToWeapon(1);
                break;
        }
    }

    void SwitchToWeapon(int weaponIndex)
    {
        assaultRifle.SetActive(weaponIndex == 0);
        pistol.SetActive(weaponIndex == 1);
    }
}