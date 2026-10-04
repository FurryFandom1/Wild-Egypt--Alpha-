using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("Player Money")]
    [SerializeField] private int currentMoney = 0;

    [Header("Enemy Reward")]
    [SerializeField, Range(0f, 1f)] private float enemyMoneyChance = 1f;
    [SerializeField] private int enemyMoneyReward = 5;

    [Header("UI")]
    [SerializeField] private TMP_Text CoinsTextUI;

    public int CurrentMoney => currentMoney;

    private void Awake()
    {
        Instance = this;

        UpdateCoinsUI();
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

        UpdateCoinsUI();

        Debug.Log(
            "Игрок получил " + amount +
            ". Текущий баланс: " + currentMoney
        );
    }

    private void UpdateCoinsUI()
    {
        if (CoinsTextUI != null)
        {
            CoinsTextUI.text = currentMoney.ToString();
        }
    }
}