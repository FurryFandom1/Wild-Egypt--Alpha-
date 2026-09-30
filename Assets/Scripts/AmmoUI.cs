using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _ammoText;
    [SerializeField] private Transform weaponsParent;
    [SerializeField] private Image icon;

    private GunData _gunData;

    private void Awake()
    {
        if (_ammoText == null)
        {
            _ammoText = GetComponent<TMP_Text>();
        }
    }

    private void Update()
    {
        if (weaponsParent == null)
            return;

        Gun gun = weaponsParent.GetComponentInChildren<Gun>();

        if (gun == null || gun.gunData == null)
            return;

        _gunData = gun.gunData;

        if (icon != null)
        {
            icon.sprite = _gunData.gunIcon;
        }

        if (_ammoText != null)
        {
            _ammoText.text = $"{_gunData.currentAmmo} / {_gunData.magSize}";
        }
    }
}