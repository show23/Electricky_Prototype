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
        EventObject = FindObjectOfType<EventSystem>().gameObject;
        UiObject = GameObject.Find("UI");
        audioListener = FindObjectOfType<AudioListener>().GetComponent<AudioListener>();

        playerInput.enabled = false;
        UiObject.SetActive(false);

        yield return null;

        opc.enabled = false;
        cm.enabled = false;

        Vector3 v3 = tracePosition.position;
        v3.y += 1f;
        v3 += (tracePosition.forward * 1.8f);
        v3 += (tracePosition.right * -0.2f);

        cameraTrans.position = v3;
        cameraTrans.rotation = tracePosition.rotation;
        cameraTrans.Rotate(0f, 180f, 0, Space.World);
        cameraTrans.localScale = Vector3.one;

        EventObject.SetActive(false);
        audioListener.enabled = false;
        SceneManager.LoadScene("result", LoadSceneMode.Additive);
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
