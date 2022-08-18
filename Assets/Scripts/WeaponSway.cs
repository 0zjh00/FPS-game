using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
// ����ҡ��
// </summary>
public class WeaponSway : MonoBehaviour
{
    //ҡ�ڵĲ���
    public float amout;//ҡ�ڷ���
    public float smoothAmout;//ҡ��ƽ��ֵ
    public float maxAmout;//������ҡ��

    [SerializeField] private Vector3 originPosition;//��ʼλ��


    // Start is called before the first frame update
    void Start()
    {
        //����λ��(���Ӧ��������ı任λ��)
        originPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //��ȡ�����ֵ
        float movementX = -Input.GetAxis("Mouse X") * amout;
        float movementY = -Input.GetAxis("Mouse Y") * amout;
        //����
        movementX = Mathf.Clamp(movementX, -maxAmout, maxAmout);
        movementY = Mathf.Clamp(movementY, -maxAmout, maxAmout);
        //�ֱ�λ�ñ仯
        Vector3 finallyPosition = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finallyPosition + originPosition, Time.deltaTime * smoothAmout);
    }
}
