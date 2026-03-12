using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Główny menedżer gry — zarządza falami wrogów, stanem gry, Game Over.
/// Singleton.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Spawning")]
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public int currentWave = 0;
    public int enemiesPerWave = 3;

    private int enemiesAlive = 0;
    private bool gameRunning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // Wczytaj nazwę gracza z ustawień
        string savedName = DataManager.Instance?.GetSetting("playerName", "Player1");
        Player player = FindFirstObjectByType<Player>();
        if (player != null) player.playerName = savedName;

        StartGame();
    }

    public void StartGame()
    {
        gameRunning = true;
        currentWave = 0;
        SpawnWave();
    }

    private void SpawnWave()
    {
        currentWave++;
        Debug.Log($"[GameManager] Fala {currentWave} rozpoczęta!");
        UIManager.Instance?.ShowWaveInfo(currentWave);

        bool isBossWave = currentWave % 3 == 0;
        int count = isBossWave ? 1 : enemiesPerWave + (currentWave - 1);

        enemiesAlive = count;

        Player player = FindFirstObjectByType<Player>();

        for (int i = 0; i < count; i++)
        {
            // Spawnuj wokół gracza w losowym miejscu
            Vector2 randomOffset = Random.insideUnitCircle.normalized * 10f;
            Vector3 spawnPos = player != null
                ? player.transform.position + new Vector3(randomOffset.x, randomOffset.y, 0)
                : spawnPoints[i % spawnPoints.Length].position;

            GameObject prefab = (isBossWave && i == 0) ? bossPrefab : enemyPrefab;

            if (prefab != null)
                Instantiate(prefab, spawnPos, Quaternion.identity);
            else
                Debug.LogWarning("[GameManager] Brak przypisanego prefabu wroga!");
        }
    }

    /// <summary>Wywoływana kiedy wróg (nie-boss) zginie.</summary>
    public void OnEnemyDefeated()
    {
        Debug.Log("[GameManager] OnEnemyDefeated wywołane!");
        enemiesAlive--;
        Debug.Log($"[GameManager] Wrogów pozostało: {enemiesAlive}");
        if (enemiesAlive <= 0 && gameRunning)
            SpawnWave();
    }

    /// <summary>Wywoływana kiedy boss zginie.</summary>
    public void OnBossDefeated()
    {
        enemiesAlive--;
        if (enemiesAlive <= 0 && gameRunning)
            SpawnWave();
    }

    public void GameOver()
    {
        if (!gameRunning) return;
        gameRunning = false;
        Time.timeScale = 0f;  // Zatrzymaj grę
        Debug.Log("[GameManager] GAME OVER");
        UIManager.Instance?.ShowGameOver();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;  // Wznów grę
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
