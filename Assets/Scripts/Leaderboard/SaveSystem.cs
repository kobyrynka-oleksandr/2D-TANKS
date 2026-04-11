using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor.Overlays;
using UnityEngine;

public static class SaveSystem
{
    private static string Path => Application.persistentDataPath + "/players.save";

    public static void SavePlayer(PlayerData player)
    {
        SaveData saveData = LoadAll() ?? new SaveData();

        int index = saveData.Players.FindIndex(p => p.Name == player.Name);
        if (index >= 0) saveData.Players[index] = player;
        else saveData.Players.Add(player);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(Path, FileMode.Create);
        formatter.Serialize(stream, saveData);
        stream.Close();
    }

    public static PlayerData LoadPlayer(string name)
    {
        SaveData saveData = LoadAll();
        if (saveData == null) return null;

        PlayerData data = saveData.Players.Find(p => p.Name == name);
        if (data == null) Debug.LogError($"Player '{name}' not found");
        return data;
    }

    public static SaveData LoadAll()
    {
        if (!File.Exists(Path)) return null;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(Path, FileMode.Open);
        SaveData saveData = formatter.Deserialize(stream) as SaveData;
        stream.Close();
        return saveData;
    }
    public static List<PlayerData> LoadLeaderboard()
    {
        SaveData saveData = LoadAll();
        if (saveData == null) return new List<PlayerData>();

        saveData.Players.Sort((a, b) => b.Stage.CompareTo(a.Stage));
        return saveData.Players;
    }


    public static void DeletePlayer(string name)
    {
        SaveData saveData = LoadAll();
        if (saveData == null) return;

        saveData.Players.RemoveAll(p => p.Name == name);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(Path, FileMode.Create);
        formatter.Serialize(stream, saveData);
        stream.Close();
    }
}