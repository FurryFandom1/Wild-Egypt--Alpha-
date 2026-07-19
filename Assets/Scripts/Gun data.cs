using UnityEngine;
[CreateAssetMenu(fileName ="Gun", menuName = "Weapon/Gun")]
public class GunData : ScriptableObject
{
    [Header("Info")]
    public string Name;
    [Header("Shooting")]
    public float damage;
    public float maxDistance;
    [Header("Reloading")]
    public int currentAmmo;
    public int magSize;
    public float fireRate;
    public float reloadTime;
    [HideInInspector]
        public bool reloading;
}
