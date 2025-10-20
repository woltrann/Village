using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class EnemyWave
{
    public GameObject enemyPrefab;
    public float startTime = 0f;
    public int enemyCount = 5;
    public float spawnInterval = 1.5f;
}

[System.Serializable]
public class EnemyWaveSet
{
    public int level = 1;
    public List<EnemyWave> enemies;
}

public class EnemyWaveManager : MonoBehaviour
{
    public static EnemyWaveManager Instance;

    [Header("Wave Settings")]
    public List<EnemyWaveSet> levelWaves;
    public int currentLevelforEnemy = 1;
    public int waveIndex = 1;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI waveCountText;
    public TextMeshProUGUI starCountText;

    [Header("Spawn Settings")]
    public List<Transform> spawnPoints;
    public float spawnRadius = 3f;

    private bool isSpawning = false;
    private int activeEnemies = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void StartGame()
    {
        StartEnemyWaves(currentLevelforEnemy);
        waveText.text = "Dalga: " + waveIndex + "/50";    
        DayNightCycle.Instance.ToggleDayNight();
    }

    public void StartEnemyWaves(int level)
    {
        if (isSpawning) return;
        StartCoroutine(StartWaveSequence(level));
    }

    private IEnumerator StartWaveSequence(int level)
    {
        isSpawning = true;

        EnemyWaveSet selectedWaveSet = levelWaves.Find(w => w.level == level);
        if (selectedWaveSet == null)
        {
            Debug.LogWarning($"❌ Level {level} için düşman dalgası bulunamadı!");
            isSpawning = false;
            yield break;
        }

        Debug.Log($"⚔️ Level {level} için {selectedWaveSet.enemies.Count} grup başlatılıyor...");

        int totalEnemies = 0;
        foreach (var wave in selectedWaveSet.enemies)
            totalEnemies += wave.enemyCount;

        activeEnemies = totalEnemies;

        foreach (EnemyWave wave in selectedWaveSet.enemies)
        {
            StartCoroutine(SpawnEnemies(wave));
        }

        yield return new WaitUntil(() => activeEnemies <= 0);

        Debug.Log("✅ Dalga tamamlandı!");
        OnWaveCompleted(level);

        isSpawning = false;
    }

    private IEnumerator SpawnEnemies(EnemyWave wave)
    {
        yield return new WaitForSeconds(wave.startTime);

        // 🔥 Grup için rastgele tek bir spawn noktası seç
        Transform groupSpawn = spawnPoints[Random.Range(0, spawnPoints.Count)];

        for (int i = 0; i < wave.enemyCount; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));
            GameObject enemyObj = Instantiate(wave.enemyPrefab, groupSpawn.position + offset, Quaternion.identity);

            EnemyManager enemy = enemyObj.GetComponent<EnemyManager>();
            if (enemy != null)
                enemy.OnEnemyDied += HandleEnemyDeath; // ✅ Event dinle

            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    private void HandleEnemyDeath(EnemyManager enemy)
    {
        activeEnemies--;
        if (activeEnemies < 0) activeEnemies = 0;

        Debug.Log($"☠️ Düşman öldü. Kalan: {activeEnemies}");
    }

    private void OnWaveCompleted(int level)
    {
        Debug.Log($"🌅 Level {level} dalgası tamamen bitti!");

        waveIndex++;
        waveText.text = "Dalga: " + waveIndex + "/50";
        waveCountText.text = "Tamamlanan Dalgalar: " + (waveIndex - 1);
        starCountText.text = "Toplanan Yıldızlar: " + (waveIndex - 1);
        currentLevelforEnemy++;
        DayNightCycle.Instance.ToggleDayNight();
    }
    public void AfterDie()
    {
        waveIndex = 1;
        currentLevelforEnemy = 1;
        PlayerStats.Instance.gold = 0;
        PlayerStats.Instance.endPanel.SetActive(false);
        GameManager.Instance.GamePanel.SetActive(false);
        GameManager.Instance.MenuPanel.SetActive(true);
    }
}
