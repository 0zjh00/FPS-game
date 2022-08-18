using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// <summary>
// ��������ƶ�
// </summary>
public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    public float walkspeed = 10f;//�����ٶ�
    public float runSpeed = 15f;//�����ٶ�
    public float speed;//�ƶ��ٶ�
    public Vector3 moveDirction;//�����ƶ�����

    private Transform groundCheck;//������
    private float groundDistance = 0.1f;//�����ľ���
    public LayerMask groundMash;

    public float jumpForce = 3f;//��Ծ����
    public Vector3 velocity;//�������Y���1�������仯������
    public float gravity = -20f;//��������

    private bool isJump;//�ж��Ƿ�����Ծ
    public bool isRun;//�ж��Ƿ��ڱ���
    public bool isWalk;//�ж��Ƿ�������
    private bool isGround;

    [SerializeField] private float slopeForce = 6.0f;//��б��ʱʩ�ӵ�����
    [SerializeField] private float slopeForceRayLength = 2.0f;//б�����߳���

    [Header("��������")]
    private AudioSource audioSource;
    public AudioClip walkingSound;
    public AudioClip runningSound;


    //��λ����
    [Header("��λ����")]
    [SerializeField][Tooltip("���ܰ���")] private KeyCode runInputName;//���ܼ�λ
    [SerializeField][Tooltip("��Ծ����")] public string jumpInputName = "Jump";

    private void Start()
    {
        //��ȡplayer���ϵ�CharacterController ���
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


    //�ƶ�
    public void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        isWalk = (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0) ? true : false;
        isRun = Input.GetKey(runInputName);
        if(isRun)//�������߻��߱��ܵ��ٶ�
        {
            speed = runSpeed;
        }
        else
        {
            speed = walkspeed;
        }

        moveDirction = (transform.right * h + transform.forward * v).normalized;   //��������ƶ�����
        characterController.Move(moveDirction * walkspeed * Time.deltaTime);//�ƶ�

        if(isGround==false){velocity.y += gravity * Time.deltaTime;}//���ڵ��棨���У����ۼ����µ���
        characterController.Move(velocity * Time.deltaTime);//��playerʩ��һ�����ϵ���Ծ��������
        Jump();
        if (OnSlpe()) //�������б�����ƶ�
        {
            //����������
            characterController.Move(Vector3.down * characterController.height / 2 * slopeForce * Time.deltaTime);
        }
        PlayFootStepSound();
    }

    // <summary>
    // �����ƶ���Ч
    // </summary>
    public void PlayFootStepSound()
    {
        if (isGround && moveDirction.sqrMagnitude > 0.9f) 
        {
            audioSource.clip = isRun ? runningSound : walkingSound;//�������߻�����Ч
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
    // ��Ծ
    // </summary>
    public void Jump()
    {
        isJump = Input.GetButtonDown(jumpInputName);//������Ծ��
        //ʩ����Ծ��

        //if(isJump && isGround)
        if(isJump)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    // <summary>
    // ������
    // </summary>
    public void CheckGround()
    {
        //��groundCheckλ������1�������⣬�ж��Ƿ��ڵ�����
        isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMash);
        //�ڵ����ϸ�1�����µ���
        if (isGround && velocity.y <= 0) 
        {
            velocity.y = -2f;
        }
    }

    // <summary>
    // �ж��Ƿ���б��
    // </summary>
    public bool OnSlpe()
    {
        if(isJump)
        {
            return false;
        }

        RaycastHit hit;
        //���´�����ߣ�����Ƿ���б���ϣ�
        if(Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit, characterController.height / 2 * slopeForceRayLength))
        {
            //����Ӵ����ĵ�ķ��߲���(0,1,0)�ķ����ϣ���ô�������б����
            if(hit.normal !=Vector3.up)
            {
                return true;
            }
        }

        return false;
    }

}




