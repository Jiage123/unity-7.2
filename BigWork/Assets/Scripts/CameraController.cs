using System.Collections;
using System.Collections.Generic;
using UnityEngine;  // ���������������ת����

public class CameraController : MonoBehaviour
{
    public Transform Player; // ����
    private float MouseX, MouseY; //��ȡ����ƶ���ֵ
    private float MouseSensitivity; // ���������
    private bool isGround;
    private float XRotation; // ������ת����Χ��X����ת

    void Start()
    {
        MouseSensitivity = 150f; // �����������Ϊ150f
        isGround = true;
        // ȷ����ͷ�ĳ�ʼλ��Ϊ��ȷ��
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        // Mouse X ��ʾ�����ᣬ��������ƶ����� -1����������ƶ����� 1
        MouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime * 2;

        // Mouse Y ��ʾ������ᣬ��������ƶ����� -1����������ƶ����� 1
        MouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        // XRotationΪ��ֵʱ������̧����գ�����MouseYΪ��ʱ����������ƶ������ö�ӦXRotationΪ��
        XRotation -= MouseY; // ���Y����ƶ���

        // ��XRotationd��ֵ������ -70f �� 70f֮��
        XRotation = Mathf.Clamp(XRotation, -25f, 25f);

        // ����������ת
        Player.Rotate(Vector3.up * MouseX);

        // ���������ת
        transform.localRotation = Quaternion.Euler(XRotation, 0, 0);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isGround)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGround = true;
            }
        }
    }
}