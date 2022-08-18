using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// <summary>
// �������
// </summary>
public class WeaponController : MonoBehaviour
{
    public PlayerMovement PM;
    public Transform shooterPoint;//�����λ��

    public int bulletsMag = 31;//һ����ϻ�ӵ�����
    public int range = 100;//���������
    public int bulletLeft = 300;//�����ı���
    public int currentBullects;//��ǰ�ӵ�����


    public ParticleSystem muzzleFlash;//ǹ�ڻ�����Ч
    public Light muzzleFlashLight;//ǹ�ڻ���ƹ�
    public GameObject hitParticle;//�ӵ�����������Ч
    public GameObject bulletHole;//����

    //��Ƶ����
    private AudioSource audioSource;
    public AudioClip AK47SoundClip;//ǹ�����ЧƬ��
    public AudioClip reloadAmmoLeftClip;//���ӵ���Ч1Ƭ��
    public AudioClip reloadOutOfAmmoLeftClip;//���ӵ���Ч2Ƭ��

    private bool isReload;//�ж��Ƿ���װ��
    private bool isAiming;//�ж��Ƿ�����׼

    private bool GunShootInnput;//�������������
    public float fireRate = 0.1f;//���٣�ԽС����ٶ�Խ��
    private float fireTimer;//��ʱ��

    [Header("��λ����")]
    [SerializeField][Tooltip("��װ�ӵ�����")] private KeyCode reloadInputName;
    [SerializeField][Tooltip("�鿴��������")] private KeyCode inspectInputName;


    [Header("UI����")]
    public Image CrossHairUI;
    public Text AmmoTextUI;

    private Animator anim;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        reloadInputName = KeyCode.R;
        inspectInputName = KeyCode.F;
        currentBullects = bulletsMag;
        UpdateAmmoUI();
    }

    // Update is called once per frame
    void Update()
    {
        GunShootInnput = Input.GetMouseButton(0);
        if(GunShootInnput)
        {
            GunFire();
        }
        else
        {
            muzzleFlashLight.enabled = false;
        }

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        //���ֻ��ӵ��Ķ���
        if (info.IsName("reload_zmmo_left") || info.IsName("reload_out_of_ammo")) 
        {
            isReload = true;
        }
        else
        {
            isReload = false;
        }

        if (Input.GetKeyDown(reloadInputName) && currentBullects < bulletsMag && bulletLeft > 0) 
        {
            Reload();
        }
        DoingAim();
        if(Input.GetKeyDown(inspectInputName))
        {
            //���ò鿴��������
            anim.SetTrigger("inspect");
        }
        anim.SetBool("Run", PM.isRun);
        anim.SetBool("Walk", PM.isWalk);

        if (fireTimer < fireRate) 
        {
            fireTimer += Time.deltaTime;
        }
    }

    // <summary>
    // ���
    // </summary>
    public void GunFire()
    {
        //�������٣�����ʱ����ֵ�����ٻ�С��ô����������ִ��
        //ÿ֡��������ִ�еĴ������Ӷ�ʵ�����ٿ���
        if (fireTimer < fireRate || currentBullects <= 0 || isReload || PM.isRun) return;


        RaycastHit hit;
        Vector3 shootDirection = shooterPoint.forward;//���������ǰ��
        if (Physics.Raycast(shooterPoint.position, shootDirection, out hit, range))//�ж����
        {
            Debug.Log(hit.transform.name + "����");
            Destroy(hit.collider.gameObject);//          �ݻٻ��е�����




            GameObject hitParticleEffect = Instantiate(hitParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));//���ɳ��ӵ����еĻ����Ч
            GameObject bulletHoleEffect = Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));//���ɳ�������Ч
            //������Ч
            Destroy(hitParticleEffect, 1f);
            Destroy(bulletHoleEffect, 3f);
        }
        if(!isAiming)
        {
            //������ͨ���𶯻����������뵭��Ч����
            anim.CrossFadeInFixedTime("fire", 0.1f);
        }
        else
        {
            //��׼״̬�����ſ��𶯻�
            anim.CrossFadeInFixedTime("aim_fire", 0.1f);
        }

        PlayerShootSound();//���������Ч
        muzzleFlash.Play();//���Ż����Ч
        muzzleFlashLight.enabled = true;
        currentBullects--;
        UpdateAmmoUI();
        fireTimer = 0f;//���ü�ʱ��

    }

    // <summary>
    // ��װ��ҩ�߼�
    // </summary>
    public void Reload()
    {
        if (bulletLeft <= 0) return;
        DoReloadAnimation();
        //������Ҫ��װ���ӵ���=1����ϻ�ӵ���-��ǰ��ϻ�ӵ���
        int bulletToLoad = bulletsMag - currentBullects;
        //���㱸����۳��ӵ���
        int bulletToReduce = (bulletLeft >= bulletToLoad) ? bulletToLoad : bulletLeft;
        bulletLeft -= bulletToReduce;//���ٱ���
        currentBullects += bulletToReduce;//��ǰ�ӵ�������
        UpdateAmmoUI();
    }

    // <summary>
    // ����װ������
    // </summary>
    public void DoReloadAnimation()
    {
        if (currentBullects > 0)
        {
            //���Ŷ���1
            anim.Play("reload_ammo_left", 0, 0);
            audioSource.clip = reloadAmmoLeftClip;
            audioSource.Play();
        }
        if(currentBullects == 0)
        {
            //���Ŷ���2
            anim.Play("reload_out_of_ammo", 0, 0);
            audioSource.clip = reloadOutOfAmmoLeftClip;
            audioSource.Play();
        }
    }

    public void PlayerShootSound()
    {
        audioSource.clip = AK47SoundClip;
        audioSource.Play();
    }

    // <summary>
    // ��׼���߼�
    // </summary>
    public void DoingAim()
    {
        if (Input.GetMouseButton(1) && !isReload && !PM.isRun)  
        {
            //��׼   ׼����ʧ����Ұ��ǰ
            isAiming = true;
            anim.SetBool("Aim", true);
            CrossHairUI.gameObject.SetActive(false);
            mainCamera.fieldOfView = 25;//��׼��ʱ���������Ұ��С
        }
        else
        {
            //����׼
            isAiming = false;
            anim.SetBool("Aim", false);
            CrossHairUI.gameObject.SetActive(true);
            mainCamera.fieldOfView = 60;//����׼��ʱ���������Ұ���
        }
        
    }

    // <summary>
    // ����UI
    // </summary>
    public void UpdateAmmoUI()
    {
        AmmoTextUI.text = currentBullects + " / " + bulletLeft;
    }
}
