using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{

   
    private int comboCount = 0;
    public int maxCombo = 4;
    public string enemyTag = "Enemy";
    public float attackRange = 4;
    public float comboResetTime = 1.0f; // タイマーの時間（秒）
    private float lastKeyPressTime = 0.0f;
    bool kAllowed = true;
    public float cooldownTime = 0.3f;
    public float hitStop = 0.1f;
    public float attackAngle = 90;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (kAllowed && Input.GetKeyDown(KeyCode.K))
        {
            kAllowed = false;
            comboCount++;
            lastKeyPressTime = Time.time;
            
        }


        if (Time.time - lastKeyPressTime > comboResetTime)
        {
            comboCount = 0;
        }
        if (comboCount >= maxCombo + 1)
        {
            comboCount = 0;
        }
        switch (comboCount)            
        {
            case 1:
                
                Combo1();
                break;

            case 2:
                Combo2();
                break;

            case 3:
                Combo3();
                break;
            case 4:
                Combo4();
                break;
        }
        //Debug.Log(comboCount);
    }

    void Combo1()
    {
        //Debug.Log("Combo1");
        StartCoroutine(EnableKAfterCooldown());
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag(enemyTag))
            {
                Vector3 direction = col.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, direction);

                if (angle <= attackAngle / 2)
                {
                    Debug.Log("Combo1 Enemyに当たったよ");
                    StartCoroutine(HitStopCoroutine());

                }
            }
        }
    }
    void Combo2()
    {
        //Debug.Log("Combo2");
        StartCoroutine(EnableKAfterCooldown());
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag(enemyTag))
            {
                Vector3 direction = col.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, direction);

                if (angle <= attackAngle / 2)
                {
                    Debug.Log("Combo2 Enemyに当たったよ");
                    StartCoroutine(HitStopCoroutine());

                }
            }
        }
    }
    void Combo3()
    {
        //Debug.Log("Combo3");
        StartCoroutine(EnableKAfterCooldown());
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag(enemyTag))
            {
                Vector3 direction = col.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, direction);

                if (angle <= attackAngle / 2)
                {
                    Debug.Log("Combo3 Enemyに当たったよ");
                    StartCoroutine(HitStopCoroutine());

                }
            }
        }
    }
    void Combo4()
    {
       // Debug.Log("Combo4");
        StartCoroutine(EnableKAfterCooldown());
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag(enemyTag))
            {
                Vector3 direction = col.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, direction);

                if (angle <= attackAngle / 2)
                {
                    Debug.Log("Combo4 Enemyに当たったよ");
                    StartCoroutine(HitStopCoroutine());

                }
            }
        }
    }
    IEnumerator EnableKAfterCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        kAllowed = true;
    }

    private IEnumerator HitStopCoroutine()
    {
        Debug.Log("HitStop");
        // ヒットストップの開始
        Time.timeScale = 0f;
        // 指定した時間だけ停止
        yield return new WaitForSecondsRealtime(hitStop);
        // ヒットストップの終了
        Time.timeScale = 1f;

    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, attackRange);
       
    }
}
