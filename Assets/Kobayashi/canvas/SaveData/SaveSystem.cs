using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.CompilerServices;



[System.Serializable]
public class SaveDataTime
{
    public float[] time = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
}

public class SaveSystem : MonoBehaviour
{
    private const string folderName = "SaveDataFile";

    public class rankData
    {
        public int minute = 99;
        public float seconds = 59.9f;
        public rankData()
        {
            minute = 99;
            seconds = 59.9f;
        }
    }

    [System.Serializable]
    public class SaveDataRanking
    {
        public rankData[] rankDatas = new rankData[5];
        public SaveDataRanking() 
        { 
            for(int i =0; i < rankDatas.Length; i++) 
            {
                rankDatas[i] = new rankData();
            }
        }
    }

    private rankData[] bestData = new rankData[5];
    public rankData[] BestData 
    { 
        get { return bestData; }
    }

    private rankData[] nowData = new rankData[5];

    private void Start()
    {
        for (int i = 0; i < bestData.Length; i++)
        {
            bestData[i] = new rankData();
            nowData[i] = new rankData();
        }
    }

    private void IsUpdateData(rankData data)
    {

        for(int i = 0;  i < bestData.Length; i++) 
        {
            if (bestData[i].minute > data.minute) 
            { 
                for (int j = bestData.Length - 1; j > i; j--) 
                {
                    Debug.Log(i);
                    Debug.Log(j);
                    Debug.Log(bestData[j - 1].minute);
                    nowData[j].minute = bestData[j-1].minute;
                    nowData[j].seconds = bestData[j-1].seconds;
                }
                nowData[i].minute = data.minute;
                nowData[i].seconds = data.seconds;
                return;
            }
            else if(bestData[i].minute == data.minute)
            {
                if(IsUpdateDataSeconds(data.seconds))
                {
                    return;
                }
            }
        }
    }

    private bool IsUpdateDataSeconds(float second)
    {
        for(int i = 0; i < bestData.Length; i++)
        {
            if(bestData[i].seconds > second)
            {
                for (int j = bestData.Length - 1; j > i; j--)
                {
                    nowData[j].minute = bestData[j - 1].minute;
                    nowData[j].seconds = bestData[j - 1].seconds;
                }
                nowData[i].seconds = second;
                return true;
            }
        }

        return false;
    }

    public void Save(int minute, float second)
    {
        Load();

        rankData d = new rankData();
        d.minute = minute;
        d.seconds = second;

        IsUpdateData(d);

        SaveDataRanking dataTime = new SaveDataRanking();
        for(int i = 0; i < nowData.Length; i++) 
        {
            dataTime.rankDatas[i] = nowData[i];
        }

        string json = JsonUtility.ToJson(dataTime);

        File.WriteAllText(Application.persistentDataPath + "/" + folderTimeName + ".json", json);
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/" + folderName + ".json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveDataRanking dataTime = JsonUtility.FromJson<SaveDataRanking>(json);
            bestData = dataTime.rankDatas;
            nowData = dataTime.rankDatas;
        }
        else
        {
            for (int i = 0; i < bestTime.Length; i++)
            {
                bestData[i].minute = 99;
                bestData[i].seconds = 59.9f;
                nowData[i].minute = 99;
                nowData[i].seconds = 59.9f;
            }
        }
    }

//----------------------------------------------------------------------------------------------
// fatality
//score nannte iranai
    private const string folderTimeName = "SaveDataTimeFile";

    private float[] bestTime = new float[5];
    public float[] BestTime 
    { 
        get { return bestTime; }
    }

    private float[] nowTime = new float[5];

    private void IsUpdateDataTime(float time)
    {
        for (int i = 0; i < bestTime.Length; i++)
        {
            if (bestTime[i] <= time)
            {
                for (int j = nowTime.Length; j > i; j--)
                {
                    nowTime[j] = bestTime[j - 1];
                }

                nowTime[i] = time;
                break;
            }
        }
    }

    public void SaveTime(float time)
    {
        LoadTime();

        IsUpdateDataTime(time);

        //
        SaveDataTime data = new SaveDataTime();
        for(int i = 0;i < nowTime.Length;i++)
        {
            data.time[i] = nowTime[i];
        }

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.streamingAssetsPath + "/" + folderTimeName + ".json", json);
    }

    public void LoadTime()
    {
        string path = Application.persistentDataPath + "/" + folderTimeName + ".json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveDataTime dataTime = JsonUtility.FromJson<SaveDataTime>(json);
            bestTime = dataTime.time;
            nowTime = dataTime.time;
        }
        else
        {
            for(int i = 0;i < bestTime.Length;i++)
            {
                bestTime[i] = 0.0f;
                nowTime[i] = 0.0f;
            }
        }
    }

    public void ResetSaveDataTime()
    {
        SaveDataTime data = new SaveDataTime();
        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.streamingAssetsPath + "/" + folderTimeName + ".json", json);
    }
}
