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

        public float HitKnockBack;

        public float useEnergy;
        public float maxDamage;
        public float minDamage;

        public float addSpeedPower;

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
    private int comboTimer = 0;
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
        comboTimer = 0;
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
            if (!isChargeAttack)
                comboTimer++;

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
            if (!isChargeAttack)
            {
                if (CheckCombo(comboCount) && inputAttackTrigger)
                {
                    isAttack = true;
                    Combo(comboCount);
                    

                    s_Animator.SetTrigger("isAttack");
                    s_Animator.SetFloat("ComboCount", comboCount);

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
                        && comboTimer > ComboAttackList[0].comboAccseptStartTime
                                    + ComboAttackList[0].comboAccseptLength && !inputAttackTrigger)
                    {
                        comboCount = 0;
                        chargeTimer = 0;
                        isChargeAttack = true;
                        chargeAttack.currentRotationSpeed = playerControll.CurrentRotationSpeed;
                        playerControll.CurrentRotationSpeed *= chargeAttack.RotationSpeedValue;
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

                    Vector3 MoveVel = transform.forward * chargeAttack.runAddSpeed;
                    //rigidBody.AddForce(MoveVel, ForceMode.Acceleration);
                    rigidBody.velocity = MoveVel;

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
                    //Debug.Log("Charge Attack!");
                    //チャージされたフレーム数などからダメージ量を計算
                    float chargeValue = Mathf.Clamp((float)chargeAttack.MaxChargeTime / chargeTimer, 0.0f, 1.0f);
                    float damage = Mathf.Lerp(chargeAttack.minDamage, chargeAttack.maxDamage, chargeValue);


                    if (AttackHitCheck(chargeAttack.attackRange, chargeAttack.attackAngle, damage, chargeAttack.HitKnockBack))
                    {
                        //Debug.Log("ChargeAttack Enemyに当たったよ");
                        StartCoroutine(HitStopCoroutine(chargeAttack.hitStop));
                    }


                    chargeTimer = 0;
                    isChargeAttack = false;
                    isAttack = false;
                    playerControll.CurrentRotationSpeed = chargeAttack.currentRotationSpeed;
                }
            }
        }

        if (!isGround && !isAttack)
        {
            if (inputAttackTrigger)
            {
                comboTimer = 0;
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


    //各コンボの攻撃判定
    void Combo(int list)
    {
        //Debug.Log("Combo" + (list+1));

        rigidBody.velocity = Vector3.zero;
        Vector3 MoveVel = transform.forward * ComboAttackList[list].addSpeedPower;
        //rigidBody.AddForce(MoveVel, ForceMode.Acceleration);
        rigidBody.velocity = MoveVel;

        //エネルギー関連の更新
        float energyValue = playerControll.CurrentEnergy / ComboAttackList[list].useEnergy;
        energyValue = Mathf.Clamp(energyValue, 0, 1);
        playerControll.CurrentEnergy -= ComboAttackList[list].useEnergy;
        
        //ダメージ数値は同じなのにforでやるとコストかかるので
        //外に出しておく
        float damage = Mathf.Lerp(ComboAttackList[list].minDamage, ComboAttackList[list].maxDamage, energyValue);

        if (AttackHitCheck(ComboAttackList[list].attackRange, ComboAttackList[list].attackAngle, damage, ComboAttackList[list].HitKnockBack))
        {
            //Debug.Log("Combo" + list + " Enemyに当たったよ");
            StartCoroutine(HitStopCoroutine(ComboAttackList[list].hitStop));
        }
    }

    bool AttackHitCheck(float range, float angle, float damage, float KnockBack)
    {
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
        return isHit;
    }


    bool CheckCombo(int list)
    {
        if (comboCount == 0)
        {
            return true;
        }
        if (isAttack)
        {
            if ((comboTimer > ComboAttackList[list].comboAccseptStartTime) &&
                (comboTimer < ComboAttackList[list].comboAccseptStartTime 
                                + ComboAttackList[list].comboAccseptLength))
            {
                return true;
            }
        }

        return false;
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
