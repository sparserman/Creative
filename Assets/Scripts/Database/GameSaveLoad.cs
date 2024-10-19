using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaveLoad : MonoBehaviour
{
    // Save
    public void GameSave()
    {
        string jsonData = JsonUtility.ToJson(GameManager.GetInstance().gi, true);
        string path = Application.dataPath + "/gameInfo.json";
        File.WriteAllText(path, jsonData);
    }


    // Load
    public void GameLoad()
    {
        string path = Path.Combine(Application.dataPath, "gameInfo.json");
        string jsonData = File.ReadAllText(path);
        GameManager.GetInstance().gi = JsonUtility.FromJson<GameInfo>(jsonData);
    }
}
