using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject _primaryWeapons; 
    public GameObject _secondaryWeapons;       

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
        _primaryWeapons.SetActive(weaponIndex == 0);
        _secondaryWeapons.SetActive(weaponIndex == 1);
    }
}
