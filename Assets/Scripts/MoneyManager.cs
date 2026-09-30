using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("Player Money")]
    [SerializeField] private int currentMoney = 0;

    [Header("Enemy Reward")]
    [SerializeField, Range(0f, 1f)]
    private float enemyMoneyChance = 1f;

    [SerializeField]
    private int enemyMoneyReward = 5;

    public int CurrentMoney => currentMoney;

    private void Awake()
    {
        Instance = this;

        Debug.Log("MoneyManager запущен");
    }

    public void RewardPlayerForEnemyKill()
{
    if (Random.value <= enemyMoneyChance)
    {
        AddMoneyToPlayer(enemyMoneyReward);
    }
}
    public void AddMoneyToPlayer(int amount)
    {
        currentMoney += amount;

        Debug.Log(
            "Игрок получил " + amount +
            ". Текущий баланс: " + currentMoney
        );
    }
}