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
	void Start()
	{
		minute = 0;
		seconds = 0f;
		oldSeconds = 0f;
        timerText = GetComponentInChildren<TextMeshProUGUI>();
	}

	void Update()
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