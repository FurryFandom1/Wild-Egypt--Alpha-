using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawn : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] enemies;

    [Header("Wave Settings")]
    [SerializeField] private int waveCount = 0;
    [SerializeField] private int enemyCount = 5;
    [SerializeField] private int enemiesAddedPerWave = 5;

    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private float timeBetweenWaves = 20;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI waveCountText;
    [SerializeField] private TextMeshProUGUI waveCoolDownText;

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private static readonly WaitForSeconds oneSecond = new WaitForSeconds(1f);

    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("EnemySpawn: не указаны точки спавна!");
            enabled = false;
            return;
        }

        if (enemies == null || enemies.Length == 0)
        {
            Debug.LogError("EnemySpawn: не указаны префабы врагов!");
            enabled = false;
            return;
        }
        

        StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
        
            waveCount++;

            UpdateWaveText();

            Debug.Log(
                $"Началась волна {waveCount}. " +
                $"Количество врагов: {enemyCount}"
            );

            yield return StartCoroutine(SpawnWave());

            yield return new WaitUntil(IsWaveCleared);

            Debug.Log($"Волна {waveCount} зачищена!");

            enemyCount += enemiesAddedPerWave;

            Debug.Log(
                $"Следующая волна через {timeBetweenWaves} секунд"
            );
            
            
            if (waveCount >= 10)
            {
                timeBetweenWaves = 20;
                enemiesAddedPerWave = 10;
            }
            
            yield return StartCoroutine(WaveCoolDownTextUpdate());
        }
    }

    private IEnumerator SpawnWave()
    {
        aliveEnemies.Clear();

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject randomEnemy =
                enemies[Random.Range(0, enemies.Length)];

            Transform randomSpawnPoint =
                spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject spawnedEnemy = Instantiate(
                randomEnemy,
                randomSpawnPoint.position,
                randomSpawnPoint.rotation
            );

            aliveEnemies.Add(spawnedEnemy);

            if (i < enemyCount - 1)
            {
                yield return new WaitForSeconds(spawnRate); //КД между спавном
            }
        }
    }

        private IEnumerator WaveCoolDownTextUpdate()
    {
        for (int timer = (int)timeBetweenWaves; timer > 0; timer--)
        {
            waveCoolDownText.SetText("{0}", timer);

            yield return oneSecond;
        }

        waveCoolDownText.SetText("");
    }
    private bool IsWaveCleared()
    {
        aliveEnemies.RemoveAll(
            enemy => enemy == null || !enemy.activeInHierarchy
        );
        return aliveEnemies.Count == 0;
    }

    private void UpdateWaveText()
    {
        if (waveCountText != null)
        {
            waveCountText.text = "Wave " + waveCount;
        }
    }
}