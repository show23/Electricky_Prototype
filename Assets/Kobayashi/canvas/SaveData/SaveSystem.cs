using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class rankData
{
    public int score = 0;
    public float time = 0.0f;
}


[System.Serializable]
public class SaveDataRanking
{   
    public rankData[] rankDatas = new rankData[5];
}

[System.Serializable]
public class SaveDataTime
{
    public float[] time = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
}

public class SaveSystem : MonoBehaviour
{
    private const string folderName = "SaveDataFile";

    private rankData[] bestData = new rankData[5];
    public rankData[] BestData 
    { 
        get { return bestData; }
    }

    private rankData[] nowData = new rankData[5];
    public rankData[] NowData
    {  
        get { return nowData; }
        set { nowData = value; }
    }

    private rankData rankDataa = new rankData();

    public void CreateScore(float time)
    {
        rankDataa.time = time;
        rankDataa.score = (int)time * 100;
    }

    public void IsUpdateData(rankData data)
    {
        for(int i = 0; i < bestData.Length; i++) 
        { 
            if (bestData[i].time <= data.time) 
            {
                for (int j = nowData.Length; j > nowData.Length - i; j--)
                {
                    nowData[j] = bestData[i-1];
                }

                nowData[i] = data;
                break;
            }
        }
        
    }

    public void Save()
    {
        Load();



        //var jsonData = JsonUtility.ToJson(data);
    }

    public void Load()
    {
        string path = Application.persistentDataPath + folderName + ".json";

        if(File.Exists(path)) 
        { 
            string json = File.ReadAllText(path);
            SaveDataRanking data = JsonUtility.FromJson<SaveDataRanking>(json);
            bestData = data.rankDatas;
            nowData = data.rankDatas;
        }
        else
        {
            for(int i = 0; i < bestData.Length; i++) 
            {
                bestData[i].score = 0;
                bestData[i].time = 0.0f;
                nowData[i].score = 0;
                nowData[i].time = 0.0f;
            }
        }
    }

    // fatarity
    //score nannte iranai
    private const string folderTimeName = "SaveDataTimeFile";

    private float[] bestTime = new float[5];
    public float[] BestTime 
    { 
        get { return bestTime; }
        set { bestTime = value; }
    }

    private float[] nowTime = new float[5];
    public float[] NowTime
    { 
        get { return nowTime; } 
        set {  nowTime = value; } 
    }

    private void IsUpdateDataTime(float time)
    {
        for (int i = 0; i < BestTime.Length; i++)
        {
            if (BestTime[i] <= time)
            {
                for (int j = nowData.Length; j > nowData.Length - i; j--)
                {
                    nowTime[j] = bestTime[i - 1];
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
    }

    public void LoadTime()
    {
        string path = Application.persistentDataPath + folderTimeName + ".json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveDataTime dataTime = JsonUtility.FromJson<SaveDataTime>(json);
            bestTime = dataTime.time;
            nowTime = dataTime.time;
        }
        else
        {
            for(int i = 0;i < BestTime.Length;i++)
            {
                bestTime[i] = 0.0f;
                nowTime[i] = 0.0f;
            }
        }
    }
    
}
