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
    public GameObject Capsule_forDebug;


    //Self Objects & Scripts
    private Rigidbody s_Rigidbody;
    private Animator s_Animator;
    private CapsuleCollider s_Collider;


    //InputAction
    private PlayerInput playerInput;
    private InputAction move,jump,crouch,attack_1, attack_2, attack_3, run,cam;

    private bool jumpInputTrigger = false;
    private bool attack_1_InputTrigger = false;
    private bool attack_2_InputTrigger = false;
    private bool attack_3_InputTrigger = false;
    private bool slideInputTrigger = false;


    private Vector2 MoveInput;
    private Vector2 CameraInput;
    private bool RunInput; 

    private bool JumpInput;
    private bool Attack_1_Input;
    private bool Attack_2_Input;
    private bool Attack_3_Input;
    private bool CrouchInput;
    
    private bool OldJumpInput;
    private bool OldAttack_1_Input;
    private bool OldAttack_2_Input;
    private bool OldAttack_3_Input;
    private bool OldCrouchInput;

    //プレイヤーのステータス
    [Tooltip("歩行速度")] 
    public float MaxWalkSpeed;
    [Tooltip("加速度(歩行)")]
    public float WalkAcc;
    [Tooltip("走る時の速度")]
    public float MaxRunSpeed;
    [Tooltip("加速度(走る)")]
    public float RunAcc;
    [Tooltip("しゃがみ時の速度")]
    public float MaxCrouchSpeed;


    [Tooltip("壁走りを強制終了する速度")]
    public float WallRunStopSpeedValue;
    private GameObject WallObj;


    [Tooltip("速度維持率"), Range(0.97f,1.0f)]
    public float VelocityHoldRate;

    [Tooltip("空中速度維持率(地上基準)"), Range(0.97f,1.0f)]
    public float AirVelocityHoldRate;
    [Tooltip("空中加速率(地上基準)"), Range(0.0f, 1.0f)]
    public float AirVelocityAccRate;

    [Tooltip("プレイヤーの通常ジャンプ力")]
    public float PlayerJumpPower;
    [Tooltip("プレイヤーの2段ジャンプ時パワー(通常ジャンプ力基準)")]
    public float PlayerSecondJumpMultiplyValue;

    [Tooltip("プレイヤーの回転速度")]
    public float PlayerRotationSpeed;

    [Tooltip("スライディング速度")]
    public float SlidingSpeed;

    [Tooltip("スライディング維持時間")]
    public float keepSlideTime;
    private float SlidingTimer;



    [Tooltip("プレイヤーの現在の速度値(デバッグ用)"),SerializeField]
    private float playerSpeed;


    //壁走り関係の数値設定
    public bool isWallHit = false;

    public enum wallSide
    {
        NoWallDitect,
        Right,
        Left
    }

    public wallSide wallStatus = wallSide.NoWallDitect;
    [Tooltip("このレイヤーのオブジェクトにレイが当たった時に壁があると判定する")]
    public LayerMask wallLayers = 0;
    private Vector3 WallRunVec;
    private Vector3 WallNormalVec;
    private float WallDistance;
    


    //接地判定
    public bool isGround = true;
    public bool FirstJumped = false;
    public bool SecondJumped = false;

    public bool isClouch = false;
    public bool isSliding = false;

    public bool isWallRun = false;


    //スライディング関係の数値設定
    private Vector3 NormalCenter;
    private float NormalHeight;

    public Vector3 CrouchCenter;
    public float CrouchHeight;


    private PlayerAttack playerAttack;
    private Line line;

    void Start()
    { 
        s_Rigidbody = GetComponent<Rigidbody>();
        s_Animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        s_Collider = GetComponent<CapsuleCollider>();
        playerAttack = GetComponent<PlayerAttack>();
        line = GetComponent<Line>();
        NormalCenter = s_Collider.center;
        NormalHeight = s_Collider.height;

        move = playerInput.actions["Move"];
        jump = playerInput.actions["Jump"];
        crouch = playerInput.actions["Crouch"];
        attack_1 = playerInput.actions["Attack_1"];
        attack_2 = playerInput.actions["Attack_2"];
        attack_3 = playerInput.actions["Attack_3"];
        run = playerInput.actions["Run"];
        cam = playerInput.actions["CameraXY"];
        jumpInputTrigger = false; 
        attack_1_InputTrigger = false;
        slideInputTrigger = false;

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
        PlayerCameraOrigin.transform.localPosition = new Vector3(0, heightOffSet, 0);
        if (isWallRun)
        {
            if (wallStatus == wallSide.Right)
            {
                PlayerCameraOrigin.transform.localPosition = new Vector3(-3, heightOffSet, 0);
            }
            if (wallStatus == wallSide.Left)
            {
                PlayerCameraOrigin.transform.localPosition = new Vector3( 3, heightOffSet, 0);
            }
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
                //WallRunEnd();
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
            CameraInput = cam.ReadValue<Vector2>();
            JumpInput = jump.ReadValue<float>() > 0;
            Attack_1_Input = attack_1.ReadValue<float>() > 0;
            Attack_2_Input = attack_2.ReadValue<float>() > 0;
            Attack_3_Input = attack_3.ReadValue<float>() > 0;
            CrouchInput = crouch.ReadValue<float>() > 0;

            if (isGround)
                RunInput = run.ReadValue<float>() > 0;


            if (isWallRun) { 
                RunInput = true;
                CrouchInput = false;
            }

            jumpInputTrigger = false;
            attack_1_InputTrigger = false;
            attack_2_InputTrigger = false;
            attack_3_InputTrigger = false;
            slideInputTrigger = false;

            if (JumpInput)
            {
                if (!OldJumpInput)
                {
                    jumpInputTrigger = true;
                    CrouchInput = false;
                }
            }

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


            bool HeadHit = false;
            if (OldCrouchInput)
            {
                //頭頂部になにかあるときに自動でしゃがみ入力
                {
                    Vector3 vec = (CrouchHeight - 0.1f) * transform.up;
                    RaycastHit Hit;
                    float raycastDistance = NormalHeight - CrouchHeight + 0.2f;
                    if (Physics.Raycast(transform.position + vec, Vector3.up, out Hit, raycastDistance))
                    {
                        CrouchInput = true;
                        HeadHit = true;
                    }
                }
            }




            if (CrouchInput)
            {
                if (!OldCrouchInput && RunInput)
                {
                    slideInputTrigger = true;
                }
                RunInput = false;
            }


            //走っているときはスティックいっぱいに入力している判定になる
            //10/26 仕様変更により削除
            //if (RunInput)
            //{
            //    Vector2 vec = MoveInput.normalized;
            //    MoveInput = vec;
            //}

            OldJumpInput = JumpInput;
            OldAttack_1_Input = Attack_1_Input;
            OldAttack_2_Input = Attack_2_Input;
            OldAttack_3_Input = Attack_3_Input;
            OldCrouchInput = CrouchInput;
        }

        //-------------------------------------------------------------------------------
        //壁判定の更新
        //-------------------------------------------------------------------------------

        WallRunCheck();


        //壁走り開始判定
        if (!isWallRun && isWallHit && !isGround && RunInput)
            isWallRun = true;
        
        if (!isWallHit) 
            isWallRun = false;

        if (isWallRun)
        {
            RunInput = true;
            FirstJumped = false;
            SecondJumped = false;
            s_Rigidbody.useGravity = false;

            transform.position += -WallNormalVec.normalized * WallDistance;

            s_Rigidbody.velocity = WallRunVec * MaxRunSpeed;

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
        
        if (!isWallRun)
        {
            transform.rotation = Quaternion.LookRotation(transform.forward + transform.right * CameraInput.x * PlayerRotationSpeed);
        }

        //------------------------------------------------------------
        //しゃがみ&スライディング
        //------------------------------------------------------------

        {
            if (!isSliding && slideInputTrigger)
            {
                SlidingTimer = 0;
                isSliding = true;
                Vector3 vec = s_Rigidbody.velocity;
                vec.y = 0;
                s_Rigidbody.AddForce(vec.normalized * SlidingSpeed, ForceMode.Impulse);
            }

            if (isSliding)
            {
                SlidingTimer += Time.deltaTime;

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

        //-------------------------------------------------------------------------------
        //プレイヤーの動きをRigidBodyに入力
        //-------------------------------------------------------------------------------

        
        float speedHoldRate = VelocityHoldRate;
        float speedAddRate = 1.0f;


        if (!isGround)
        {
            speedHoldRate = AirVelocityHoldRate;
            speedAddRate = AirVelocityAccRate;
        }

        //減速処理(ぬるぬる動く性質の温床)
        {
            Vector3 selfSpeed = s_Rigidbody.velocity;
            selfSpeed *= speedHoldRate;
            selfSpeed.y = s_Rigidbody.velocity.y;

            s_Rigidbody.velocity = selfSpeed;
        }

        //移動入力処理
        if (!isWallRun)
        {
            Vector3 moveForward = MoveOriginVector * MoveInput.y;

            if (!isWallRun)
                moveForward += PlayerCamera.transform.right * MoveInput.x;


            moveForward = Vector3.Scale(moveForward, new Vector3(1, 0, 1));

            Vector3 Vel = Vector3.Scale(s_Rigidbody.velocity, new Vector3(1, 0, 1));

            float maxSpeed = MaxWalkSpeed;
            float accValue = WalkAcc;
            if (RunInput)
            {
                maxSpeed = MaxRunSpeed;
                accValue = RunAcc;
            }
            if (CrouchInput)
            {
                maxSpeed = MaxCrouchSpeed;
            }

            accValue *= speedAddRate;

            //加速数値の調整(最高速度を超えないようにする)
            //↑この処理のせいでかなりごちゃついてしまった
            float MaxaddSpeed = Mathf.Clamp(maxSpeed - Vel.magnitude, 0, maxSpeed);

            if (accValue >= MaxaddSpeed)
            {
                accValue = Mathf.Clamp(accValue - moveForward.magnitude, 0, accValue);
            }

            Vector3 MoveVel = moveForward * accValue;

            s_Rigidbody.AddForce(MoveVel);
        }

        //------------------------------------------------------------
        //ジャンプ
        //------------------------------------------------------------
        if (!isWallRun)
        {
            //処理フロー上2段ジャンプの方が先の方がいい
            if (jumpInputTrigger && FirstJumped && !SecondJumped)
            {
                SecondJumped = true;
                FirstJumped = true;
                isGround = false;
                s_Rigidbody.AddForce(Vector3.up * PlayerJumpPower * PlayerSecondJumpMultiplyValue, ForceMode.Impulse);
            }

            if (jumpInputTrigger && !FirstJumped)
            {
                FirstJumped = true;
                isGround = false;
                s_Rigidbody.AddForce(Vector3.up * PlayerJumpPower, ForceMode.Impulse);
            }
        }
        else
        {
            if (jumpInputTrigger)
            {
                FirstJumped = true;
                SecondJumped = false;
                isWallRun = false;

                Vector3 A = transform.forward;
                Vector3 B = new Vector3(MoveInput.x, 0, MoveInput.y);


                Vector3 moveVel = new Vector3(MoveInput.x, 0, MoveInput.y) + transform.forward;
                moveVel.y = 0;
                moveVel = moveVel.normalized;
                
                s_Rigidbody.velocity = new Vector3(0, 0, 0);
                s_Rigidbody.AddForce(Vector3.up * PlayerJumpPower + moveVel * MaxRunSpeed, ForceMode.Impulse);
            }
        }


        //---------------------------------------------------------
        //壁走りを維持するのかの判定()
        //---------------------------------------------------------

        playerSpeed = new Vector2(s_Rigidbody.velocity.x, s_Rigidbody.velocity.z).magnitude;

        if (isWallRun)
        {
            if (playerSpeed < WallRunStopSpeedValue)
            {
                //Debug.Log("WallRun : Stopped.tooslow(" + playerSpeed + ")");
                //WallRunEnd();
            }
        }

        //-------------------------------------------------------------------------------
        //#プレイヤーの攻撃処理
        //-------------------------------------------------------------------------------

        if (attack_1_InputTrigger)
        {
            Attack();
        }
        if (Attack_1_Input)
        {
            
        }
        if(Attack_2_Input)
        {
            HikiyoseAttack();
        }
       
        if(Attack_3_Input)
        {
            LineAttack();
        }

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
        playerAttack.isAttack2 = true;
    }

    private void LineAttack()
    {
        line.isAttack3 = true;
    }


    private void WallRunCheck()
    {
        float offsetY = s_Rigidbody.position.y;
        Vector3 origin = transform.position + transform.up * offsetY;
        float distance = 0.2f + s_Collider.radius;

        Vector3 RightNormal = Vector3.zero;
        Vector3 LeftNormal = Vector3.zero;

        float RightDist = distance;
        bool isRightHit = false;
        RaycastHit Righthit;

        float LeftDist = distance;
        bool isLeftHit = false;
        RaycastHit Lefthit;

        //右壁チェック
        if (Physics.Raycast(origin, transform.right, out Righthit, distance, wallLayers, QueryTriggerInteraction.Ignore))
        {
            isRightHit = true;
            RightNormal = Righthit.normal;
            RightDist = Righthit.distance;
        }

        //左壁チェック
        if (Physics.Raycast(origin, -transform.right, out Lefthit, distance, wallLayers, QueryTriggerInteraction.Ignore))
        {
            isLeftHit = true;
            LeftNormal = Lefthit.normal;
            LeftDist = Lefthit.distance;
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
        else
        {
            WallNormalVec = Vector3.zero;
            isWallHit = false;
            wallStatus = wallSide.NoWallDitect;
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
