using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private GunData _gunData;
    [SerializeField] private TMP_Text _ammoText;
    private int _currnetAmmo;
    private int _magSize;
    private int lastAmmo = -1;


    void Awake()
    {
        if (_ammoText == null)
        {
            _ammoText = GetComponent<TMP_Text>();
            
            UpdateAmmoText();
        }
    }
    
    private void Update()
    {
        if (_gunData == null || _ammoText == null)
            return;

        if (lastAmmo != _gunData.currentAmmo)
        {
            UpdateAmmoText();
        }
    }
    private void UpdateAmmoText()
    {
        _currnetAmmo = _gunData.currentAmmo;
        _magSize = _gunData.magSize;
        _ammoText.text = $"{_currnetAmmo} / {_magSize}";
    }

}
