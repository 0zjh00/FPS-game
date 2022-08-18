using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// <summary>
// ���������ת
// ���������ת����ʵ��������ת
// �����������ת����ʵ��������ת
// </summary>
public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;//����������
    public Transform playerBody;//��ҵ�λ��
    public float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //����������ڸ���Ϸ���ڵ����ģ�����Ӳ�����
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;//��������ת����ֵ�����ۼ�
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);//������ֵ���ۼƣ�����80�㣩
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);//��Һ�����ת
    }
}
