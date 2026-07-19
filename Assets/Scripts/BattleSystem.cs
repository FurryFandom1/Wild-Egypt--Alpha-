using System.Collections;
using UnityEngine;
using TMPro;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] public TextMeshProUGUI waveCountText;
    [SerializeField] public int waveCount = 0;
    [SerializeField] public float spawnRate = 2.0f;
    [SerializeField] public float timeBetweenWaves = 20.0f;
    [SerializeField] public GameObject enemy;
    [SerializeField] public int enemyCount;

    bool waveIsDone = true;
    void Update()
    {
//        waveCountText.text = "Wave " + waveCount.ToString();
        if (waveIsDone == true)
        {
            StartCoroutine(waveSpawner());
        }

    }


    IEnumerator waveSpawner()
    {
        waveCount += 1;
        waveIsDone = false;
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyClone = Instantiate(enemy);
            enemyClone.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

            yield return new WaitForSeconds(spawnRate);
        }

        spawnRate -= 0.7f;
        enemyCount += 3;

        yield return new WaitForSeconds(timeBetweenWaves);

        waveIsDone = true;
        
        
    }
}
