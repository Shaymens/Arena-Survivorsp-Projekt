using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TextMeshProUGUI scoresText;

    private void Start()
    {
        LoadScores();
    }

    private void LoadScores()
    {
        if (scoresText == null) return;

        var scores = DataManager.Instance?.LoadAllScores();
        if (scores == null || scores.Count == 0)
        {
            scoresText.text = "=== TOP WYNIKI ===\nBrak wyników";
            return;
        }

        scores.Sort((a, b) => b.score.CompareTo(a.score));
        string board = "=== TOP WYNIKI ===\n";
        int top = Mathf.Min(5, scores.Count);
        for (int i = 0; i < top; i++)
            board += $"{i + 1}. {scores[i]}\n";
        scoresText.text = board;
    }

    public void PlayGame()
    {
        string playerName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(playerName))
            playerName = "Player1";
        DataManager.Instance?.SetSetting("playerName", playerName);
        SceneManager.LoadScene(1);
    }

    public void ResetScores()
    {
        DataManager.Instance?.DeleteAllScores();
        LoadScores();
        Debug.Log("[MainMenu] Wyniki zresetowane!");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}