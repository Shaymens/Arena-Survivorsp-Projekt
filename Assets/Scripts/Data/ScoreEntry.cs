using System;


// Model danych — jeden wpis w tabeli wyników.
/// Serializowany do pliku tekstowego CSV.

[Serializable]
public class ScoreEntry
{
    public string playerName;
    public int score;
    public string date;

    public ScoreEntry(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
        this.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }

    // Konwertuje wpis do linii CSV.
    public string ToCsv() => $"{playerName},{score},{date}";

    // <summary>Tworzy ScoreEntry z linii CSV.
    public static ScoreEntry FromCsv(string line)
    {
        string[] parts = line.Split(',');
        if (parts.Length < 3) return null;
        return new ScoreEntry(parts[0], int.Parse(parts[1]))
        {
            date = parts[2]
        };
    }

    public override string ToString() => $"{playerName} | {score} pkt | {date}";
}
