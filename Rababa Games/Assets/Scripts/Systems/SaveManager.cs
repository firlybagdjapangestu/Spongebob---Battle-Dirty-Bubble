using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static readonly string filePath = Path.Combine(Application.persistentDataPath, "savegame.json");

    public static void Save(GameSaveData newData)
    {
        GameSaveData existingData = Load(); // Ambil data lama dulu

        bool shouldSave = false;

        if (newData.player1Score > existingData.player1Score)
        {
            existingData.player1Score = newData.player1Score;
            shouldSave = true;
        }

        if (newData.player2Score > existingData.player2Score)
        {
            existingData.player2Score = newData.player2Score;
            shouldSave = true;
        }

        if (shouldSave)
        {
            string json = JsonUtility.ToJson(existingData, true);
            File.WriteAllText(filePath, json);
            Debug.Log("[Save] Score updated and saved.");
        }
        else
        {
            Debug.Log("[Save] Skor tidak lebih tinggi, tidak disimpan.");
        }
    }


    public static GameSaveData Load()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("[Save] No save file found, returning default data.");
            return new GameSaveData();
        }

        string json = File.ReadAllText(filePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
        Debug.Log($"[Save] Data loaded from {filePath}");
        return data;
    }

    public static void DeleteSave()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("[Save] Save file deleted.");
        }
    }
}
