using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerControll : MonoBehaviour
{
    //Other Objects & Scripts
    public GameObject PlayerCamera;
    public GameObject PlayerCameraOrigin;

    //forDebug
    //2023/11/04
    //スライディング+しゃがみ要素削除のため不要
    //public GameObject Capsule_forDebug;

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


    //2023/11/04
    //スライディング+しゃがみ要素削除のため不要
    /*
    [Space(20)]
    public Vector3 CrouchCenter;
    public float CrouchHeight;
    [Tooltip("しゃがみ時の速度")]
    public float MaxCrouchSpeed;
    [Tooltip("スライディング速度")]
    public float SlidingSpeed;
    [Tooltip("スライディング維持フレーム数")]
    public int keepSlideTime;
    private int SlidingTimer;


    //スライディング関係の初期値設定
    private Vector3 NormalCenter;
    private float NormalHeight;
    
     
    
    public bool isClouch = false;
    public bool isSliding = false;
    
    
    private bool CrouchInput;
    private bool slideInputTrigger = false;
    private bool OldCrouchInput;
    */

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


    [Space(30)]

    [Tooltip("速度維持率"), Range(0.0f,1.0f)]
    public float VelocityHoldRate;

    [Tooltip("空中速度維持率(地上基準)"), Range(0.0f,1.0f)]
    public float AirVelocityHoldRate;
    [Tooltip("空中加速率(地上基準)"), Range(0.0f, 1.0f)]
    public float AirVelocityAccRate;

    [Tooltip("プレイヤーの回転速度")]
    public float PlayerRotationSpeed;

    [Tooltip("現在の速度値")]
    public float playerSpeed;




    [Space(20)]

    //接地判定
    public bool isGround = true;
    public bool FirstJumped = false;
    public bool SecondJumped = false;


    public bool isWallRun = false;



    private PlayerAttack playerAttack;
    private Line line;


    //Self Objects & Scripts
    private Rigidbody s_Rigidbody;
    private Animator s_Animator;
    private CapsuleCollider s_Collider;


    //InputAction
    private PlayerInput playerInput;
    private InputAction move, jump, attack, run, dodge; // cam, crouch, attack_1, attack_2, attack_3,

    private bool jumpInputTrigger = false;
    private bool dodgeInputTrigger = false;
    
    

    private Vector2 MoveInput;
    //private Vector2 CameraInput;
    private bool RunInput;

    private bool JumpInput;
    private bool DodgeInput;

    //2023/11/04
    //攻撃入力が1つになった為
    //攻撃入力はリワーク

    /*
    private bool attack_1_InputTrigger = false;
    private bool attack_2_InputTrigger = false;
    private bool attack_3_InputTrigger = false;
    private bool Attack_1_Input;
    private bool Attack_2_Input;
    private bool Attack_3_Input;
    private bool OldAttack_1_Input;
    private bool OldAttack_2_Input;
    private bool OldAttack_3_Input;
    */


    private bool attackInputTrigger = false;
    private bool AttackInput;
    private bool OldAttackInput;



    private bool OldJumpInput;
    private bool OldDodgeInput;
    

    void Start()
    { 
        s_Rigidbody = GetComponent<Rigidbody>();
        s_Animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        s_Collider = GetComponent<CapsuleCollider>();
        playerAttack = GetComponent<PlayerAttack>();
        line = GetComponent<Line>();

        /*
        NormalCenter = s_Collider.center;
        NormalHeight = s_Collider.height;
        attack_1 = playerInput.actions["Attack_1"];
        attack_2 = playerInput.actions["Attack_2"];
        attack_3 = playerInput.actions["Attack_3"];
        crouch = playerInput.actions["Crouch"];
        slideInputTrigger = false;
        attack_1_InputTrigger = false;
        */

        move = playerInput.actions["Move"];
        jump = playerInput.actions["Jump"];
        dodge = playerInput.actions["Dodge"];
        attack = playerInput.actions["Attack"];

        run = playerInput.actions["Run"];
        //cam = playerInput.actions["CameraXY"];
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
            //CameraInput = cam.ReadValue<Vector2>();
            JumpInput = jump.ReadValue<float>() > 0;

            AttackInput = attack.ReadValue<float>() > 0;
            /*
            Attack_1_Input = attack_1.ReadValue<float>() > 0;
            Attack_2_Input = attack_2.ReadValue<float>() > 0;
            Attack_3_Input = attack_3.ReadValue<float>() > 0;
            CrouchInput = crouch.ReadValue<float>() > 0;
            attack_1_InputTrigger = false;
            attack_2_InputTrigger = false;
            attack_3_InputTrigger = false;
            slideInputTrigger = false;
            */

            DodgeInput = dodge.ReadValue<float>() > 0;

            if (isGround)
                RunInput = run.ReadValue<float>() > 0;


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

            /*
            if (Attack_1_Input)
            {
                if (!OldAttack_1_Input)
                {
                    attack_1_InputTrigger = true;
                }
            }
            if (Attack_2_Input)
            {
                if (!OldAttack_2_Input)
                {
                    attack_2_InputTrigger = true;
                }
            }
            if (Attack_3_Input)
            {
                if (!OldAttack_3_Input)
                {
                    attack_3_InputTrigger = true;
                }
            }

            if (OldCrouchInput)
            {
                //スライディング中も勝手にしゃがみ入力
                {
                    if (isSliding)
                    {
                        CrouchInput = true;
                    }
                }
                //頭頂部になにかあるときに自動でしゃがみ入力
                {
                    Vector3 vec = (CrouchHeight - 0.1f) * transform.up;
                    RaycastHit Hit;
                    float raycastDistance = NormalHeight - CrouchHeight + 0.2f;
                    if (Physics.Raycast(transform.position + vec, Vector3.up, out Hit, raycastDistance))
                    {
                        CrouchInput = true;
                    }
                }
            }

            if (CrouchInput)
            {
                if (!OldCrouchInput && RunInput && isGround)
                {
                    slideInputTrigger = true;
                }
                RunInput = false;
            }
            */

            if (DodgeInput)
            {
                if (!OldDodgeInput)
                {
                    dodgeInputTrigger = true;
                }
            }


            //走っているときはスティックいっぱいに入力している判定になる
            //10/26 仕様変更により削除
            /*
            if (RunInput)
            {
                Vector2 vec = MoveInput.normalized;
                MoveInput = vec;
            }
            */

            OldJumpInput = JumpInput;
            OldAttackInput = AttackInput;
            /*
            OldAttack_1_Input = Attack_1_Input;
            OldAttack_2_Input = Attack_2_Input;
            OldAttack_3_Input = Attack_3_Input;
            OldCrouchInput = CrouchInput;
            */
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

        /*
        if (!isWallRun)
        {
            transform.rotation = Quaternion.LookRotation(transform.forward + transform.right * CameraInput.x * PlayerRotationSpeed);
        }
        */

        //------------------------------------------------------------
        //しゃがみ&スライディング
        //------------------------------------------------------------
        {
            /*
            {
                if (!isSliding && slideInputTrigger)
                {
                    SlidingTimer = 0;
                    isSliding = true;
                    Vector3 vec = s_Rigidbody.velocity;
                    vec.y = 0;
                    s_Rigidbody.AddForce(vec.normalized * SlidingSpeed, ForceMode.VelocityChange);
                }

                if (isSliding)
                {
                    SlidingTimer++;

                    if (SlidingTimer > keepSlideTime)
                    {
                        isSliding = false;
                    }
                }

                isClouch = false;
                Capsule_forDebug.transform.localPosition = new Vector3(0, 0.9f, 0);
                Capsule_forDebug.transform.localScale = new Vector3(0.3f, 0.9f, 0.3f);

                if (CrouchInput || isSliding)
                {
                    isClouch = true;
                    Capsule_forDebug.transform.localPosition = new Vector3(0, 0.45f, 0);
                    Capsule_forDebug.transform.localScale = new Vector3(0.3f, 0.45f, 0.3f);
                }

                if (isClouch)
                {
                    s_Collider.center = CrouchCenter;
                    s_Collider.height = CrouchHeight;
                }
                else
                {
                    s_Collider.center = NormalCenter;
                    s_Collider.height = NormalHeight;
                }
            }
            */
        }
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

        //減速処理(ぬるぬる動く性質の温床)
        if (MoveInput.magnitude < 0.01f)
        {
            Vector3 selfSpeed = s_Rigidbody.velocity;
            selfSpeed *= speedHoldRate;
            selfSpeed.y = s_Rigidbody.velocity.y;

            s_Rigidbody.velocity = selfSpeed;
        }

        //移動入力処理

        Vector3 moveForward = MoveOriginVector * MoveInput.y;
        moveForward += PlayerCamera.transform.right * MoveInput.x;
        moveForward = Vector3.Scale(moveForward, new Vector3(1, 0, 1));


        if (!isWallRun)
        {

            Vector3 Vel = Vector3.Scale(s_Rigidbody.velocity, new Vector3(1, 0, 1));

            float maxSpeed = MaxWalkSpeed;
            float accValue = WalkAcc;
            if (RunInput)
            {
                maxSpeed = MaxRunSpeed;
                accValue = RunAcc;
            }
            /*
            if (CrouchInput)
            {
                maxSpeed = MaxCrouchSpeed;
            }
            */
            accValue *= speedAddRate;

            //加速数値の調整(最高速度を超えないようにする)
            //↑この処理のせいでかなりごちゃついてしまった
            float MaxaddSpeed = Mathf.Clamp(maxSpeed - Vel.magnitude, 0, maxSpeed);

            if (accValue >= MaxaddSpeed)
            {
                accValue = Mathf.Clamp(accValue - moveForward.magnitude, 0, accValue);
            }

            Vector3 MoveVel = moveForward * accValue;

            s_Rigidbody.AddForce(MoveVel,ForceMode.Acceleration);
        }

        //入力方向へのプレイヤーの回転
        if (isGround)
            transform.rotation = Quaternion.LookRotation(Vector3.Slerp(transform.forward, moveForward, MoveInputRotationSpeed), Vector3.up);

        //------------------------------------------------------------
        //ジャンプ
        //------------------------------------------------------------
        {

            float InputValue = MoveInput.magnitude;

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


        //速度計測
        playerSpeed = new Vector2(s_Rigidbody.velocity.x, s_Rigidbody.velocity.z).magnitude;


        //-------------------------------------------------------------------------------
        //#プレイヤーの攻撃処理
        //-------------------------------------------------------------------------------

        if (attackInputTrigger)
        {
            Attack();
        }
        if (AttackInput)
        {
            
        }


        /*
        if(Attack_2_Input)
        {
            HikiyoseAttack();
        }
       
        if(Attack_3_Input)
        {
            LineAttack();
        }
        */
        //-------------------------------------------------------------------------------
        //#アニメーションをアニメーターに登録
        //-------------------------------------------------------------------------------







    }
    private void Attack()
    {
        playerAttack.isAttack = true;
    }

    private void HikiyoseAttack()
    {
        //playerAttack.isAttack2 = true;
    }

    private void LineAttack()
    {
        //line.isAttack3 = true;
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
        Vector3 vec = new Vector3(MoveInput.x, 0, MoveInput.y);
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
}
