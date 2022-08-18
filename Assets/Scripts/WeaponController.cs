using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// <summary>
// 武器射击
// </summary>
public class WeaponController : MonoBehaviour
{
    public PlayerMovement PM;
    public Transform shooterPoint;//射击的位置

    public int bulletsMag = 31;//一个弹匣子弹数量
    public int range = 100;//武器的射程
    public int bulletLeft = 300;//武器的备弹
    public int currentBullects;//当前子弹数量


    public ParticleSystem muzzleFlash;//枪口火焰特效
    public Light muzzleFlashLight;//枪口火焰灯光
    public GameObject hitParticle;//子弹击中粒子特效
    public GameObject bulletHole;//弹孔

    //音频参数
    private AudioSource audioSource;
    public AudioClip AK47SoundClip;//枪射击音效片段
    public AudioClip reloadAmmoLeftClip;//换子弹音效1片段
    public AudioClip reloadOutOfAmmoLeftClip;//换子弹音效2片段

    private bool isReload;//判断是否在装弹
    private bool isAiming;//判断是否在瞄准

    private bool GunShootInnput;//按下鼠标左键射击
    public float fireRate = 0.1f;//射速，越小射击速度越快
    private float fireTimer;//计时器

    [Header("键位设置")]
    [SerializeField][Tooltip("填装子弹按键")] private KeyCode reloadInputName;
    [SerializeField][Tooltip("查看武器按键")] private KeyCode inspectInputName;


    [Header("UI设置")]
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
        //两种换子弹的动画
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
            //设置查看武器动画
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
    // 射击
    // </summary>
    public void GunFire()
    {
        //控制射速，若计时器的值比射速还小那么跳出方法不执行
        //每帧减少射线执行的次数，从而实现射速控制
        if (fireTimer < fireRate || currentBullects <= 0 || isReload || PM.isRun) return;


        RaycastHit hit;
        Vector3 shootDirection = shooterPoint.forward;//射击方向（向前）
        if (Physics.Raycast(shooterPoint.position, shootDirection, out hit, range))//判断射击
        {
            Debug.Log(hit.transform.name + "打到了");
            Destroy(hit.collider.gameObject);//          摧毁击中的物体




            GameObject hitParticleEffect = Instantiate(hitParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));//生成出子弹击中的火光特效
            GameObject bulletHoleEffect = Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));//生成出弹孔特效
            //回收特效
            Destroy(hitParticleEffect, 1f);
            Destroy(bulletHoleEffect, 3f);
        }
        if(!isAiming)
        {
            //播放普通开火动画（动画淡入淡出效果）
            anim.CrossFadeInFixedTime("fire", 0.1f);
        }
        else
        {
            //瞄准状态，播放开火动画
            anim.CrossFadeInFixedTime("aim_fire", 0.1f);
        }

        PlayerShootSound();//播放射击音效
        muzzleFlash.Play();//播放火光特效
        muzzleFlashLight.enabled = true;
        currentBullects--;
        UpdateAmmoUI();
        fireTimer = 0f;//重置计时器

    }

    // <summary>
    // 填装弹药逻辑
    // </summary>
    public void Reload()
    {
        if (bulletLeft <= 0) return;
        DoReloadAnimation();
        //计算需要填装的子弹数=1个弹匣子弹数-当前弹匣子弹数
        int bulletToLoad = bulletsMag - currentBullects;
        //计算备弹需扣除子弹数
        int bulletToReduce = (bulletLeft >= bulletToLoad) ? bulletToLoad : bulletLeft;
        bulletLeft -= bulletToReduce;//减少备弹
        currentBullects += bulletToReduce;//当前子弹数增加
        UpdateAmmoUI();
    }

    // <summary>
    // 播放装弹动画
    // </summary>
    public void DoReloadAnimation()
    {
        if (currentBullects > 0)
        {
            //播放动画1
            anim.Play("reload_ammo_left", 0, 0);
            audioSource.clip = reloadAmmoLeftClip;
            audioSource.Play();
        }
        if(currentBullects == 0)
        {
            //播放动画2
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
    // 瞄准的逻辑
    // </summary>
    public void DoingAim()
    {
        if (Input.GetMouseButton(1) && !isReload && !PM.isRun)  
        {
            //瞄准   准心消失，视野靠前
            isAiming = true;
            anim.SetBool("Aim", true);
            CrossHairUI.gameObject.SetActive(false);
            mainCamera.fieldOfView = 25;//瞄准的时候摄像机视野变小
        }
        else
        {
            //非瞄准
            isAiming = false;
            anim.SetBool("Aim", false);
            CrossHairUI.gameObject.SetActive(true);
            mainCamera.fieldOfView = 60;//非瞄准的时候摄像机视野变大
        }
        
    }

    // <summary>
    // 更新UI
    // </summary>
    public void UpdateAmmoUI()
    {
        AmmoTextUI.text = currentBullects + " / " + bulletLeft;
    }
}
