using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveDataRanking
{
    public int[] best = { 0, 0, 0, 0, 0 };
}

public class SaveSystem : MonoBehaviour
{
    private const string folderName = "SaveDataFile";

    private int[] bestData = new int[5];

    public void Save()
    {


        //var jsonData = JsonUtility.ToJson(data);
    }

    public void Load()
    {
        string path = Application.persistentDataPath + folderName + ".json";

        if(File.Exists(path)) 
        { 
            string json = File.ReadAllText(path);
            SaveDataRanking data = JsonUtility.FromJson<SaveDataRanking>(json);
            bestData = data.best;
        }
    }
}
