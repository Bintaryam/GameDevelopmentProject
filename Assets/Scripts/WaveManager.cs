using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int totalWaves          = 5;
    public int baseEnemiesPerWave  = 3;
    public int enemiesAddedPerWave = 2;
    public float timeBetweenWaves  = 5f;

    public UnityEvent OnAllWavesComplete;

    private int currentWave = 0;
    private int activeEnemies = 0;

    void Start()
    {
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        yield return new WaitForSeconds(2f);

        for (int wave = 1; wave <= totalWaves; wave++)
        {
            currentWave = wave;
            int enemyCount = baseEnemiesPerWave + (wave - 1) * enemiesAddedPerWave;
            Debug.Log($"[WaveManager] Starting Wave {wave}/{totalWaves} — {enemyCount} enemies");

            SpawnWave(enemyCount);

            yield return new WaitUntil(() => activeEnemies <= 0);
            Debug.Log($"[WaveManager] Wave {wave} cleared!");

            if (wave < totalWaves)
                yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("[WaveManager] All waves complete!");
        OnAllWavesComplete?.Invoke();
    }

    private void SpawnWave(int count)
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveManager] enemyPrefab or spawnPoints not set!");
            return;
        }

        activeEnemies = count;
        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.OnEnemyDied += OnEnemyDied;
            }
        }
    }

    private void OnEnemyDied()
    {
        activeEnemies = Mathf.Max(0, activeEnemies - 1);
    }
}
