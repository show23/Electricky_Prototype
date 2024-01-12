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

    [SerializeField] private float _time = 0.5f;


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

        Vector3 cameraPosOrigin = cameraTrans.position;
        Quaternion cameraRot = cameraTrans.rotation;

        Vector3 v3 = t.position;
        v3.y += 1f;
        v3 += (t.forward * offsetForward);

        //--
        //Vector3 diffPos = v3 - cameraPosOrigin;
        //Vector3 diffRot = cameraRot.eulerAngles - t.rotation.eulerAngles;

        //diffRot = CheckRot(diffRot);

        //diffRot.y += 180f - 10f;

        //diffRot = CheckRot(diffRot);

        //Debug.Log(t.rotation.eulerAngles);
        //Debug.Log(diffRot);

        //float timeAdd = 0.0f;

        //for (;;)
        //{
        //    Vector3 deltaPos = diffPos * (Time.deltaTime / _time);
        //    Vector3 deltaRot = diffRot * (Time.deltaTime / _time);

        //    cameraTrans.position += deltaPos;
        //    cameraTrans.Rotate(deltaRot, Space.World);

        //    timeAdd += Time.deltaTime;
        //    if(timeAdd >= _time)
        //    {
        //        break;
        //    }

        //    yield return null;
        //}
        //--

        cameraTrans.position = v3;
        cameraTrans.rotation = t.rotation;
        cameraTrans.Rotate(0f, 180f - 10f, 0, Space.World);
        cameraTrans.localScale = Vector3.one;

        Debug.Log(cameraTrans.rotation.eulerAngles);

        


        if (EventObject != null)
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


    private Vector3 CheckRot(Vector3 t)
    {
        Vector3 vector3 = t;
        
        for(;;)
        {
            int num = 0;

            if (vector3.x > 360)
                num = 1;
            if (vector3.x < -360)
                num = 2;
            if (vector3.x > 180)
                num = 3;
            if (vector3.x < -180)
                num = 4;

            switch (num) 
            {
                case 1:
                    vector3.x -= 360f;
                    break;

                case 2:
                    vector3.x += 360f;
                    break;

                case 3:
                    vector3.x = -360f + vector3.x;
                    break;

                case 4:
                    vector3.x = 360f - vector3.x;
                    break;
            }

            if(num == 0)
            { break; }
        }

        for (;;)
        {
            int num = 0;

            if (vector3.y > 360)
                num = 1;
            if (vector3.y < -360)
                num = 2;
            if (vector3.y > 180)
                num = 3;
            if (vector3.y < -180)
                num = 4;

            switch (num)
            {
                case 1:
                    vector3.y -= 360f;
                    break;

                case 2:
                    vector3.y += 360f;
                    break;

                case 3:
                    vector3.y = -360f + vector3.y;
                    break;

                case 4:
                    vector3.y = 360f - vector3.y;
                    break;
            }

            if (num == 0)
            { break; }
        }

        for (;;)
        {
            int num = 0;

            if (vector3.z > 360)
                num = 1;
            if (vector3.z < -360)
                num = 2;
            if (vector3.z > 180)
                num = 3;
            if (vector3.z < -180)
                num = 4;

            switch (num)
            {
                case 1:
                    vector3.z -= 360f;
                    break;

                case 2:
                    vector3.z += 360f;
                    break;

                case 3:
                    vector3.z = -360f + vector3.z;
                    break;

                case 4:
                    vector3.z = 360f - vector3.z;
                    break;
            }

            if (num == 0)
            { break; }
        }

        return vector3;
    }
}
