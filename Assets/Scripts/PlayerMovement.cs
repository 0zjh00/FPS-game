using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// <summary>
// 控制玩家移动
// </summary>
public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    public float walkspeed = 10f;//行走速度
    public float runSpeed = 15f;//奔跑速度
    public float speed;//移动速度
    public Vector3 moveDirction;//设置移动方向

    private Transform groundCheck;//地面检查
    private float groundDistance = 0.1f;//与地面的距离
    public LayerMask groundMash;

    public float jumpForce = 3f;//跳跃力度
    public Vector3 velocity;//设置玩家Y轴的1个冲量变化（力）
    public float gravity = -20f;//设置重力

    private bool isJump;//判断是否在跳跃
    public bool isRun;//判断是否在奔跑
    public bool isWalk;//判断是否在行走
    private bool isGround;

    [SerializeField] private float slopeForce = 6.0f;//走斜坡时施加的力度
    [SerializeField] private float slopeForceRayLength = 2.0f;//斜坡射线长度

    [Header("声音设置")]
    private AudioSource audioSource;
    public AudioClip walkingSound;
    public AudioClip runningSound;


    //键位设置
    [Header("键位设置")]
    [SerializeField][Tooltip("奔跑按键")] private KeyCode runInputName;//奔跑键位
    [SerializeField][Tooltip("跳跃按键")] public string jumpInputName = "Jump";

    private void Start()
    {
        //获取player身上的CharacterController 组件
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        runInputName = KeyCode.LeftShift;
        groundCheck = GameObject.Find("Player/CheckGround").GetComponent<Transform>();
    }

    private void Update()
    {
        CheckGround();
        Move();
    }


    //移动
    public void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        isWalk = (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0) ? true : false;
        isRun = Input.GetKey(runInputName);
        if(isRun)//设置行走或者奔跑的速度
        {
            speed = runSpeed;
        }
        else
        {
            speed = walkspeed;
        }

        moveDirction = (transform.right * h + transform.forward * v).normalized;   //设置玩家移动方向
        characterController.Move(moveDirction * walkspeed * Time.deltaTime);//移动

        if(isGround==false){velocity.y += gravity * Time.deltaTime;}//不在地面（空中），累加向下的力
        characterController.Move(velocity * Time.deltaTime);//给player施加一个向上的跳跃力和重力
        Jump();
        if (OnSlpe()) //如果处在斜坡上移动
        {
            //向下增加力
            characterController.Move(Vector3.down * characterController.height / 2 * slopeForce * Time.deltaTime);
        }
        PlayFootStepSound();
    }

    // <summary>
    // 播放移动音效
    // </summary>
    public void PlayFootStepSound()
    {
        if (isGround && moveDirction.sqrMagnitude > 0.9f) 
        {
            audioSource.clip = isRun ? runningSound : walkingSound;//设置行走或奔跑音效
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

        }
        else
        {
            if(audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }

    // <summary>
    // 跳跃
    // </summary>
    public void Jump()
    {
        isJump = Input.GetButtonDown(jumpInputName);//按下跳跃键
        //施加跳跃力

        //if(isJump && isGround)
        if(isJump)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    // <summary>
    // 地面检测
    // </summary>
    public void CheckGround()
    {
        //在groundCheck位置上做1个球体检测，判断是否在地面上
        isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMash);
        //在地面上给1个向下的力
        if (isGround && velocity.y <= 0) 
        {
            velocity.y = -2f;
        }
    }

    // <summary>
    // 判断是否在斜坡
    // </summary>
    public bool OnSlpe()
    {
        if(isJump)
        {
            return false;
        }

        RaycastHit hit;
        //向下打错射线（检测是否在斜坡上）
        if(Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit, characterController.height / 2 * slopeForceRayLength))
        {
            //如果接触到的点的法线不在(0,1,0)的方向上，那么人物就在斜坡上
            if(hit.normal !=Vector3.up)
            {
                return true;
            }
        }

        return false;
    }

}




