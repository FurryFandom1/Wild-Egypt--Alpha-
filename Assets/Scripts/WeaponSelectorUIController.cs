using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectorUIController : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private GameObject _primaryWeapons;
    [SerializeField] private GameObject _secondaryWeapons;
    
    [Header("UI images")]
    [SerializeField] private Image _primaryImage;
    [SerializeField] private Image _secondaryImage;
    
    [Header("Sprites")]
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _selectedSprite;

    void Update()
    {
        if (_primaryWeapons.activeSelf)
        {
            if (_primaryImage.sprite != _selectedSprite)
            {
                _primaryImage.sprite = _selectedSprite;
                _secondaryImage.sprite = _normalSprite;
            }
        }
        
        else if (_secondaryWeapons.activeSelf)
        {
            if (_secondaryImage.sprite != _selectedSprite)
            {
                _primaryImage.sprite = _selectedSprite;
                _secondaryImage.sprite = _normalSprite;
            }
        }
    }
}
