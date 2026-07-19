using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] public Image HealthSprite;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float currentHealth;



    void Start()
    {
        currentHealth = maxHealth;
    }
    public void DamageTaken(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Damage taken " + damage);
        HealthSprite.fillAmount = currentHealth / 100;

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(2);
        }
    }

}
