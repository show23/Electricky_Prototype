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

    //�v���C���[�̃X�e�[�^�X
    [Tooltip("���s���x")] 
    public float MaxWalkSpeed;
    [Tooltip("�����x(���s)")]
    public float WalkAcc;
    [Tooltip("���鎞�̑��x")]
    public float MaxRunSpeed;
    [Tooltip("�����x(����)")]
    public float RunAcc;
    [Tooltip("���Ⴊ�ݎ��̑��x")]
    public float MaxCrouchSpeed;


    [Tooltip("�Ǒ���������I�����鑬�x")]
    public float WallRunStopSpeedValue;
    private GameObject WallObj;


    [Tooltip("���x�ێ���"), Range(0.97f,1.0f)]
    public float VelocityHoldRate;

    [Tooltip("�󒆑��x�ێ���(�n��)"), Range(0.97f,1.0f)]
    public float AirVelocityHoldRate;
    [Tooltip("�󒆉�����(�n��)"), Range(0.0f, 1.0f)]
    public float AirVelocityAccRate;

    [Tooltip("�v���C���[�̒ʏ�W�����v��")]
    public float PlayerJumpPower;
    [Tooltip("�v���C���[��2�i�W�����v���p���[(�ʏ�W�����v�͊)")]
    public float PlayerSecondJumpMultiplyValue;

    [Tooltip("�v���C���[�̉�]���x")]
    public float PlayerRotationSpeed;

    [Tooltip("�X���C�f�B���O���x")]
    public float SlidingSpeed;

    [Tooltip("�X���C�f�B���O�ێ�����")]
    public float keepSlideTime;
    private float SlidingTimer;



    [Tooltip("�v���C���[�̌��݂̑��x�l(�f�o�b�O�p)"),SerializeField]
    private float playerSpeed;


    //�ǔ���̋���
    public bool isWallHit = false;
    [Tooltip("0:noWall 1:RightWall 2:LeftWall")]
    public int wallStatus = 0;

    //�v���C���[����ǂւ̃x�N�g��
    private Vector3 WallVector;

    //�Ǒ���̎��A���������ȊO�͂��̃x�N�g���ƕ��s�ɑ���Ώ����ł���悤�ɂ���
    private Vector3 WallRunVector;

    //�ǂ̊p�x(Quad��EularAngles����擾)
    private float WallAngle;



    //�ڒn����
    public bool isGround = true;
    public bool SecondJumped = false;

    public bool isClouch = false;
    public bool isSliding = false;


    //�X���C�f�B���O�֌W�̐��l�ݒ�
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

        // �}�E�X�J�[�\�����\���ɂ��A�ʒu���Œ�
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    //�Ƃ肠�����ŏ����t���[�������Ă���
    //�������������Ă��Ȃ����̂ɂ͓���#������
    private void FixedUpdate()
    {
<<<<<<< HEAD
=======
        
>>>>>>> Oshima_Test
        //-------------------------------------------------------------------------------
        //#�ǂ̔z�u���m�F���A�J�����̈ʒu�𒲐��A�ǔ�������
        //-------------------------------------------------------------------------------
        
        PlayerCameraOrigin.transform.localPosition = new Vector3(0, 1.375f, 0);
        if (isWallHit)
        {
            if (wallStatus == 1)
            {
                PlayerCameraOrigin.transform.localPosition = new Vector3(-1, 1.375f, 0);
            }
            if (wallStatus == 2)
            {
                PlayerCameraOrigin.transform.localPosition = new Vector3( 1, 1.375f, 0);
            }
        }

        //���ł�isGround�̍X�V
        {
            float raycastDistance = 0.2f;
            RaycastHit Hit;
            if (Physics.Raycast(transform.position + transform.up * 0.1f, Vector3.down, out Hit, raycastDistance))
            {
                isGround = true;
                SecondJumped = false;
                WallRunEnd();
            }
            else
            {
                isGround = false;
            }
        }
        //-------------------------------------------------------------------------------
        //�v���C���[�̓��͒l�̌��m
        //-------------------------------------------------------------------------------


        //���͒l�̍X�V
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


            if (isWallHit) { 
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
                //�������ɂȂɂ�����Ƃ��Ɏ����ł��Ⴊ�ݓ���
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


            //�����Ă���Ƃ��̓X�e�B�b�N�����ς��ɓ��͂��Ă��锻��ɂȂ�
            if (RunInput)
            {
                Vector2 vec = MoveInput.normalized;
                MoveInput = vec;
            }

            OldJumpInput = JumpInput;
            OldAttack_1_Input = Attack_1_Input;
            OldAttack_2_Input = Attack_2_Input;
            OldAttack_3_Input = Attack_3_Input;
            OldCrouchInput = CrouchInput;
        }

        //-------------------------------------------------------------------------------
        //�J�����̊p�x/�ǂ̊p�x����v���C���[�̓��͒l�𒲐� �v���C���[����]
        //-------------------------------------------------------------------------------

        Vector3 MoveOriginVector = Vector3.Scale(PlayerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        transform.rotation = Quaternion.LookRotation(transform.forward + transform.right * CameraInput.x * PlayerRotationSpeed);

        if (isWallHit)
        {
            MoveOriginVector = Vector3.Scale(WallRunVector, new Vector3(1, 0, 1)).normalized;
            transform.rotation = Quaternion.LookRotation(MoveOriginVector);
        }

        //------------------------------------------------------------
        //���Ⴊ��&�X���C�f�B���O
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
        //�v���C���[�̓�����RigidBody�ɓ���
        //-------------------------------------------------------------------------------

        {
            Vector3 moveForward = MoveOriginVector * MoveInput.y;

            if (!isWallHit)
                moveForward += PlayerCamera.transform.right * MoveInput.x;

            float speedHoldRate = VelocityHoldRate;
            float speedAddRate = 1.0f;


            if (!isGround)
            {
                speedHoldRate = AirVelocityHoldRate;
                speedAddRate = AirVelocityAccRate;
            }



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

            //�������l�̒���(�ō����x�𒴂��Ȃ��悤�ɂ���)
            //�����̏����̂����ł��Ȃ育������Ă��܂���
            float MaxaddSpeed = Mathf.Clamp(maxSpeed - Vel.magnitude, 0, maxSpeed);

            if (accValue >= MaxaddSpeed)
            {
                accValue = Mathf.Clamp(accValue - moveForward.magnitude, 0, accValue);
            }

            Vector3 MoveVel = moveForward * accValue;

            s_Rigidbody.AddForce(MoveVel);
            s_Rigidbody.useGravity = true;

            if (isWallHit)
            {
                Vector3 v = s_Rigidbody.velocity;
                v.y = 0;
                s_Rigidbody.velocity = v;
                s_Rigidbody.useGravity = false;
            }

            //��������(�o�O�̉���)
            {
                Vector3 selfSpeed = s_Rigidbody.velocity;
                selfSpeed *= speedHoldRate;
                selfSpeed.y = s_Rigidbody.velocity.y;

                s_Rigidbody.velocity = selfSpeed;
            }
        }

        //------------------------------------------------------------
        //�W�����v
        //------------------------------------------------------------
        if (!isWallHit)
        {
            //�����t���[��2�i�W�����v�̕�����̕�������
            if (jumpInputTrigger && !isGround && !SecondJumped)
            {
                SecondJumped = true;
                isGround = false;
                s_Rigidbody.AddForce(Vector3.up * PlayerJumpPower * PlayerSecondJumpMultiplyValue, ForceMode.Impulse);
            }

            if (jumpInputTrigger && isGround)
            {
                isGround = false;
                s_Rigidbody.AddForce(Vector3.up * PlayerJumpPower, ForceMode.Impulse);
            }
        }
        else
        {
            if (jumpInputTrigger)
            {
                SecondJumped = false;
                isWallHit = false;

                Vector3 moveVel = new Vector3(MoveInput.x, 0, MoveInput.y) + transform.forward;
                moveVel.y = 0;
                moveVel = moveVel.normalized;
                
                s_Rigidbody.velocity = new Vector3(0, 0, 0);
                s_Rigidbody.AddForce(Vector3.up * PlayerJumpPower + moveVel * MaxRunSpeed, ForceMode.Impulse);
            }
        }


        //---------------------------------------------------------
        //�Ǒ�����ێ�����̂��̔���()
        //---------------------------------------------------------

        playerSpeed = new Vector2(s_Rigidbody.velocity.x, s_Rigidbody.velocity.z).magnitude;

        if (isWallHit)
        {
            if (playerSpeed < WallRunStopSpeedValue)
            {
                WallRunEnd();
                //Debug.Log("WallRun : Stopped.tooslow(" + playerSpeed + ")");
            }
        }

        //-------------------------------------------------------------------------------
        //#�v���C���[�̍U������
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
        //#�A�j���[�V�������A�j���[�^�[�ɓo�^
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

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            if (!isGround)
            {
                if (playerSpeed > WallRunStopSpeedValue)
                {
                    isGround = false;
                    SecondJumped = false;
                    if (isWallHit)
                    {
                        if (WallObj == collision.gameObject)
                            return;

                        WallSet(collision);
                        //���x�̈��p��

                        float spd = s_Rigidbody.velocity.magnitude;
                        Vector3 moveVel = WallRunVector.normalized * spd;
                        s_Rigidbody.velocity = moveVel;

                        //Debug.Log("WallRun : Changed.otherWallDitect");
                    }
                    else
                    {
                        WallSet(collision);
                        isWallHit = true;

                        //�Ǒ���̏���
                        //�Ǒ���J�n���ɕǉ����ɉ�������悤�ɂ���
                        Vector3 moveVel = WallRunVector.normalized * MaxRunSpeed;
                        s_Rigidbody.velocity = moveVel;

                        //Debug.Log("WallRun : Start (" + moveVel.magnitude + ")");
                    }
                }
            }
        }
    }


    void WallCheck()
    {





    }




    void WallSet(Collision obj)
    {
        Vector3 WallPoint = obj.GetContact(0).point;

        WallVector = WallPoint - transform.position;
        // �v���C���[�̈ʒu
        Vector3 playerPosition = transform.position;

        // �v���C���[����ǂւ̃x�N�g�����v�Z
        Vector3 playerToWall = WallPoint - playerPosition;
        playerToWall.y = 0; // Y�������𖳎�

        // �v���C���[�̑O���x�N�g��
        Vector3 playerForward = transform.forward;
        playerForward.y = 0; // Y�������𖳎�

        
        // �v���C���[�̌���ɂ���ꍇ�͉������Ȃ�
        if (Vector3.Dot(playerForward, playerToWall) < 0)
        {
            return;
        }
        
        // �v���C���[�̑O���x�N�g���ƃv���C���[����ǂւ̃x�N�g���̊p�x���v�Z
        float angle = Vector3.SignedAngle(playerForward, playerToWall, Vector3.up);

        // �p�x�����Ȃ�ǂ̓v���C���[�̉E���ɂ���
        // �p�x�����Ȃ�ǂ̓v���C���[�̍����ɂ���
        // ���[�V�����ɓ��͂��邽�߂ɕ����Ă���
        if (angle > 0)
        {
            wallStatus = 1;
            //Debug.Log("WallRun : Right Wall Ditect");
        }
        else
        {
            wallStatus = 2;
            //Debug.Log("WallRun : Left Wall Ditect");
        }

        WallAngle = obj.transform.eulerAngles.y;
        WallRunVector = Quaternion.Euler(0, WallAngle, 0) * Vector3.right;

        if (Vector3.Dot(WallRunVector, transform.forward) < 0)
        {
            WallRunVector = -WallRunVector;
        }

        // �x�N�g���𐳋K���i�P�ʃx�N�g���ɂ���j
        WallRunVector = WallRunVector.normalized;
        WallObj = obj.gameObject;
        //Debug.Log(obj.gameObject.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isWallHit)
        {
            if (collision.gameObject.tag == "Wall" && collision.gameObject == WallObj)
            {
                //Debug.Log("WallRun : Stopped.noWallHit");
                WallRunEnd();
            }
        }
    }

    
    private void WallRunEnd()
    {
        WallObj = null;
        isWallHit = false;
        wallStatus = 0;
    }


    private void OnDrawGizmos()
    {
        Vector3 vec = new Vector3(MoveInput.x, 0, MoveInput.y);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward + vec).normalized * 2);


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
