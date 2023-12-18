using UnityEngine;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    private const string folderName = "SaveDataFile";

    [System.Serializable]
    public class rankData
    {
        public int minute;
        public float seconds;
    }

    [System.Serializable]
    public class SaveDataRanking
    {
        public rankData[] rankDatas;

        public rankData thisTimeData;
        public int thisBreak;
    }


    private rankData[] bestData = new rankData[5];
    public rankData[] BestData 
    { 
        get { return bestData; }
    }

    private rankData[] nowData = new rankData[5];

    private rankData thisData;
    public rankData ThisData
    { get { return thisData; } }

    private int breakEnemy = 0;
    public int BreakEnemy 
    {
        get { return breakEnemy; }
    }

    private SaveDataRanking dataTime;
    

    private void Start()
    {
        for (int i = 0; i < bestData.Length; i++)
        {
            bestData[i] = new rankData();
            bestData[i].seconds = 59.99f;
            bestData[i].minute = 99;
            nowData[i] = new rankData();
            nowData[i].seconds = 59.99f;
            nowData[i].minute = 99;
        }

        dataTime = new SaveDataRanking();
        dataTime.rankDatas = new rankData[5];
        for(int i = 0;i < bestData.Length;i++)
        {
            dataTime.rankDatas[i] = new rankData();
            dataTime.rankDatas[i].seconds = 59.99f;
            dataTime.rankDatas[i].minute = 99;
        }

        dataTime.thisTimeData = new rankData();
        dataTime.thisTimeData.seconds = 59.99f;
        dataTime.thisTimeData.minute = 99;
        dataTime.thisBreak = 0;

        thisData = new rankData();
        
    }

    private void IsUpdateData(rankData data)
    {
        for(int i = 0;  i < bestData.Length; i++) 
        {
            if (bestData[i].minute > data.minute) 
            {
                for (int j = bestData.Length - 1; j > i; j--) 
                {
                    nowData[j].minute = bestData[j-1].minute;
                    nowData[j].seconds = bestData[j-1].seconds;
                }
                nowData[i].minute = data.minute;
                nowData[i].seconds = data.seconds;
                return;
            }
            else if(bestData[i].minute == data.minute)
            {
                if(IsUpdateDataSeconds(data))
                {
                    return;
                }
            }
        }
    }

    private bool IsUpdateDataSeconds(rankData data)
    {
        for(int i = 0; i < bestData.Length; i++)
        {
            if(bestData[i].seconds > data.seconds)
            {
                for (int j = bestData.Length - 1; j > i; j--)
                {
                    nowData[j].minute = bestData[j - 1].minute;
                    nowData[j].seconds = bestData[j - 1].seconds;
                }
                nowData[i].minute = data.minute;
                nowData[i].seconds = data.seconds;
                return true;
            }
        }
        return false;
    }

    public void Save(int minute, float second, int enemyBreak)
    {
        Load();

        rankData d = new rankData();
        d.minute = minute;
        d.seconds = second;

        IsUpdateData(d);

        for(int i = 0; i < nowData.Length; i++) 
        {
            dataTime.rankDatas[i] = nowData[i];
        }

        dataTime.thisTimeData.minute = minute;
        dataTime.thisTimeData.seconds = second;
        dataTime.thisBreak = enemyBreak;

        string json = JsonUtility.ToJson(dataTime);

        File.WriteAllText(Application.persistentDataPath + "/" + folderName + ".json", json);
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/" + folderName + ".json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            dataTime = JsonUtility.FromJson<SaveDataRanking>(json);
            bestData = dataTime.rankDatas;
            nowData = dataTime.rankDatas;

            thisData.minute = 
                dataTime.thisTimeData.minute;
            thisData.seconds = dataTime.thisTimeData.seconds;
        }
        else
        {
            for (int i = 0; i < bestData.Length; i++)
            {
                bestData[i].minute = 99;
                bestData[i].seconds = 59.9f;
                nowData[i].minute = 99;
                nowData[i].seconds = 59.9f;
            }

            thisData.minute = 99;
            thisData.seconds = 59.9f;
            dataTime.thisBreak = 0;
        }
    }
}
