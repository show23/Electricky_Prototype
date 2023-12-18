using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Result : MonoBehaviour
{
    //ugokasu
    [SerializeField] private RectTransform _resultClear;
    [SerializeField] private RectTransform _resultGroup;

    [SerializeField] private float _resultClrearEndPosY = -185f;
    [SerializeField] private float _resultGroupEndPosX = -1050f;
    [SerializeField] private float _resultInTime = 0.50f;

    // time ka score no dottika dakede yoikamo?
    [SerializeField] private TextMeshProUGUI _textTime;

    private SaveSystem saveSystem;

    // other scene
    private Scene _scene;

    private Coroutine _coroutine;

    // Start is called before the first frame update
    void Start()
    {
        // _scene = SceneManager.GetSceneByName("f");


        saveSystem = GetComponentInChildren<SaveSystem>();
        saveSystem.Load();

        int minute = saveSystem.ThisData.minute;
        float seconds = saveSystem.ThisData.seconds;
        float timedecimal = seconds * 100 - (int)seconds * 100;

        //_textTime = GetComponentInChildren<TextMeshProUGUI>();
        _textTime.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00") + ":" + ((int)timedecimal).ToString("00");

        _coroutine = StartCoroutine(SceneInPos());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SceneInPos()
    {
        Vector2 yC = _resultClear.anchoredPosition;
        _resultClear.anchoredPosition = new Vector2(yC.x, 0.0f);

        Vector2 xG = _resultGroup.anchoredPosition;
        _resultGroup.anchoredPosition = new Vector2(0.0f, xG.y);

        float timeAdd = 0.0f;

        for (;;)
        {
            float clearPosY = _resultClrearEndPosY * (Time.deltaTime / _resultInTime);
            float groupPosX = _resultGroupEndPosX * (Time.deltaTime / _resultInTime);

            _resultClear.anchoredPosition += new Vector2(0.0f, clearPosY);
            _resultGroup.anchoredPosition += new Vector2(groupPosX, 0.0f);

            timeAdd += Time.deltaTime;

            if(timeAdd >= _resultInTime)
            {
                break;
            }

            yield return null;
        }

        _resultClear.anchoredPosition = new Vector2(yC.x, _resultClrearEndPosY);
        _resultGroup.anchoredPosition = new Vector2(_resultGroupEndPosX, xG.y);



    }

    private void Init()
    {
        // kokoni other scene kara mottekita data wo TMP ni utikomu
    }
}
