using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Scene_Change_Game : MonoBehaviour
{
    private GameObject UiObject;
    private GameObject EventObject;

    private Transform charaLookAtPosition;
    private Transform tracePosition;
    private ObjectPositionCopy opc;
    private CameraMove_ByFukuda_3 cm;

    private Transform cameraTrans;
    private AudioListener audioListener;

    private Timer_decimal_TMP timeUI;
    private PlayerInput playerInput;
    private IEnumerator c;
    private Coroutine _coroutine;

    [SerializeField] private float offsetLeft = 1.3f;
    [SerializeField] private float offsetForward = 1.8f;
    [SerializeField] private float angle = 0.3f;


    // Start is called before the first frame update
    void Start()
    {
        opc = FindObjectOfType<ObjectPositionCopy>().GetComponent<ObjectPositionCopy>();
        charaLookAtPosition = opc.transform;

        tracePosition = FindObjectOfType<PlayerInput>().transform;
        cameraTrans = FindObjectOfType<Camera>().transform;

        cm = FindObjectOfType<CameraMove_ByFukuda_3>().GetComponent<CameraMove_ByFukuda_3>();

        c = TransferResult();
    }

    private IEnumerator TransferResult()
    {
        timeUI = FindObjectOfType<Timer_decimal_TMP>().GetComponent<Timer_decimal_TMP>();
        timeUI.StopTime();
        playerInput = FindObjectOfType<PlayerInput>().GetComponent<PlayerInput>();
        if (SceneManager.GetActiveScene().name == "K0134S)-(I_UI")
            EventObject = FindObjectOfType<EventSystem>().gameObject;
        UiObject = GameObject.Find("UI");
        audioListener = FindObjectOfType<AudioListener>().GetComponent<AudioListener>();

        playerInput.enabled = false;
        UiObject.SetActive(false);

        yield return null;

        opc.enabled = false;
        cm.enabled = false;

        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemys)
        {
            enemy.SetActive(false);
        }

        GameObject[] Bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in Bullets)
        {
            bullet.SetActive(false);
        }

        Transform t = new GameObject().transform;
        t.position = tracePosition.position;
        t.position += tracePosition.right * -offsetLeft;
        //t.position = new Vector3(t.position.x, 0f, t.position.z);
        t.rotation = tracePosition.rotation;
        t.RotateAround(tracePosition.position, Vector3.up, 30f);

        Vector3 v3 = t.position;
        v3.y += 1f;
        v3 += (t.forward * offsetForward);

        cameraTrans.position = v3;
        cameraTrans.rotation = t.rotation;
        cameraTrans.Rotate(0f, 180f - 10f, 0, Space.World);
        cameraTrans.localScale = Vector3.one;

        if(EventObject != null)
        EventObject.SetActive(false);
        audioListener.enabled = false;
        SceneManager.LoadScene("result", LoadSceneMode.Additive);

        for (;;)
        {
            yield return null;

            Vector3 p = tracePosition.position;
            t.RotateAround(p, Vector3.up, -angle);

            Vector3 vR3 = t.position;
            vR3.y += 1f;
            vR3 += (t.forward * offsetForward);

            cameraTrans.position = vR3;
            cameraTrans.rotation = t.rotation;
            cameraTrans.Rotate(0f, 180f - 10f, 0, Space.World);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _coroutine = StartCoroutine(c);
        }
    }
}
