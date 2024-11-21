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
        public int hintsLeft;
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
        lastPuzzleId = LoadPuzzleData(true);
    }

    public void SavePuzzleData(int puzzleID, int hints)
    {
        PuzzleData data = new PuzzleData { lastPlayedPuzzleID = puzzleID, hintsLeft = hints};
        string json = JsonUtility.ToJson(data);

        string path = Path.Combine(Application.persistentDataPath, "playerdata.json");
        File.WriteAllText(path, json);
    }

    public int LoadPuzzleData(bool isPuzzle)
    {
        string path = Path.Combine(Application.persistentDataPath, "playerdata.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PuzzleData data = JsonUtility.FromJson<PuzzleData>(json);
            int returnValue = isPuzzle ? data.lastPlayedPuzzleID : data.hintsLeft;
            return returnValue;
        }

        return isPuzzle ? 0 : 3;
    }
}
