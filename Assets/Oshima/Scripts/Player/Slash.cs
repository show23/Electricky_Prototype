using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    [System.Serializable]
    public struct AttackList
    {
        public float attackRange;
        public float attackAngle;

        public float useEnergy;
        public float maxDamage;
        public float minDamage;

        public int comboAccseptStartTime;
        public int comboAccseptLength;
        
        public int cooldownTime;
        public int hitStop;
    }

    private int comboCount = 0;
    public int maxCombo = 4;
    [SerializeField]
    private List<AttackList> attackList;
    public string enemyTag = "Enemy";

    private bool isAttack = false;
    private bool inputAttack = false;

    private PlayerControll playerControll;

    [SerializeField,Tooltip("forDebug. do not touch it")]
    private int comboTimer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        comboTimer = 0;
        playerControll = FindObjectOfType<PlayerControll>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAttack)
        {
            comboTimer++;
        }

        if (CheckCombo(comboCount) && inputAttack)
        {
            isAttack = true;
            Combo(comboCount);
            comboCount++;
            comboTimer = 0;

            if (comboCount >= maxCombo + 1)
            {
                comboCount = 0;
            }
        }

        if (comboTimer > attackList[comboCount].cooldownTime)
        {
            isAttack = false;
            comboCount = 0;
        }
        inputAttack = false;
    }

    void Combo(int list)
    {
        Debug.Log("Combo" + list+1);
        StartCoroutine(EnableKAfterCooldown(list));
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackList[list].attackRange);

        float energyValue = playerControll.CurrentEnergy / attackList[list].useEnergy;
        energyValue = Mathf.Clamp(energyValue, 0, 1);
        //ダメージ数値は同じなのにforでやるとコストかかるので
        //外に出しておく
        float damage = Mathf.Lerp(attackList[list].minDamage, attackList[list].maxDamage, energyValue);

        playerControll.CurrentEnergy -= attackList[list].useEnergy;

        
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag(enemyTag))
            {
                Vector3 direction = col.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, direction);

                if (angle <= attackList[list].attackAngle / 2)
                {
                    Debug.Log("Combo"+list+" Enemyに当たったよ");
                    StartCoroutine(HitStopCoroutine(list));

                    //ここで敵の体力を減らす処理をする
                    //ダメージ量はリストから使うのではなく
                    //電力量を考慮した値を80行目あたりで(damage変数)求めてあるので
                    //そこから使ってください

                }
            }
        }
    }

    bool CheckCombo(int list)
    {
        if (comboCount == 0)
            return true;

        if (isAttack)
        {
            if ((comboCount > attackList[list].comboAccseptStartTime) &&
                (comboCount <
                attackList[list].comboAccseptStartTime + attackList[list].comboAccseptLength))
            {
                return true;
            }
        }

        comboCount = 0;
        return false;
    }


    IEnumerator EnableKAfterCooldown(int list)
    {
        yield return new WaitForSeconds(attackList[list].cooldownTime);
    }

    private IEnumerator HitStopCoroutine(int list)
    {
        Debug.Log("HitStop");
        // ヒットストップの開始
        Time.timeScale = 0f;
        // 指定した時間だけ停止
        yield return new WaitForSecondsRealtime(attackList[list].hitStop);
        // ヒットストップの終了
        Time.timeScale = 1f;

    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        //Gizmos.DrawWireSphere(transform.position, attackRange);
       
    }



    public void inputAttackTrigger()
    {
        inputAttack = true;
    }
}
