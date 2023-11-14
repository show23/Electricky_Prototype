using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Player_Slash))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerControll : MonoBehaviour
{


    [System.Serializable]
    public struct PlayerBasicStatus
    {
        //ゲーム内で変更される系のステータス
        [Tooltip("体力")]
        public float HP;
        [Tooltip("最大体力")]
        public float maxHP;


        [Tooltip("体力自動回復までの時間")]
        public int HPHealTime;
        [Tooltip("自動回復の強さ(1フレーム当たり)")]
        public float HPHealValue;


        [Tooltip("電力")]
        public float Energy;
        [Tooltip("最大電力")]
        public float maxEnergy;

        [Tooltip("移動での電力回復割合(速度と掛ける)")]
        public float EnergyAddValue;
        
        [HideInInspector]
        public int healTimer;
        [HideInInspector]
        public float oldHP;
    }


    [Header("プレイヤーのステータス設定")]
    [SerializeField]
    private PlayerBasicStatus _PlayerBasicStatus;


    public float CurrentHp
    {
        get { return _PlayerBasicStatus.HP; }
        set {

            //ここに回避をぶち込む
            if (_PlayerBasicStatus.HP > value && noDamage)
            {
                //複数攻撃を受けたとしても エネルギーの回復は1度だけ
                if (!_DodgeStatus.isDodgePerfectHappened)
                    CurrentEnergy += _DodgeStatus.PerfectDodgeAddEnergy;
                
                _DodgeStatus.isDodgePerfectHappened = true;
                return;
            }

            _PlayerBasicStatus.HP = value;
            if (_PlayerBasicStatus.HP > _PlayerBasicStatus.maxHP)
                _PlayerBasicStatus.HP = _PlayerBasicStatus.maxHP;
            if (_PlayerBasicStatus.HP < 0)
            {
                //ここでゲームオーバー呼んでもいい

                _PlayerBasicStatus.HP = 0;
            }
        }
    }

    public float CurrentMaxHp
    {
        get { return _PlayerBasicStatus.maxHP; }
        set {
            float oldmaxhp = _PlayerBasicStatus.maxHP;
            _PlayerBasicStatus.maxHP = value;

            //最大体力が増えた場合は増えた分体力も増える
            if (oldmaxhp < value)
            {
                CurrentHp += value - oldmaxhp;
            } 
        }
    }
    
    public float CurrentEnergy
    {
        get { return _PlayerBasicStatus.Energy; }
        set {
            _PlayerBasicStatus.Energy = value;
            if (_PlayerBasicStatus.Energy > _PlayerBasicStatus.maxEnergy)
                _PlayerBasicStatus.Energy = _PlayerBasicStatus.maxEnergy;
            if (_PlayerBasicStatus.Energy < 0)
                _PlayerBasicStatus.Energy = 0;
        }
    }

    public float CurrentMaxEnergy
    {
        get { return _PlayerBasicStatus.maxEnergy; }
        set { _PlayerBasicStatus.maxEnergy = value; }
    }

    public float CurrentRotationSpeed
    {
        get { return _PlayerMoveStatus.MoveInputRotationSpeed; }
        set { _PlayerMoveStatus.MoveInputRotationSpeed = value; }
    }


    //プレイヤーのステータス
    [System.Serializable]
    public struct PlayerMoveStatus
    {
        [Tooltip("最大歩行速度")]
        public float MaxWalkSpeed;
        [Tooltip("歩行加速度")]
        public float WalkAcc;
        [Tooltip("最大走り速度")]
        public float MaxRunSpeed;
        [Tooltip("走り加速度")]
        public float RunAcc;
        
        [Tooltip("移動入力による回転(Lerp処理)"), Range(0.0f, 1.0f)]
        public float MoveInputRotationSpeed;

        [Tooltip("上方向ジャンプ力")]
        public float JumpPower;
        [Tooltip("水平方向ジャンプ力")]
        public float JumpHorizonPower;
        [Tooltip("2段ジャンプ時パワー(通常ジャンプ力基準)")]
        public float SecondJumpMultiplyValue;
        [Tooltip("2段ジャンプ時水平方向パワー(通常ジャンプ力基準)")]
        public float SecondJumpHorizonPowerMultiplyValue;


        [Tooltip("速度維持率"), Range(0.0f, 1.0f)]
        public float VelocityHoldRate;
        [Tooltip("空中速度維持率(地上基準)"), Range(0.0f, 1.0f)]
        public float AirVelocityHoldRate;
        [Tooltip("空中加速率(地上基準)"), Range(0.0f, 1.0f)]
        public float AirVelocityAccRate;
        [Tooltip("移動スティック入力のデッドゾーン値")]
        public float UseInputValue;
    }

    [SerializeField]
    private PlayerMoveStatus _PlayerMoveStatus;

    //壁走り関係の数値設定

    public enum wallSide
    {
        NoWallDitect,
        Right,
        Left
    }


    [System.Serializable]
    public struct WallRunStatus
    {
        [Tooltip("壁走り判定のレイヤー設定")]
        public LayerMask wallLayers;

        [Tooltip("壁判定を取る距離")]
        public float WallDitectDistance;

        [Tooltip("壁走り速度")]
        public float wallRunSpeed;
        [Tooltip("壁ジャンプ水平方向強さ")]
        public float wallJumpHorizonPower;
        [Tooltip("壁ジャンプ後、次の壁を認識するまでのフレーム数")]
        public int WJtoNextWallTime;

        //この下は非表示
        [HideInInspector]
        public wallSide wallStatus;
        [HideInInspector]
        public int WJtoNextWallTimer;
        [HideInInspector]
        public Vector3 WallRunVec;
        [HideInInspector]
        public Vector3 WallNormalVec;
        [HideInInspector]
        public float WallDistance;
        [HideInInspector]
        public int WallRunTimer;
    }

    [SerializeField]
    private WallRunStatus _WallRunStatus;

    [System.Serializable]
    public struct DodgeStatus
    {
        [Tooltip("回避クールタイム")]
        public int DodgeCoolTime;
        [Tooltip("回避終了時間")]
        public int DodgeEndTime;
        [Tooltip("無敵時間長さ")]
        public int DodgeMutekiLength;
        [Tooltip("無敵時間開始フレーム")]
        public int DodgeMutekiStart;

        [Tooltip("回避発動時の加算速度")]
        public float DodgeAddPower;
        [Tooltip("回避の電力消費量")]
        public float DodgeUseEnergy;
        [Tooltip("回避成功時の加算エネルギー量")]
        public float PerfectDodgeAddEnergy;
        
        [HideInInspector]
        public int DodgeTimer;
        [HideInInspector]
        public bool isDodgePerfectHappened;
    }



    [SerializeField]
    private DodgeStatus _DodgeStatus;

    [Space(10)]

    [Header("以下 デバッグ用の状況表示 (変更不能)")]
    public float playerSpeed;
    //接地判定
    public bool isGround = true;
    public bool FirstJumped = false;
    public bool SecondJumped = false;
    public bool isWallRun = false;
    public bool isWallHit = false;
    public bool isDodge = false;
    public bool noDamage = false;
    public bool isAttack = false;
    public bool isChargeAttack = false;



    //Other Objects & Scripts
    private GameObject PlayerCamera;
    private GameObject PlayerCameraOrigin;

    private Player_Slash playerAttack;
    private GaugeController gaugeController;


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
    private bool AttackInput;
    private bool OldAttackInput;



    private bool OldJumpInput;
    private bool OldDodgeInput;
    

    void Start()
    {
        _WallRunStatus.wallStatus = wallSide.NoWallDitect;
        _WallRunStatus.WJtoNextWallTimer = 0;
        _WallRunStatus.WallRunTimer = 0;

        PlayerCamera = FindObjectOfType<CameraMove_ByFukuda_3>().gameObject;
        PlayerCameraOrigin = GameObject.Find("PlayerCameraOrigin");
        gaugeController = FindObjectOfType<GaugeController>();

        s_Rigidbody = GetComponent<Rigidbody>();
        s_Animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        s_Collider = GetComponent<CapsuleCollider>();
        playerAttack = GetComponent<Player_Slash>();


        _PlayerBasicStatus.HP = _PlayerBasicStatus.maxHP;
        _PlayerBasicStatus.Energy = _PlayerBasicStatus.maxEnergy;

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
            if (_WallRunStatus.wallStatus == wallSide.Right)
            {
                vec = new Vector3(-5, heightOffSet, 0);
            }
            if (_WallRunStatus.wallStatus == wallSide.Left)
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
                if (!isChargeAttack)
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


        if (_WallRunStatus.WJtoNextWallTimer < _WallRunStatus.WJtoNextWallTime)
            _WallRunStatus.WJtoNextWallTimer++;



        //壁走り開始判定
        if (!isWallRun && isWallHit && !isGround && RunInput &&
            (_WallRunStatus.WJtoNextWallTimer >= _WallRunStatus.WJtoNextWallTime))
        {
            _WallRunStatus.WallRunTimer = 0;
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
            _WallRunStatus.WallRunTimer++;
            Vector3 Vec = transform.position - (_WallRunStatus.WallNormalVec.normalized * _WallRunStatus.WallDistance);

            transform.position = Vector3.Lerp(transform.position, Vec, 0.1f);

            s_Rigidbody.velocity 
                = _WallRunStatus.WallRunVec * _WallRunStatus.wallRunSpeed;

            transform.rotation = Quaternion.LookRotation(_WallRunStatus.WallRunVec, Vector3.up);
            Vector3 v = s_Rigidbody.velocity;
            v.y = 0;
            s_Rigidbody.velocity = v;
        }
        else
        {
            s_Rigidbody.useGravity = true;
        }


        //-------------------------------------------------------------------------------
        //カメラの角度/壁の角度からプレイヤーの入力値を調整
        //-------------------------------------------------------------------------------

        Vector3 MoveOriginVector = 
            Vector3.Scale(PlayerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;


        //移動入力処理
        //ここでの計算はチャージ攻撃などでも使えるので
        //入力値から直接計算している

        Vector3 moveForward = MoveOriginVector;
        if (_PlayerMoveStatus.UseInputValue < MoveInput.magnitude)
        {
            moveForward *= MoveInput.y;
            moveForward += PlayerCamera.transform.right * MoveInput.x;
        }

        moveForward = Vector3.Scale(moveForward, new Vector3(1, 0, 1)).normalized;


        //-------------------------------------------------------------------------------
        //プレイヤーの動きをRigidBodyに入力
        //-------------------------------------------------------------------------------



        if (!isWallRun && !isDodge && !isAttack)
        {
            float speedHoldRate = _PlayerMoveStatus.VelocityHoldRate;
            float speedAddRate = 1.0f;


            if (!isGround)
            {
                speedHoldRate = _PlayerMoveStatus.AirVelocityHoldRate;
                speedAddRate = _PlayerMoveStatus.AirVelocityAccRate;
            }

            //減速処理(ぬるぬる動く性質の温床)
            if (MoveValue.magnitude < _PlayerMoveStatus.UseInputValue)
            {
                Vector3 selfSpeed = s_Rigidbody.velocity;
                selfSpeed *= speedHoldRate;
                selfSpeed.y = s_Rigidbody.velocity.y;

                s_Rigidbody.velocity = selfSpeed;
            }

            float maxSpeed = _PlayerMoveStatus.MaxWalkSpeed;
            float accValue = _PlayerMoveStatus.WalkAcc;
            if (RunInput)
            {
                maxSpeed = _PlayerMoveStatus.MaxRunSpeed;
                accValue = _PlayerMoveStatus.RunAcc;
            }
            accValue *= speedAddRate;


            Vector3 MoveVel = moveForward * MoveValue.magnitude * accValue;
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
        if (isGround && _PlayerMoveStatus.UseInputValue < MoveInput.magnitude)
            transform.rotation = 
                Quaternion.LookRotation(
                    Vector3.Slerp(transform.forward,
                    moveForward,
                    _PlayerMoveStatus.MoveInputRotationSpeed),
                    Vector3.up);

        //------------------------------------------------------------
        //回避
        //------------------------------------------------------------

        if (!isDodge && dodgeInputTrigger && _DodgeStatus.DodgeCoolTime < _DodgeStatus.DodgeTimer)
        {
            isDodge = true;
            _DodgeStatus.DodgeTimer = 0;
            _DodgeStatus.isDodgePerfectHappened = false;
            this.CurrentEnergy -= _DodgeStatus.DodgeUseEnergy;
            s_Rigidbody.velocity =
                transform.forward * _DodgeStatus.DodgeAddPower;
        }

        if (isDodge)
        {
            if (_DodgeStatus.DodgeMutekiStart == _DodgeStatus.DodgeTimer)
                noDamage = true;

            if (_DodgeStatus.DodgeMutekiStart + _DodgeStatus.DodgeMutekiLength == _DodgeStatus.DodgeTimer)
                noDamage = false;

            if (_DodgeStatus.DodgeEndTime == _DodgeStatus.DodgeTimer)
                isDodge = false;
        }


        if (_DodgeStatus.DodgeTimer < _DodgeStatus.DodgeCoolTime + 1)
        {
            _DodgeStatus.DodgeTimer++;
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
                    //transform.rotation = Quaternion.LookRotation(moveForward, Vector3.up);
                    s_Rigidbody.AddForce(
                        Vector3.up * _PlayerMoveStatus.JumpPower 
                        * _PlayerMoveStatus.SecondJumpMultiplyValue 
                        + moveForward.normalized * InputValue 
                        *_PlayerMoveStatus.JumpHorizonPower 
                        * _PlayerMoveStatus.SecondJumpHorizonPowerMultiplyValue, 
                        ForceMode.VelocityChange);
                }

                if (jumpInputTrigger && !FirstJumped)
                {
                    FirstJumped = true;
                    isGround = false;
                    s_Rigidbody.velocity = new Vector3(0, 0, 0);
                    //transform.rotation = Quaternion.LookRotation(moveForward, Vector3.up); 
                    s_Rigidbody.AddForce(
                        Vector3.up * _PlayerMoveStatus.JumpPower 
                        + moveForward.normalized * InputValue * _PlayerMoveStatus.JumpHorizonPower,
                        ForceMode.VelocityChange);
                }
            }
            else
            {
                if (jumpInputTrigger)
                {
                    FirstJumped = true;
                    SecondJumped = false;
                    isWallRun = false;
                    _WallRunStatus.WJtoNextWallTimer = 0;
                    WallRunCheck();
                    s_Rigidbody.velocity = new Vector3(0, 0, 0);
                    transform.rotation = Quaternion.LookRotation(moveForward, Vector3.up); 
                    s_Rigidbody.AddForce(
                        Vector3.up * _PlayerMoveStatus.JumpPower 
                        + moveForward.normalized * InputValue * _WallRunStatus.wallJumpHorizonPower,
                        ForceMode.VelocityChange);
                }
            }
        }

        //--------------------------------------------------------------------------------------
        //エネルギー値の回復
        //--------------------------------------------------------------------------------------

        //速度計測
        playerSpeed = new Vector2(s_Rigidbody.velocity.x, s_Rigidbody.velocity.z).magnitude;
        if (isGround && !isDodge && !isAttack && MoveValue.magnitude > _PlayerMoveStatus.UseInputValue)
        {
            CurrentEnergy += playerSpeed * _PlayerBasicStatus.EnergyAddValue;
        }


        //----------------------------------------------------------------------------------
        //体力値の自動回復
        //----------------------------------------------------------------------------------

        


        if (CurrentHp < _PlayerBasicStatus.maxHP)
        {
            if (_PlayerBasicStatus.healTimer > _PlayerBasicStatus.HPHealTime)
            {
                CurrentHp += _PlayerBasicStatus.HPHealValue;
            }

            if (_PlayerBasicStatus.healTimer < _PlayerBasicStatus.HPHealTime + 1)
            {
                _PlayerBasicStatus.healTimer++;
            }
            if (_PlayerBasicStatus.oldHP > CurrentHp)
            {
                _PlayerBasicStatus.healTimer = 0;
            }
        }



        //-------------------------------------------------------------------------------
        //#プレイヤーの攻撃処理
        //-------------------------------------------------------------------------------

        Attack(AttackInput);
        

        //-------------------------------------------------------------------------------
        //#アニメーションをアニメーターに登録
        //-------------------------------------------------------------------------------





        //--------------------------------------------------------------------------------
        //＃UIを更新
        //--------------------------------------------------------------------------------

        //エネルギーUIの更新
        //ほんとはUI側スクリプト側から参照してもらうのがいいのかも
        gaugeController.UpdateGauge(_PlayerBasicStatus.Energy, _PlayerBasicStatus.maxEnergy);

    }
    private void Attack(bool val)
    {
        playerAttack.inputAttackTrigger(val);
    }


    private void WallRunCheck()
    {
        float offsetY = s_Rigidbody.position.y;
        Vector3 origin = transform.position + transform.up * offsetY;
        
        float distance = _WallRunStatus.WallDitectDistance + s_Collider.radius;

        Vector3 RightNormal = Vector3.zero;
        Vector3 LeftNormal = Vector3.zero;

        float RightDist = distance;
        bool isRightHit = false;
        RaycastHit Righthit;

        float LeftDist = distance;
        bool isLeftHit = false;
        RaycastHit Lefthit;

        //右壁チェック
        if (Physics.Raycast(origin, Vector3.Lerp(transform.right, transform.forward, 0.4f),
            out Righthit, distance, _WallRunStatus.wallLayers, QueryTriggerInteraction.Ignore))
        {
            isRightHit = true;
            RightNormal = Righthit.normal;
            RightDist = Righthit.distance;
        }

        //左壁チェック
        if (Physics.Raycast(origin, Vector3.Lerp(-transform.right,transform.forward, 0.4f), 
            out Lefthit, distance, _WallRunStatus.wallLayers, QueryTriggerInteraction.Ignore))
        {
            isLeftHit = true;
            LeftNormal = Lefthit.normal;
            LeftDist = Lefthit.distance;
        }


        if (!isRightHit && !isLeftHit)
        {
            _WallRunStatus.WallNormalVec = Vector3.zero;
            isWallHit = false;
            _WallRunStatus.wallStatus = wallSide.NoWallDitect;
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
                _WallRunStatus.WallDistance = RightDist;
                _WallRunStatus.WallNormalVec = RightNormal;
                _WallRunStatus.wallStatus = wallSide.Right;
            }
            else
            {
                _WallRunStatus.WallDistance = LeftDist;
                _WallRunStatus.WallNormalVec = LeftNormal;
                _WallRunStatus.wallStatus = wallSide.Left;
            }

            //ここの数値やベクトルをもとに壁に平行なベクトルを求め
            //壁走りの方向を決める
            //https://docs.unity3d.com/ja/2019.4/Manual/ComputingNormalPerpendicularVector.html
            {
                Vector3 A = -_WallRunStatus.WallNormalVec.normalized;
                Vector3 B = Vector3.up.normalized;

                _WallRunStatus.WallRunVec = Vector3.Cross(A,B);

                if (_WallRunStatus.wallStatus == wallSide.Left)
                {
                    _WallRunStatus.WallRunVec *= -1;
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


    public void AttackStatus(bool value, bool charge)
    {
        isAttack = value;
        isChargeAttack = charge;
    }


}
