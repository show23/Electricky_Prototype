using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Unity.VisualScripting;
//using UnityEditor.SearchService;

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
        InThisResult,
        ThisResult,
        OutThisResult,
        InRanking,
        Ranking,
        OutRanking,

        Move,
        ManualResult,
        ManualRanking,

        RoadTitle,

        End
    }
    private State state = State.Non;

    // other scene
    private Scene _scene;
    private Scene _sceneDebug;

    [SerializeField] private Transform _cameraObject;

    private IEnumerator c;
    private IEnumerator cMove;
    private Coroutine _coroutine;

    // Start is called before the first frame update
    void Start()
    {
        //_cameraObject = FindObjectOfType<Camera>().transform;

        _scene = SceneManager.GetSceneByName("BuildScene");
        _sceneDebug = SceneManager.GetSceneByName("K0134S)-(I_UI");

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

        c = SceneInResultAndClearPos();
        _coroutine = StartCoroutine(c);
    }

    // Update is called once per frame
    void Update()
    {
        if (_scene.IsValid() != false)
        {
            foreach (GameObject rootGameObject in _scene.GetRootGameObjects())
            {
                Camera camera = rootGameObject.GetComponent<Camera>();
                if (camera != null)
                {
                    _cameraObject.position = camera.transform.position;
                    _cameraObject.rotation = camera.transform.rotation;
                    break;
                }
            }
        }

        if (_sceneDebug.IsValid() != false)
        {
            foreach (GameObject rootGameObject in _sceneDebug.GetRootGameObjects())
            {
                Camera camera = rootGameObject.GetComponent<Camera>();
                if (camera != null)
                {
                    _cameraObject.position = camera.transform.position;
                    _cameraObject.rotation = camera.transform.rotation;
                    break;
                }
            }
        }
    }

    public void PressDecision()
    {
        if(state == State.ThisResult)
        {
            StopCoroutine(c);
            c = SceneOutResultPos();
            _coroutine = StartCoroutine(c);
        }
        else if(state == State.Ranking | state == State.ManualResult | state == State.ManualRanking) 
        {
            state = State.RoadTitle;
            SceneManager.LoadScene("Title");
        }
    }

    public void PressLeftAndRight()
    {
        Debug.Log(state);

        if(state == State.ManualResult) 
        {
            c = SceneOutResultPos(true);
            _coroutine = StartCoroutine(c);
        }
        if(state == State.ManualRanking | state == State.Ranking)
        {
            c = SceneOutRankPos();
            _coroutine = StartCoroutine(c);
        }
    }

    // 0
    private IEnumerator SceneInResultAndClearPos()
    {
        state = State.Intro;

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

        state = State.ThisResult;
        yield return new WaitForSeconds(_waitTime);

        cMove = SceneOutResultPos();
        _coroutine = StartCoroutine(cMove);
    }

    // 1  5(true)
    private IEnumerator SceneOutResultPos(bool manual = false)
    {
        state = State.OutThisResult;

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

        if (manual)
        {
            cMove = SceneInRankPos(true);
            _coroutine = StartCoroutine(cMove);
        }
        else
        {
            cMove = SceneInRankPos();
            _coroutine = StartCoroutine(cMove);
        }
    }

    // 2  6(true)
    private IEnumerator SceneInRankPos(bool manual = false)
    {
        state = State.InRanking;

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

        if( manual ) 
            state = State.ManualRanking;
        else
            state = State.Ranking;
    }

    // 3
    private IEnumerator SceneOutRankPos()
    {
        state = State.OutRanking;

        Vector2 xC = _resultClear.anchoredPosition;
        Vector2 yG = _resultGroup.anchoredPosition;
        Vector2 yR = _resultRank.anchoredPosition;

        _resultClear.anchoredPosition = new Vector2(xC.x, _resultClrearEndPosY);
        _resultGroup.anchoredPosition = new Vector2(0.0f, yG.y);
        _resultRank.anchoredPosition = new Vector2(_resultRankEndPosX, yR.y);

        float timeAdd = 0.0f;

        for (;;)
        {
            float rankPosX = _resultRankEndPosX * (Time.deltaTime / _resultInTime);

            _resultRank.anchoredPosition -= new Vector2(rankPosX, 0.0f);

            timeAdd += Time.deltaTime;
            if (timeAdd >= _resultInTime)
            {
                break;
            }

            yield return null;
        }

        _resultRank.anchoredPosition = new Vector2(0.0f, yR.y);
        cMove = SceneInResultPos();
        _coroutine = StartCoroutine(cMove);
    }

    // 4
    private IEnumerator SceneInResultPos()
    {
        state = State.InThisResult;

        Vector2 xC = _resultClear.anchoredPosition;
        Vector2 yG = _resultGroup.anchoredPosition;
        Vector2 yR = _resultRank.anchoredPosition;

        _resultClear.anchoredPosition = new Vector2(xC.x, _resultClrearEndPosY);
        _resultGroup.anchoredPosition = new Vector2(0.0f, yG.y);
        _resultRank.anchoredPosition = new Vector2(0.0f, yR.y);

        float timeAdd = 0.0f;

        for (;;)
        {
            float groupPosX = _resultGroupEndPosX * (Time.deltaTime / _resultInTime);

            _resultGroup.anchoredPosition += new Vector2(groupPosX, 0.0f);

            timeAdd += Time.deltaTime;
            if (timeAdd >= _resultInTime)
            {
                break;
            }

            yield return null;
        }

        _resultGroup.anchoredPosition = new Vector2(_resultGroupEndPosX, yG.y);

        state = State.ManualResult;
    }
}
