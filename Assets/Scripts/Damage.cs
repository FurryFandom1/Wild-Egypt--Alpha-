using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private float damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem enemy = other.GetComponent<HealthSystem>();
            enemy.DamageTaken(damage);
        }
    }
    
}
