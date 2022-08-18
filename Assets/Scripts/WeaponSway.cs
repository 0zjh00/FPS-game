using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
// 武器摇摆
// </summary>
public class WeaponSway : MonoBehaviour
{
    //摇摆的参数
    public float amout;//摇摆幅度
    public float smoothAmout;//摇摆平滑值
    public float maxAmout;//最大幅度摇摆

    [SerializeField] private Vector3 originPosition;//初始位置


    // Start is called before the first frame update
    void Start()
    {
        //自身位置(相对应父级物体的变换位置)
        originPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //获取鼠标轴值
        float movementX = -Input.GetAxis("Mouse X") * amout;
        float movementY = -Input.GetAxis("Mouse Y") * amout;
        //限制
        movementX = Mathf.Clamp(movementX, -maxAmout, maxAmout);
        movementY = Mathf.Clamp(movementY, -maxAmout, maxAmout);
        //手臂位置变化
        Vector3 finallyPosition = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finallyPosition + originPosition, Time.deltaTime * smoothAmout);
    }
}
