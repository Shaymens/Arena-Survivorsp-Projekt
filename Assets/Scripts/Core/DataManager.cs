using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


// Menedżer danych — CRUD na plikach tekstowych (wyniki i ustawienia).


public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    private string scoresFilePath;
    private string settingsFilePath;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        scoresFilePath = Path.Combine(Application.persistentDataPath, "scores.txt");
        settingsFilePath = Path.Combine(Application.persistentDataPath, "settings.txt");

        Debug.Log($"[DataManager] Ścieżka danych: {Application.persistentDataPath}");
        EnsureFilesExist();
    }

    private void EnsureFilesExist()
    {
        if (!File.Exists(scoresFilePath))
            File.WriteAllText(scoresFilePath, "");
        if (!File.Exists(settingsFilePath))
            File.WriteAllText(settingsFilePath, "playerName=Player1\nvolume=1.0");
    }

    // ──────────────────────────────────────────
    // CRUD — WYNIKI (scores.txt)
    // ──────────────────────────────────────────

    // CREATE — Dodaje nowy wynik.
    public void SaveScore(ScoreEntry entry)
    {
        try
        {
            List<ScoreEntry> scores = LoadAllScores();

            // Dodaj nowy wynik
            scores.Add(entry);

            // Sortuj od najwyższego
            scores.Sort((a, b) => b.score.CompareTo(a.score));

            // Zachowaj tylko top 5
            if (scores.Count > 5)
                scores.RemoveRange(5, scores.Count - 5);

            WriteAllScores(scores);
            Debug.Log($"[DataManager] Zapisano wynik: {entry}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] Błąd zapisu: {e.Message}");
        }
    }

    // READ — Wczytuje wszystkie wyniki z pliku.
    public List<ScoreEntry> LoadAllScores()
    {
        var scores = new List<ScoreEntry>();
        if (!File.Exists(scoresFilePath)) return scores;

        try
        {
            string[] lines = File.ReadAllLines(scoresFilePath);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                ScoreEntry entry = ScoreEntry.FromCsv(line);
                if (entry != null) scores.Add(entry);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] Błąd odczytu: {e.Message}");
        }

        return scores;
    }

    // UPDATE — Aktualizuje wynik gracza (nadpisuje najlepszy).
    public void UpdateBestScore(string playerName, int newScore)
    {
        List<ScoreEntry> scores = LoadAllScores();

        // Znajdź czy gracz już istnieje
        ScoreEntry existing = scores.Find(s => s.playerName == playerName);

        if (existing != null)
        {
            // Aktualizuj tylko jeśli nowy wynik jest wyższy
            if (newScore > existing.score)
            {
                scores.Remove(existing);
                scores.Add(new ScoreEntry(playerName, newScore));
                Debug.Log($"[DataManager] Zaktualizowano rekord: {playerName} -> {newScore}");
            }
        }
        else
        {
            // Dodaj nowy rekord
            scores.Add(new ScoreEntry(playerName, newScore));
            Debug.Log($"[DataManager] Nowy rekord: {playerName} -> {newScore}");
        }

        WriteAllScores(scores);
    }

    // DELETE — Usuwa wszystkie wyniki danego gracza.
    public void DeleteScoresByPlayer(string playerName)
    {
        List<ScoreEntry> scores = LoadAllScores();
        scores.RemoveAll(s => s.playerName == playerName);
        WriteAllScores(scores);
        Debug.Log($"[DataManager] Usunięto wyniki gracza: {playerName}");
    }

    private void WriteAllScores(List<ScoreEntry> scores)
    {
        try
        {
            List<string> lines = new List<string>();
            foreach (var s in scores)
                lines.Add(s.ToCsv());
            File.WriteAllLines(scoresFilePath, lines);
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] Błąd zapisu wyników: {e.Message}");
        }
    }

    // ──────────────────────────────────────────
    // CRUD — USTAWIENIA (settings.txt)
    // ──────────────────────────────────────────

    // READ — Wczytuje ustawienie po kluczu.
    public string GetSetting(string key, string defaultValue = "")
    {
        if (!File.Exists(settingsFilePath)) return defaultValue;
        foreach (string line in File.ReadAllLines(settingsFilePath))
        {
            string[] parts = line.Split('=');
            if (parts.Length == 2 && parts[0].Trim() == key)
                return parts[1].Trim();
        }
        return defaultValue;
    }

    // CREATE/UPDATE — Zapisuje ustawienie.
    public void SetSetting(string key, string value)
    {
        var lines = File.Exists(settingsFilePath)
            ? new List<string>(File.ReadAllLines(settingsFilePath))
            : new List<string>();

        bool found = false;
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].StartsWith(key + "="))
            {
                lines[i] = $"{key}={value}";
                found = true;
                break;
            }
        }
        if (!found) lines.Add($"{key}={value}");

        File.WriteAllLines(settingsFilePath, lines);
        Debug.Log($"[DataManager] Ustawienie zapisane: {key}={value}");
    }

    // DELETE — Usuwa ustawienie.
    public void DeleteSetting(string key)
    {
        if (!File.Exists(settingsFilePath)) return;
        var lines = new List<string>(File.ReadAllLines(settingsFilePath));
        lines.RemoveAll(l => l.StartsWith(key + "="));
        File.WriteAllLines(settingsFilePath, lines);
    }

    // DELETE — Usuwa wszystkie wyniki.
    public void DeleteAllScores()
    {
        try
        {
            File.WriteAllText(scoresFilePath, "");
            Debug.Log("[DataManager] Wszystkie wyniki usunięte!");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] Błąd usuwania: {e.Message}");
        }
    }
}
