using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Slash : MonoBehaviour
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
    
    [System.Serializable]
    public struct ChargeAttack
    {
        public float attackRange;
        public float attackAngle;

        public float MaxChargeTime;

        public float maxDamage;
        public float minDamage;

        public float EnergyChargeValue;

        public float runAddSpeed;
        public float maxrunSpeed;

        public int cooldownTime;
        public int hitStop;
    }

    [SerializeField]
    private int comboCount = 0;
    private int chargeTimer = 0;

    [SerializeField]
    private List<AttackList> ComboAttackList;

    [SerializeField]
    private ChargeAttack chargeAttack;
    public string enemyTag = "Enemy";

    private Vector3 MoveForward;

    [SerializeField]
    private bool isAttack = false;
    [SerializeField]
    private bool isChargeAttack = false;
    [SerializeField]
    private bool inputAttack = false;
    [SerializeField]
    private bool oldinputAttack = false;

    private PlayerControll playerControll;
    private Rigidbody rigidBody;

    [SerializeField,Tooltip("forDebug. do not touch it")]
    private int comboTimer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        chargeTimer = 0;
        comboTimer = 0;
        comboCount = 0;
        playerControll = GetComponent<PlayerControll>();
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAttack)
        {
            if (!isChargeAttack)
                comboTimer++;

            playerControll.AttackStatus(true);
        }
        else
        {
            playerControll.AttackStatus(false);
        }


        //----------------------------------------------------------------
        //通常のコンボ攻撃の処理
        //----------------------------------------------------------------

        if (!isChargeAttack)
        {
            bool inputAttackTrigger = false;
            if (!oldinputAttack && inputAttack)
                inputAttackTrigger = true;

            if (CheckCombo(comboCount) && inputAttackTrigger)
            {
                isAttack = true;
                Combo(comboCount);
                comboCount++;
                comboTimer = 0;

                if (comboCount >= ComboAttackList.Count)
                {
                    comboCount = 0;
                }
            }
            else
            {   //攻撃1手目からチャージ攻撃に以降する
                
                if (comboCount == 1 && inputAttack 
                    && comboTimer > ComboAttackList[comboCount].cooldownTime && !inputAttackTrigger)
                {
                    comboCount = 0;
                    chargeTimer = 0;
                    isChargeAttack = true;
                }
                
                if (comboTimer > ComboAttackList[comboCount].cooldownTime)
                {
                    isAttack = false;
                    comboCount = 0;
                }
            }
        }


        //----------------------------------------------------------------
        //チャージ攻撃の処理
        //----------------------------------------------------------------

        if (isChargeAttack)
        {
            isAttack = true;
            //貯めながら走る
            if (inputAttack)
            {

                float playerSpeed = new Vector2(rigidBody.velocity.x, rigidBody.velocity.z).magnitude;
                playerControll.CurrentEnergy += playerSpeed * chargeAttack.EnergyChargeValue;
                
                chargeTimer++;

                Vector3 MoveVel = MoveForward.normalized * chargeAttack.runAddSpeed;
                rigidBody.AddForce(MoveVel, ForceMode.Acceleration);

                //新 速度制限
                {
                    Vector3 Vel = Vector3.Scale(rigidBody.velocity, new Vector3(1, 0, 1));

                    if (Vel.magnitude > chargeAttack.maxrunSpeed)
                    {
                        Vel = Vel.normalized * chargeAttack.maxrunSpeed;
                        Vel.y = rigidBody.velocity.y;
                        rigidBody.velocity = Vel;
                    }
                }

            }
            else
            //走り終わって貯め斬り！
            {
                Debug.Log("Charge Attack!");
                //チャージされたフレーム数などからダメージ量を計算
                float chargeValue = Mathf.Clamp((float)chargeAttack.MaxChargeTime / chargeTimer, 0.0f, 1.0f);
                float damage = Mathf.Lerp(chargeAttack.minDamage, chargeAttack.maxDamage, chargeValue);


                if (AttackHitCheck(chargeAttack.attackRange, chargeAttack.attackAngle, damage))
                {
                    Debug.Log("ChargeAttack Enemyに当たったよ");
                    StartCoroutine(HitStopCoroutine(chargeAttack.hitStop));
                }


                chargeTimer = 0;
                isChargeAttack = false;
                isAttack = false;
            }
        }

        oldinputAttack = inputAttack;
    }


    //各コンボの攻撃判定
    void Combo(int list)
    {
        Debug.Log("Combo" + (list+1));

        //エネルギー関連の更新
        float energyValue = playerControll.CurrentEnergy / ComboAttackList[list].useEnergy;
        energyValue = Mathf.Clamp(energyValue, 0, 1);
        playerControll.CurrentEnergy -= ComboAttackList[list].useEnergy;
        
        //ダメージ数値は同じなのにforでやるとコストかかるので
        //外に出しておく
        float damage = Mathf.Lerp(ComboAttackList[list].minDamage, ComboAttackList[list].maxDamage, energyValue);

        if (AttackHitCheck(ComboAttackList[list].attackRange, ComboAttackList[list].attackAngle, damage))
        {
            Debug.Log("Combo" + list + " Enemyに当たったよ");
            StartCoroutine(HitStopCoroutine(ComboAttackList[list].hitStop));
        }
    }

    bool AttackHitCheck(float range, float angle, float damage)
    {
        bool isHit = false;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag(enemyTag))
            {
                Vector3 direction = col.transform.position - transform.position;
                float Tangle = Vector3.Angle(transform.forward, direction);

                if (Tangle <= angle / 2)
                {
                    isHit = true;
                    //ここで敵の体力を減らす処理をする
                    //ダメージ量はdamageから使ってください








                }
            }
        }
        return isHit;
    }


    bool CheckCombo(int list)
    {
        if (comboCount == 0)
        {
            Debug.Log("CheckCombo Pass (combo = 0)");
            return true;
        }
        if (isAttack)
        {
            if ((comboTimer > ComboAttackList[list].comboAccseptStartTime) &&
                (comboTimer < ComboAttackList[list].comboAccseptStartTime 
                                + ComboAttackList[list].comboAccseptLength))
            {
                Debug.Log("CheckCombo Pass (time is good)");
                return true;
            }
        }

        Debug.Log("CheckCombo not Pass");
        return false;
    }


    private IEnumerator HitStopCoroutine(float time)
    {
        Debug.Log("HitStop");
        // ヒットストップの開始
        Time.timeScale = 0f;
        // 指定した時間だけ停止
        yield return new WaitForSecondsRealtime(time);
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
    public void inputAttackTrigger(bool val,Vector3 moveforward)
    {
        inputAttack = val;
        MoveForward = moveforward;
    }

}
