
using UnityEngine;
using UnityEngine.UI;

public class WeaponIconUI : MonoBehaviour
{
    [SerializeField] private Transform weaponsParent;

    private Image icon;

    void Start()
    {
        icon = GetComponent<Image>();
    }

    void Update()
    {
        for (int i = 0; i < weaponsParent.childCount; i++)
        {
            Transform weapon = weaponsParent.GetChild(i);

            if (weapon.gameObject.activeSelf)
            {
                Gun gun = weapon.GetComponentInChildren<Gun>(true);

                if (gun != null && gun.gunData != null)
                {
                    if (icon.sprite != gun.gunData.gunIcon)
                        icon.sprite = gun.gunData.gunIcon;
                }

                break;
            }
        }
    }
}
