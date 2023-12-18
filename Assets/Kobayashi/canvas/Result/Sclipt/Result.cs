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
    [SerializeField] private RectTransform _resultRank;

    [SerializeField] private float _resultClrearEndPosY = -185f;
    [SerializeField] private float _resultGroupEndPosX = -1050f;
    [SerializeField] private float _resultRankEndPosX = -1100;
    [SerializeField] private float _resultInTime = 0.50f;
    [SerializeField] private float _waitTime = 3.0f;

    // time ka score no dottika dakede yoikamo?
    [SerializeField] private TextMeshProUGUI _textTime;

    [SerializeField] private TextMeshProUGUI _textEnemyBreak;

    [SerializeField] private TextMeshProUGUI[] _rankTime;

    [SerializeField] private TextMeshProUGUI _textYouTime;

    private SaveSystem saveSystem;

    private enum State
    { 
        Non,

        Intro,
        ThisResult,
        OutThisResult,
        InRanking,
        Ranking,

        End
    }
    private State state = State.Non;

    // other scene
    private Scene _scene;

    private IEnumerator[] c;
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

        int enemyBreak = saveSystem.BreakEnemy;

        //_textTime = GetComponentInChildren<TextMeshProUGUI>();
        _textTime.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00") + ":" + ((int)timedecimal).ToString("00");
        _textEnemyBreak.text = enemyBreak.ToString("0000");

        _textYouTime.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00") + ":" + ((int)timedecimal).ToString("00");

        for (int i =0; i < _rankTime.Length; i++) 
        {
            minute = saveSystem.BestData[i].minute;
            seconds = saveSystem.BestData[i].seconds;
            timedecimal = seconds * 100 - (int)seconds * 100;

            _rankTime[i].text = minute.ToString("00") + ":" + ((int)seconds).ToString("00") + ":" + ((int)timedecimal).ToString("00");
        }

        c = new IEnumerator[] {SceneInResultPos(), SceneOutResultPos(), SceneInRankPos() };
        _coroutine = StartCoroutine(c[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SceneInResultPos()
    {
        Vector2 yC = _resultClear.anchoredPosition;
        Vector2 xG = _resultGroup.anchoredPosition;
        Vector2 yR = _resultRank.anchoredPosition;

        _resultClear.anchoredPosition = new Vector2(yC.x, 0.0f);
        _resultGroup.anchoredPosition = new Vector2(0.0f, xG.y);
        _resultRank.anchoredPosition = new Vector2(0.0f, yR.y);

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

        yield return new WaitForSeconds(_waitTime);
        _coroutine = StartCoroutine(c[1]);
    }

    private IEnumerator SceneOutResultPos()
    {
        Vector2 xC = _resultClear.anchoredPosition;
        Vector2 yG = _resultGroup.anchoredPosition;
        Vector2 yR = _resultRank.anchoredPosition;

        _resultClear.anchoredPosition = new Vector2(xC.x, _resultClrearEndPosY);
        _resultGroup.anchoredPosition = new Vector2(_resultGroupEndPosX, yG.y);
        _resultRank.anchoredPosition = new Vector2(0.0f, yR.y);

        float timeAdd = 0.0f;

        for(;;)
        {
            float groupPosX = _resultGroupEndPosX * (Time.deltaTime / _resultInTime);

            _resultGroup.anchoredPosition -= new Vector2(groupPosX, 0.0f);

            timeAdd += Time.deltaTime;
            if (timeAdd >= _resultInTime)
            {
                break;
            }

            yield return null;
        }

        _resultGroup.anchoredPosition = new Vector2(0.0f, yG.y);

        //yield return new WaitForSeconds(_waitTime);
        _coroutine = StartCoroutine(c[2]);
    }

    private IEnumerator SceneInRankPos()
    {
        Vector2 xC = _resultClear.anchoredPosition;
        Vector2 yG = _resultGroup.anchoredPosition;
        Vector2 yR = _resultRank.anchoredPosition;

        _resultClear.anchoredPosition = new Vector2(xC.x, _resultClrearEndPosY);
        _resultGroup.anchoredPosition = new Vector2(0.0f, yG.y);
        _resultRank.anchoredPosition = new Vector2(0.0f, yR.y);

        float timeAdd = 0.0f;

        for (;;)
        {
            float rankPosX = _resultRankEndPosX * (Time.deltaTime / _resultInTime);

            _resultRank.anchoredPosition += new Vector2(rankPosX, 0.0f);

            timeAdd += Time.deltaTime;
            if (timeAdd >= _resultInTime)
            {
                break;
            }

            yield return null;
        }

        _resultRank.anchoredPosition = new Vector2(_resultRankEndPosX, yR.y);
    }
}
