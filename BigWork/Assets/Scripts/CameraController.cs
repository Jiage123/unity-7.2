using System.Collections;
using System.Collections.Generic;
using UnityEngine;  // 用鼠标控制人物的旋转方向

public class CameraController : MonoBehaviour
{
    public Transform Player; // 主角
    private float MouseX, MouseY; //获取鼠标移动的值
    private float MouseSensitivity; // 鼠标灵敏度
    private bool isGround;
    private float XRotation; // 上下旋转量，围绕X轴旋转

    void Start()
    {
        MouseSensitivity = 150f; // 鼠标灵敏度设为150f
        isGround = true;
        // 确保镜头的初始位置为正确的
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        // Mouse X 表示鼠标横轴，鼠标向左移动返回 -1，鼠标向右移动返回 1
        MouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime * 2;

        // Mouse Y 表示鼠标纵轴，鼠标向下移动返回 -1，鼠标向上移动返回 1
        MouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        // XRotation为负值时，向上抬向天空，所以MouseY为正时，鼠标上下移动，正好对应XRotation为负
        XRotation -= MouseY; // 鼠标Y轴的移动量

        // 将XRotationd的值限制在 -70f 到 70f之间
        XRotation = Mathf.Clamp(XRotation, -25f, 25f);

        // 人物左右旋转
        Player.Rotate(Vector3.up * MouseX);

        // 相机上下旋转
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