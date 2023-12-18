using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Timer_decimal_TMP : MonoBehaviour
{
	[SerializeField]
	private int minute;
	public int Minute 
	{
		get { return minute; }
	}

	[SerializeField]
	private float seconds;
	private float Seconds
	{
		get { return seconds; }
	}	
	private float timedecimal;
	//　前のUpdateの時の秒数
	private float oldSeconds;
	//　タイマー表示用テキスト
	//private Text timerText;
	[SerializeField]
	private TextMeshProUGUI timerText;

    private int enemyBreak;

    private bool isStop;
    private IEnumerator s;
	private SaveSystem saveSystem;

    void Start()
	{
		minute = 0;
		seconds = 0f;
		oldSeconds = 0f;
        timerText = GetComponentInChildren<TextMeshProUGUI>();

		enemyBreak = 0;

        isStop = false;

        saveSystem = GetComponentInChildren<SaveSystem>();
		s = EndTimer();
    }

	void Update()
	{
		if(!isStop) 
		{
            seconds += Time.deltaTime;
            if (seconds >= 60f)
            {
                minute++;
                seconds = seconds - 60;
            }

            timedecimal = seconds * 100 - (int)seconds * 100;

            timerText.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00") + ":" + ((int)timedecimal).ToString("00");

            oldSeconds = seconds;
        }
	}

    private IEnumerator EndTimer()
	{
		if (isStop)
		{
			yield break;
		}

        saveSystem.Save(minute, seconds, enemyBreak);
        yield return null;

    }

	public void StopTime()
	{
		StartCoroutine(s);
	}

	public void AddBreakEnemyCount(int count)
	{
        enemyBreak += count;
    }
}