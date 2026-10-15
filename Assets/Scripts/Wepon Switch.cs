using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject AK; 
    public GameObject pistol;       

    void Start()
    {
        SwitchWeapon(0);
    }

    void Update()
    {
        switch (true)
        {
            case bool _ when Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1):
                SwitchWeapon(0); 
                break;

            case bool _ when Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2):
                SwitchWeapon(1); 
                break;
        }
    }

    void SwitchWeapon(int weaponIndex)
    {
        AK.SetActive(weaponIndex == 0);
        pistol.SetActive(weaponIndex == 1);
    }
}
