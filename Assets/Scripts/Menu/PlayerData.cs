using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerData : MonoBehaviour
{

    public static PlayerData Instance { get; private set; }

    public int lastPuzzleId;

    [System.Serializable]
    public class PuzzleData
    {
        public int lastPlayedPuzzleID;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {

            Destroy(gameObject);
        }
    }

    private void Start()
    {
        lastPuzzleId = LoadLastPlayedPuzzle();
    }

    public void SaveLastPlayedPuzzle(int puzzleID)
    {
        PuzzleData data = new PuzzleData { lastPlayedPuzzleID = puzzleID };
        string json = JsonUtility.ToJson(data);

        string path = Path.Combine(Application.persistentDataPath, "playerdata.json");
        File.WriteAllText(path, json);
    }

    public int LoadLastPlayedPuzzle()
    {
        string path = Path.Combine(Application.persistentDataPath, "playerdata.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PuzzleData data = JsonUtility.FromJson<PuzzleData>(json);
            return data.lastPlayedPuzzleID;
        }

        return 0;
    }
}
