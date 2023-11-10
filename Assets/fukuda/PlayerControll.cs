using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Player_Slash))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerControll : MonoBehaviour
{
    //Other Objects & Scripts
    private GameObject PlayerCamera;
    private GameObject PlayerCameraOrigin;

    //ゲーム内で変更される系のステータス
    [SerializeField]
    private float HP;
    [SerializeField]
    private float maxHP;

    [SerializeField]
    private int HPHealTime;

    private int healTimer;
    private float oldHP;

    [SerializeField]
    private float HPHealValue;

    [SerializeField]
    private float Energy;
    [SerializeField]
    private float maxEnergy;
    [SerializeField]
    private float EnergyAddValue;



    public float CurrentHp
    {
        get { return HP; }
        set {
            HP = value;
            if (HP > maxHP)
                HP = maxHP;
            if (HP < 0)
            {
             //ここでゲームオーバー呼んでもいい
                
                HP = 0;
            }
        }
    }

    public float CurrentMaxHp
    {
        get { return maxHP; }
        set { maxHP = value; }
    }
    
    public float CurrentEnergy
    {
        get { return Energy; }
        set {
            Energy = value;
            if (Energy > maxEnergy)
                Energy = maxEnergy;
            if (Energy < 0)
                Energy = 0;
        }
    }

    public float CurrentMaxEnergy
    {
        get { return maxEnergy; }
        set { maxEnergy = value; }
    }


    [Space(20)]

    //プレイヤーのステータス
    [Tooltip("歩行速度")] 
    public float MaxWalkSpeed;
    [Tooltip("加速度(歩行)")]
    public float WalkAcc;
    [Tooltip("走る時の速度")]
    public float MaxRunSpeed;
    [Tooltip("加速度(走る)")]
    public float RunAcc;
    [Tooltip("プレイヤーの通常ジャンプ力")]
    public float JumpPower;
    [Tooltip("プレイヤーの水平方向ジャンプ力")]
    public float JumpHorizonPower = 0.0f;
    [Tooltip("プレイヤーの2段ジャンプ時パワー(通常ジャンプ力基準)")]
    public float SecondJumpMultiplyValue = 1.0f;
    [Tooltip("プレイヤーの2段ジャンプ時水平方向パワー(通常ジャンプ力基準)")]
    public float SecondJumpHorizonPowerMultiplyValue = 1.0f;
    [Tooltip("移動入力によるプレイヤーの回転(Lerp処理)"), Range(0.0f, 1.0f)]
    public float MoveInputRotationSpeed;



    [Space(20)]

    //壁走り関係の数値設定
    private bool isWallHit = false;

    public enum wallSide
    {
        NoWallDitect,
        Right,
        Left
    }

    public wallSide wallStatus = wallSide.NoWallDitect;
    [Tooltip("このレイヤーのオブジェクトにレイが当たった時に壁があると判定する")]
    public LayerMask wallLayers = 0;
    public float WallDitectDistance = 0.4f;

    public float wallRunSpeed = 0.0f;
    public float wallJumpHorizonPower = 0.0f;
    [Tooltip("壁ジャンプ後、次の壁を検知するまでのフレーム数")]
    public int WJtoNextWallTime = 0;
    private int WJtoNextWallTimer = 0;
    private Vector3 WallRunVec;
    private Vector3 WallNormalVec;
    private float WallDistance;
    private int WallRunTimer = 0;


    [Space(20)]
    public int DodgeCoolTime = 240;
    public int DodgeEndTime = 90;
    public int DodgeMutekiTime = 10;
    public int DodgeMutekiStart = 3;
    private int DodgeTimer = 60;
    public float DodgeAddPower = 5.0f;

    public float DodgeUseEnergy = 0.0f;
    public float PerfectDodgeAddEnergy = 10.0f;



    [Space(30)]

    [Tooltip("速度維持率"), Range(0.0f,1.0f), SerializeField]
    private float VelocityHoldRate;

    [Tooltip("空中速度維持率(地上基準)"), Range(0.0f,1.0f), SerializeField]
    private float AirVelocityHoldRate;
    [Tooltip("空中加速率(地上基準)"), Range(0.0f, 1.0f), SerializeField]
    private float AirVelocityAccRate;


    [Tooltip("プレイヤー加速維持率が使用されるスティック入力値"), SerializeField]
    private float RateUseInputValue = 0.1f;

    [Tooltip("現在の速度値")]
    public float playerSpeed;


    [Space(20)]

    //接地判定
    public bool isGround = true;
    public bool FirstJumped = false;
    public bool SecondJumped = false;
    public bool isWallRun = false;
    public bool isDodge = false;
    public bool noDamage = false;

    public bool isAttack = false;




    private Player_Slash playerAttack;
    private GaugeController gaugeController;
    private Line line;


    //Self Objects & Scripts
    private Rigidbody s_Rigidbody;
    private Animator s_Animator;
    private CapsuleCollider s_Collider;


    //InputAction
    private PlayerInput playerInput;
    private InputAction move, jump, attack, run, dodge;

    private bool jumpInputTrigger = false;
    private bool dodgeInputTrigger = false;



    //入力値
    private Vector2 MoveInput;
    //実際に移動で使われる値
    //攻撃や回避の影響でゼロになる
    private Vector2 MoveValue;
    
    private bool RunInput;

    private bool JumpInput;
    private bool DodgeInput;


    private bool attackInputTrigger = false;
    [SerializeField]
    private bool AttackInput;
    private bool OldAttackInput;



    private bool OldJumpInput;
    private bool OldDodgeInput;
    

    void Start()
    {
        PlayerCamera = FindObjectOfType<CameraMove_ByFukuda_3>().gameObject;
        PlayerCameraOrigin = GameObject.Find("PlayerCameraOrigin");
        gaugeController = FindObjectOfType<GaugeController>();

        s_Rigidbody = GetComponent<Rigidbody>();
        s_Animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        s_Collider = GetComponent<CapsuleCollider>();
        playerAttack = GetComponent<Player_Slash>();
        line = GetComponent<Line>();


        HP = maxHP;
        Energy = maxEnergy;

        move = playerInput.actions["Move"];
        jump = playerInput.actions["Jump"];
        dodge = playerInput.actions["Dodge"];
        attack = playerInput.actions["Attack"];

        run = playerInput.actions["Run"];
        jumpInputTrigger = false; 
        dodgeInputTrigger = false;
        attackInputTrigger = false;

        // マウスカーソルを非表示にし、位置を固定
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    //とりあえずで処理フローを書いていく
    //処理が完成していないものには頭に#をつける
    private void FixedUpdate()
    {

        //-------------------------------------------------------------------------------
        //#壁の配置を確認し、カメラの位置を調整、壁判定を取る
        //-------------------------------------------------------------------------------
        float heightOffSet = s_Collider.center.y;

        if (isWallRun)
        {
            Vector3 vec = PlayerCameraOrigin.transform.localPosition;
            if (wallStatus == wallSide.Right)
            {
                vec = new Vector3(-5, heightOffSet, 0);
            }
            if (wallStatus == wallSide.Left)
            {
                vec = new Vector3( 5, heightOffSet, 0);
            }

            PlayerCameraOrigin.transform.localPosition = Vector3.Lerp(PlayerCameraOrigin.transform.localPosition, vec, 0.1f);
        }
        else
        {
            PlayerCameraOrigin.transform.localPosition = new Vector3(0, heightOffSet, 0);
        }

        //ついでにisGroundの更新
        {
            float raycastDistance = 0.2f;
            RaycastHit Hit;
            if (Physics.Raycast(transform.position + transform.up * 0.1f, Vector3.down, out Hit, raycastDistance))
            {
                isGround = true;
                FirstJumped = false;
                SecondJumped = false;
            }
            else
            {
                FirstJumped = true;
                isGround = false;
            }
        }
        //-------------------------------------------------------------------------------
        //プレイヤーの入力値の検知
        //-------------------------------------------------------------------------------

        //入力値の更新
        {
            MoveInput = move.ReadValue<Vector2>();
            MoveValue = MoveInput;
            JumpInput = jump.ReadValue<float>() > 0;

            AttackInput = attack.ReadValue<float>() > 0;
            DodgeInput = dodge.ReadValue<float>() > 0;

            if (isGround)
                RunInput = run.ReadValue<float>() > 0;

            if (isAttack)
            {
                MoveValue = Vector2.zero;
                JumpInput = false;
                DodgeInput = false;
                RunInput = false;
            }

            if (isDodge)
            {
                MoveValue = Vector2.zero;
                JumpInput = false;
                DodgeInput = false;
                isAttack = false;
                RunInput = false;
            }



            if (isWallRun) { 
                RunInput = true;
                //CrouchInput = false;
            }

            attackInputTrigger = false;
            jumpInputTrigger = false;
            dodgeInputTrigger = false;

            if (JumpInput)
            {
                if (!OldJumpInput)
                {
                    jumpInputTrigger = true;
                }
            }

            if (AttackInput)
            {
                if (!OldAttackInput)
                {
                    attackInputTrigger = true;
                }
            }

            if (DodgeInput)
            {
                if (!OldDodgeInput)
                {
                    dodgeInputTrigger = true;
                }
            }


            OldJumpInput = JumpInput;
            OldAttackInput = AttackInput;
            OldDodgeInput = DodgeInput;
        }

        //-------------------------------------------------------------------------------
        //壁判定の更新
        //-------------------------------------------------------------------------------

        WallRunCheck();


        if (WJtoNextWallTimer < WJtoNextWallTime)
            WJtoNextWallTimer++;



        //壁走り開始判定
        if (!isWallRun && isWallHit && !isGround && RunInput && (WJtoNextWallTimer >= WJtoNextWallTime))
        {
            WallRunTimer = 0;
            isWallRun = true;
        }
        if (!isWallHit) 
            isWallRun = false;

        if (isWallRun)
        {
            RunInput = true;
            FirstJumped = false;
            SecondJumped = false;
            s_Rigidbody.useGravity = false;
            WallRunTimer++;
            Vector3 Vec = transform.position - (WallNormalVec.normalized * WallDistance);

            transform.position = Vector3.Lerp(transform.position, Vec, 0.1f);

            s_Rigidbody.velocity = WallRunVec * wallRunSpeed;

            transform.rotation = Quaternion.LookRotation(WallRunVec, Vector3.up);
            Vector3 v = s_Rigidbody.velocity;
            v.y = 0;
            s_Rigidbody.velocity = v;
        }
        else
        {
            s_Rigidbody.useGravity = true;
        }




        //-------------------------------------------------------------------------------
        //カメラの角度/壁の角度からプレイヤーの入力値を調整 プレイヤーを回転
        //-------------------------------------------------------------------------------

        Vector3 MoveOriginVector = Vector3.Scale(PlayerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

        

        //-------------------------------------------------------------------------------
        //プレイヤーの動きをRigidBodyに入力 移動入力に合わせてプレイヤーの回転
        //-------------------------------------------------------------------------------


        float speedHoldRate = VelocityHoldRate;
        float speedAddRate = 1.0f;


        if (!isGround)
        {
            speedHoldRate = AirVelocityHoldRate;
            speedAddRate = AirVelocityAccRate;
        }


        //移動入力処理
        //ここでの計算はチャージ攻撃などでも使えるので
        //入力値から直接計算している

        Vector3 moveForward = MoveOriginVector * MoveInput.y;
        moveForward += PlayerCamera.transform.right * MoveInput.x;
        moveForward = Vector3.Scale(moveForward, new Vector3(1, 0, 1));




        if (!isWallRun && !isDodge && !isAttack)
        {

            //減速処理(ぬるぬる動く性質の温床)
            if (MoveValue.magnitude < RateUseInputValue)
            {
                Vector3 selfSpeed = s_Rigidbody.velocity;
                selfSpeed *= speedHoldRate;
                selfSpeed.y = s_Rigidbody.velocity.y;

                s_Rigidbody.velocity = selfSpeed;
            }

            float maxSpeed = MaxWalkSpeed;
            float accValue = WalkAcc;
            if (RunInput)
            {
                maxSpeed = MaxRunSpeed;
                accValue = RunAcc;
            }
            accValue *= speedAddRate;


            Vector3 MoveVel = moveForward * accValue;
            s_Rigidbody.AddForce(MoveVel,ForceMode.Acceleration);


            //新 速度制限
            {
                Vector3 Vel = Vector3.Scale(s_Rigidbody.velocity, new Vector3(1, 0, 1));

                if (Vel.magnitude > maxSpeed)
                {
                    Vel = Vel.normalized * maxSpeed;
                    Vel.y = s_Rigidbody.velocity.y;
                    s_Rigidbody.velocity = Vel;
                }
            }
        }

        //入力方向へのプレイヤーの回転
        if (isGround)
            transform.rotation = Quaternion.LookRotation(Vector3.Slerp(transform.forward, moveForward, MoveInputRotationSpeed), Vector3.up);

        //------------------------------------------------------------
        //回避
        //------------------------------------------------------------

        if (!isDodge && dodgeInputTrigger && DodgeCoolTime < DodgeTimer)
        {
            isDodge = true;
            DodgeTimer = 0;
            this.CurrentEnergy -= DodgeUseEnergy;
            s_Rigidbody.AddForce(moveForward.normalized * DodgeAddPower, ForceMode.Impulse);
        }

        if (isDodge)
        {

            if (DodgeMutekiStart == DodgeTimer)
                noDamage = true;

            if (DodgeMutekiStart+DodgeMutekiTime == DodgeTimer)
                noDamage = false;

            if (DodgeEndTime == DodgeTimer)
                isDodge = false;


        }


        if (DodgeTimer < DodgeCoolTime + 1)
        {
            DodgeTimer++;
        }
        //------------------------------------------------------------
        //ジャンプ
        //------------------------------------------------------------
        {

            float InputValue = MoveValue.magnitude;

            if (!isWallRun)
            {
                //処理フロー上2段ジャンプの方が先の方がいい
                if (jumpInputTrigger && FirstJumped && !SecondJumped)
                {
                    SecondJumped = true;
                    FirstJumped = true;
                    isGround = false;
                    s_Rigidbody.velocity = new Vector3(0, 0, 0);
                    transform.rotation = Quaternion.LookRotation(moveForward, Vector3.up);
                    s_Rigidbody.AddForce(Vector3.up * JumpPower * SecondJumpMultiplyValue + moveForward * InputValue * JumpHorizonPower * SecondJumpHorizonPowerMultiplyValue, ForceMode.VelocityChange);
                }

                if (jumpInputTrigger && !FirstJumped)
                {
                    FirstJumped = true;
                    isGround = false;
                    s_Rigidbody.velocity = new Vector3(0, 0, 0);
                    transform.rotation = Quaternion.LookRotation(moveForward, Vector3.up); 
                    s_Rigidbody.AddForce(Vector3.up * JumpPower + moveForward * InputValue * JumpHorizonPower, ForceMode.VelocityChange);
                }
            }
            else
            {
                if (jumpInputTrigger)
                {
                    FirstJumped = true;
                    SecondJumped = false;
                    isWallRun = false;
                    WJtoNextWallTimer = 0;
                    WallRunCheck();
                    s_Rigidbody.velocity = new Vector3(0, 0, 0);
                    transform.rotation = Quaternion.LookRotation(moveForward, Vector3.up); 
                    s_Rigidbody.AddForce(Vector3.up * JumpPower + moveForward * InputValue * wallJumpHorizonPower, ForceMode.VelocityChange);
                }
            }
        }

        //--------------------------------------------------------------------------------------
        //エネルギー値の回復
        //--------------------------------------------------------------------------------------

        //速度計測
        playerSpeed = new Vector2(s_Rigidbody.velocity.x, s_Rigidbody.velocity.z).magnitude;
        if (isGround && !isDodge && !isAttack && MoveValue.magnitude > RateUseInputValue)
        {
            CurrentEnergy += playerSpeed * EnergyAddValue;
        }


        //----------------------------------------------------------------------------------
        //体力値の自動回復
        //----------------------------------------------------------------------------------

        


        if (CurrentHp < maxHP)
        {
            if (healTimer > HPHealTime)
            {
                CurrentHp += HPHealValue;
            }

            if (healTimer < HPHealTime + 1)
            {
                healTimer++;
            }
            if (oldHP > CurrentHp)
            {
                healTimer = 0;
            }
        }



        //-------------------------------------------------------------------------------
        //#プレイヤーの攻撃処理
        //-------------------------------------------------------------------------------

        Attack(AttackInput,moveForward);
        

        //-------------------------------------------------------------------------------
        //#アニメーションをアニメーターに登録
        //-------------------------------------------------------------------------------





        //--------------------------------------------------------------------------------
        //＃UIを更新
        //--------------------------------------------------------------------------------

        //エネルギーUIの更新
        //ほんとはUI側スクリプト側から参照してもらうのがいいのかも
        gaugeController.UpdateGauge(Energy, maxEnergy);

    }
    private void Attack(bool val,Vector3 moveVec)
    {
        Vector2 vec = new Vector2(moveVec.x, moveVec.z);
        playerAttack.inputAttackTrigger(val, vec);
    }


    private void WallRunCheck()
    {
        float offsetY = s_Rigidbody.position.y;
        Vector3 origin = transform.position + transform.up * offsetY;
        
        float distance = WallDitectDistance + s_Collider.radius;

        Vector3 RightNormal = Vector3.zero;
        Vector3 LeftNormal = Vector3.zero;

        float RightDist = distance;
        bool isRightHit = false;
        RaycastHit Righthit;

        float LeftDist = distance;
        bool isLeftHit = false;
        RaycastHit Lefthit;

        //右壁チェック
        if (Physics.Raycast(origin, Vector3.Lerp(transform.right, transform.forward, 0.4f), out Righthit, distance, wallLayers, QueryTriggerInteraction.Ignore))
        {
            isRightHit = true;
            RightNormal = Righthit.normal;
            RightDist = Righthit.distance;
        }

        //左壁チェック
        if (Physics.Raycast(origin, Vector3.Lerp(-transform.right,transform.forward, 0.4f), out Lefthit, distance, wallLayers, QueryTriggerInteraction.Ignore))
        {
            isLeftHit = true;
            LeftNormal = Lefthit.normal;
            LeftDist = Lefthit.distance;
        }


        if (!isRightHit && !isLeftHit)
        {
            WallNormalVec = Vector3.zero;
            isWallHit = false;
            wallStatus = wallSide.NoWallDitect;
            return;
        }

        if (isRightHit || isLeftHit)
        {
            bool RightisNear = false;
            if (LeftDist > RightDist)
                RightisNear = true;
            
            isWallHit = true;
            if (RightisNear)
            {
                WallDistance = RightDist;
                WallNormalVec = RightNormal;
                wallStatus = wallSide.Right;
            }
            else
            {
                WallDistance = LeftDist;
                WallNormalVec = LeftNormal;
                wallStatus = wallSide.Left;
            }

            //ここの数値やベクトルをもとに壁に平行なベクトルを求め
            //壁走りの方向を決める
            //https://docs.unity3d.com/ja/2019.4/Manual/ComputingNormalPerpendicularVector.html
            {
                Vector3 A = -WallNormalVec.normalized;
                Vector3 B = Vector3.up.normalized;

                WallRunVec = Vector3.Cross(A,B);

                if (wallStatus == wallSide.Left)
                {
                    WallRunVec = -WallRunVec;
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        Vector3 vec = new Vector3(MoveValue.x, 0, MoveValue.y);
        Gizmos.color = Color.red;

        // ベクトルBを回転行列に変換
        Quaternion rotationQuaternion = Quaternion.Euler(vec);
        // ベクトルAを回転行列で回転
        Vector3 rotatedVector = rotationQuaternion * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + rotatedVector * 2);

        if (!s_Rigidbody)
            s_Rigidbody = GetComponent<Rigidbody>();
        if (!s_Animator)
        s_Animator = GetComponent<Animator>();
        if (!playerInput) 
            playerInput = GetComponent<PlayerInput>();
        if (!s_Collider)
            s_Collider = GetComponent<CapsuleCollider>();

        Gizmos.color = Color.green;
        vec = s_Collider.height * transform.up;
        Gizmos.DrawLine(transform.position + vec, transform.position + vec + transform.up * 0.2f);
    }


    public void AttackStatus(bool value)
    {
        isAttack = value;
    }


}
