using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Slash_2 : MonoBehaviour
{
    [System.Serializable]
    public struct AttackList
    {
        public float attackRange;
        public float attackAngle;

        public float HitKnockBack;

        public float useEnergy;
        public float maxDamage;
        public float minDamage;

        public float addSpeedPower;

        public int hitStop;
    }
    
    [System.Serializable]
    public struct ChargeAttack
    {
        public float attackRange;
        public float attackAngle;

        public float HitKnockBack;

        public float MaxChargeTime;

        public float maxDamage;
        public float minDamage;

        public float EnergyChargeValue;

        public float runAddSpeed;
        public float maxrunSpeed;

        //public int cooldownTime;
        public int hitStop;

        [Range(0,1)]
        public float RotationSpeedValue;

        [HideInInspector]
        public float currentRotationSpeed;
    }

    [System.Serializable]
    public struct MidAirAttack
    {
        public float attackRange;
        public float HitKnockBack;

        public float maxDamage;
        public float minDamage;

        public float useEnergy;

        public float DownPower;


        //public int cooldownTime;
        public int hitStop;

        //地面がある と認識する長さ
        [Space(5),HideInInspector]
        public float checkUnderLength;
        [HideInInspector]
        public float damage;
    }

    [System.Serializable]
    public struct SE_VFX_PrefabList
    {
        public GameObject Attack1;
        public GameObject Attack2;
        public GameObject Attack3;

        public GameObject MidAirAttack;

        //public GameObject ChargeAttack;
    }

    [Tooltip("ここに 効果音とエフェクトがセットになった\nプレハブを入れてください"), CustomLabel("効果音 エフェクト類"), SerializeField]
    private SE_VFX_PrefabList SE_VFX_Prefabs;

    [SerializeField]
    private List<AttackList> ComboAttackList;

    [SerializeField]
    private ChargeAttack chargeAttack;

    [SerializeField]
    private MidAirAttack midAirAttack;

    [Space(10)]
    [SerializeField]
    private int comboCount = 0;

    [SerializeField, Tooltip("forDebug. do not touch it")]
    private int chargeTimer = 0;

    [SerializeField]
    private string enemyTag = "Enemy";


    private bool isAttack = false;
    private bool isChargeAttack = false;
    private bool isMidAirAttack = false;
    private bool inputAttack = false;
    private bool oldinputAttack = false;

    private PlayerControll playerControll;
    private Rigidbody rigidBody;

    private Animator s_Animator;


   
    [SerializeField]
    private bool isGround = false;

    // Start is called before the first frame update
    void Start()
    {
        chargeTimer = 0;
        comboCount = 0;
        s_Animator = GetComponent<Animator>();
        playerControll = GetComponent<PlayerControll>();
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGround = false;

        float raycastDistance = 0.05f;
        RaycastHit Hit;
        if (Physics.Raycast(transform.position + transform.up * 0.01f, Vector3.down, out Hit, raycastDistance))
        {
            isGround = true;

            //空中攻撃発動
            if (isMidAirAttack)
            {
                isMidAirAttack = false;
                isAttack = false;


                //ダメージ数値は同じなのにforでやるとコストかかるので
                //外に出しておく
                float damage = midAirAttack.damage;

                bool isHit = false;
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, midAirAttack.attackRange);
                foreach (Collider col in hitColliders)
                {
                    if (col.CompareTag(enemyTag))
                    {
                        Vector3 direction = (col.transform.position - transform.position).normalized;
                        isHit = true;

                        //ここで敵の体力を減らす処理をする
                        //ダメージ量はdamageから使ってください

                        if (col.GetComponent<Enemy_Blow>())
                        {
                            col.GetComponent<Enemy_Blow>().CurrentHp -= damage;
                        }
                        if (col.GetComponent<Enemy_Shoot>())
                        {
                            col.GetComponent<Enemy_Shoot>().CurrentHp -= damage;
                        }
                        col.GetComponent<Rigidbody>().AddForce(direction * midAirAttack.HitKnockBack, ForceMode.Impulse);
                    }
                }

                if (isHit)
                {
                    Debug.Log("空中攻撃 Enemyに当たったよ");
                    StartCoroutine(HitStopCoroutine(midAirAttack.hitStop));
                }
            }
        }


        if (isAttack)
        {
            //第一引数 攻撃をしているかどうか 第二引数 チャージ攻撃か(移動制御をこちらに渡す)
            playerControll.AttackStatus(true, isChargeAttack);
        }
        else
        {
            playerControll.AttackStatus(false, false);
        }

        bool inputAttackTrigger = false;
        if (!oldinputAttack && inputAttack)
            inputAttackTrigger = true;

        

        if (isGround)
        {
            //----------------------------------------------------------------
            //通常のコンボ攻撃の処理
            //----------------------------------------------------------------
            if (inputAttackTrigger)
            {

                s_Animator.SetTrigger("ComboAttack");

                comboCount++;

                if (comboCount >= ComboAttackList.Count)
                {
                    comboCount = 0;
                }
                inputAttackTrigger = false;
            }


        }


        //------------------------------------------------------------------
        //空中攻撃の処理
        //------------------------------------------------------------------

        if (!isGround && !isAttack)
        {
            if (inputAttackTrigger)
            {
                rigidBody.velocity = Vector3.zero;
                rigidBody.AddForce(Vector3.down * midAirAttack.DownPower, ForceMode.VelocityChange);
                isMidAirAttack = true;
                isAttack = true;


                //エネルギー関連の更新
                float energyValue = playerControll.CurrentEnergy / midAirAttack.useEnergy;
                energyValue = Mathf.Clamp(energyValue, 0, 1);
                playerControll.CurrentEnergy -= midAirAttack.useEnergy;

                midAirAttack.damage = Mathf.Lerp(midAirAttack.minDamage, midAirAttack.maxDamage, energyValue);
            }
        }

        oldinputAttack = inputAttack;
    }


    //各コンボの移動判定 (アニメーターから呼び出し)
    void AttackStart(int list)
    {
        isAttack = true;
        rigidBody.velocity = Vector3.zero;
        Vector3 MoveVel = transform.forward * ComboAttackList[list].addSpeedPower;
        rigidBody.velocity = MoveVel;
    }

    //攻撃が出るフレーム (アニメーターから呼び出し)
    void AttackFrame(int listNum)
    {
        //エネルギー関連の更新
        float energyValue = playerControll.CurrentEnergy / ComboAttackList[listNum].useEnergy;
        energyValue = Mathf.Clamp(energyValue, 0, 1);
        playerControll.CurrentEnergy -= ComboAttackList[listNum].useEnergy;

        float range = ComboAttackList[listNum].attackRange;
        float angle = ComboAttackList[listNum].attackAngle;
        float KnockBack = ComboAttackList[listNum].HitKnockBack;

        //ダメージ数値は同じなのにforでやるとコストかかるので
        //外に出しておく
        float damage = Mathf.Lerp(ComboAttackList[listNum].minDamage, ComboAttackList[listNum].maxDamage, energyValue);


        bool isHit = false;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag(enemyTag))
            {
                Vector3 direction = (col.transform.position - transform.position).normalized;
                float Tangle = Vector3.Angle(transform.forward, direction);

                if (Tangle <= angle / 2)
                {
                    isHit = true;
                    //敵の体力を減らす処理
                    //チャージ攻撃の時だけ別の場所で処理しているが他は共通でこの関数内で処理している
                    if (col.GetComponent<Enemy_Blow>())
                    {
                        col.GetComponent<Enemy_Blow>().CurrentHp -= damage;
                    }
                    if (col.GetComponent<Enemy_Shoot>())
                    {
                        col.GetComponent<Enemy_Shoot>().CurrentHp -= damage;
                    }


                    col.GetComponent<Rigidbody>().AddForce(direction * KnockBack, ForceMode.Impulse);
                }
            }
        }

        switch (listNum)
        {
            case 0:
                Instantiate(SE_VFX_Prefabs.Attack1, transform.position, transform.rotation);
                break;
            case 1:
                Instantiate(SE_VFX_Prefabs.Attack2, transform.position, transform.rotation);
                break;
            case 2:
                Instantiate(SE_VFX_Prefabs.Attack3, transform.position, transform.rotation);
                break;
        }



        if ( isHit)
            StartCoroutine(HitStopCoroutine(ComboAttackList[listNum].hitStop));
    }


    void AttackEnd()
    {
        isAttack = false;
    }

    private IEnumerator HitStopCoroutine(float flame)
    {
        Debug.Log("Player -> Enemy HitStop (" + flame * Time.deltaTime + " )");
        // ヒットストップの開始
        Time.timeScale = 0f;
        // 指定した時間だけ停止
        yield return new WaitForSecondsRealtime(flame * Time.deltaTime);
        // ヒットストップの終了
        Time.timeScale = 1f;

    }

    private void OnDrawGizmos()
    {

        if (comboCount == 0)
            return;

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, ComboAttackList[comboCount].attackRange);
    }



    //攻撃入力がトリガーかどうかは受信側で判断できます
    public void inputAttackTrigger(bool val)
    {
        inputAttack = val;
    }

}
