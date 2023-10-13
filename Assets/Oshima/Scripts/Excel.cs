//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;

//public class Excel : MonoBehaviour
//{
//    TextAsset csvFile; // CSVファイル
//    List<List<string>> csvDatas = new List<List<string>>(); // CSVの中身を入れるリスト;

//    void Start()
//    {
//        csvFile = Resources.Load("Book1") as TextAsset; // リソースフォルダ内のCSVファイルを読み込む
//        StringReader reader = new StringReader(csvFile.text);

//        // , で分割しつつ一行ずつ読み込み
//        // リストに追加していく
//        while (reader.Peek() != -1) // reader.Peekが-1になるまで
//        {
//            string line = reader.ReadLine(); // 一行ずつ読み込み
//            string[] values = line.Split(','); // , 区切りでデータを分割
//            List<string> row = new List<string>(values);
//            csvDatas.Add(row); // リストに追加
//        }

//        // Unityの変数にCSVから読み込んだデータを代入する例
//        if (csvDatas.Count > 1 && csvDatas[1].Count > 1)
//        {
//            float moveSpeedFromCSV = float.Parse(csvDatas[1][1]); // 例: CSVの2列目の値をfloatに変換
//            PlayerMove.instance.moveSpeed = moveSpeedFromCSV;
//        }

//        if (csvDatas.Count > 2 && csvDatas[2].Count > 1)
//        {
//            float dashSpeedFromCSV = float.Parse(csvDatas[2][1]); // 例: CSVの2列目の値をfloatに変換
//            PlayerMove.instance.dashSpeed = dashSpeedFromCSV;
//        }

//        if (csvDatas.Count > 5 && csvDatas[5].Count > 1)
//        {
//            float hikiyoseFromCSV = float.Parse(csvDatas[5][1]);
//            PlayerAttractor.instance.attractionRange = hikiyoseFromCSV;
//        }

//        if (csvDatas.Count > 6 && csvDatas[6].Count > 1)
//        {
//            float hikiyoseForceFromCSV = float.Parse(csvDatas[6][1]);
//            PlayerAttractor.instance.attractionForce = hikiyoseForceFromCSV;
//        }

//        if (csvDatas.Count > 7 && csvDatas[7].Count > 1)
//        {
//            float hikiyoseStopFromCSV = float.Parse(csvDatas[7][1]);
//            PlayerAttractor.instance.stopDistance = hikiyoseStopFromCSV;
//        }

//        if (csvDatas.Count > 8 && csvDatas[8].Count > 1)
//        {
//            float hikiyoseIncreaseFromCSV = float.Parse(csvDatas[8][1]);
//            PlayerAttack.instance.increasedRange = hikiyoseIncreaseFromCSV;
//        }

//        if (csvDatas.Count > 9 && csvDatas[9].Count > 1)
//        {
//            float hikiyoseRangeFromCSV = float.Parse(csvDatas[9][1]);
//            PlayerAttack.instance.maxInteractionRange = hikiyoseRangeFromCSV;
//        }
//        if (csvDatas.Count > 5 && csvDatas[5].Count > 4)
//        {
//            float LineTimeFromCSV = float.Parse(csvDatas[5][4]);
//            Line.instance.buttonPressDuration = LineTimeFromCSV;
//        }
//        if (csvDatas.Count > 6 && csvDatas[5].Count > 4)
//        {
//            float LineDistanceFromCSV = float.Parse(csvDatas[6][4]);
//            Line.instance.maxDistance = LineDistanceFromCSV;
//        }
//    }
//}
