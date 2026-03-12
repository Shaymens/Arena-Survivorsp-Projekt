using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;
    public Slider healthSlider;
    public TextMeshProUGUI hpText;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI leaderboardText;

    [Header("Pause Panel")]
    public GameObject pausePanel;

    [Header("Level UI")]
    public TextMeshProUGUI levelText;
    public Slider expSlider;
    public GameObject levelUpPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Wynik: {score}";
    }

    public void UpdateHealthUI(float current, float max)
    {
        if (healthSlider != null)
            healthSlider.value = current / max;
        if (hpText != null)
            hpText.text = $"HP: {(int)current}/{(int)max}";
    }

    public void ShowWaveInfo(int wave)
    {
        if (waveText != null)
            waveText.text = $"Fala: {wave}";
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Poka¿ aktualny wynik od razu
        Player player = FindFirstObjectByType<Player>();
        if (player != null && finalScoreText != null)
            finalScoreText.text = $"Twój wynik: {player.score}";

        // Poka¿ tabelê wyników
        var scores = DataManager.Instance?.LoadAllScores();
        if (scores != null && leaderboardText != null)
        {
            scores.Sort((a, b) => b.score.CompareTo(a.score));
            string board = "=== TOP WYNIKI ===\n";
            int top = Mathf.Min(5, scores.Count);
            for (int i = 0; i < top; i++)
                board += $"{i + 1}. {scores[i]}\n";
            leaderboardText.text = board;
        }
    }


    public void ShowPause()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void HidePause()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void UpdateExpUI(int current, int max, int level)
    {
        if (expSlider != null)
            expSlider.value = (float)current / max;
        if (levelText != null)
            levelText.text = $"Poziom: {level}";
    }

    public void ShowLevelUp(int level)
    {
        if (levelUpPanel != null)
            StartCoroutine(ShowLevelUpEffect(level));
    }

    private System.Collections.IEnumerator ShowLevelUpEffect(int level)
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true);
            // ZnajdŸ tekst w panelu i zaktualizuj
            TextMeshProUGUI txt = levelUpPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = $"LEVEL UP!\nPoziom {level}";
            yield return new WaitForSecondsRealtime(2f);
            levelUpPanel.SetActive(false);
        }
    }
}