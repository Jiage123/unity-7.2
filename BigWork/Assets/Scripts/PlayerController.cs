using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rigid; //��Ҹ���
    public float moveSpeed = 2f; // ��ʼ�ƶ��ٶ�
    public float rotateSpeed = 90f;
    public float accelerationSpeed = 2f; // ��ʼ���ٶ�
    private bool isGround;
    public float jumpForce = 5f;
    private Animator animator; //��Ҷ�����
    public float kickForce = 10f; // ��������
    public float kickRange = 1f; // ��ɫ�����������
    private bool canJump = true; // ���ƽ�ɫ�Ƿ������Ծ
    public GameObject ballUI;
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        ballUI.gameObject.SetActive(false);


    }

    void Update()
    {
        Move();
        Jump();

        if (Input.GetKeyDown(KeyCode.F))
        {
            KickBall();
        }
        BallUI(); // Check the distance to the ball continuously
    }

    //�ƶ�
    void Move()
    {
        float h = Input.GetAxis("Horizontal"); //��ȡˮƽ/��ֱ�ƶ�����
        float v = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(h, 0, v);

        if (h != 0 || v != 0)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walking", true);

            float targetRotation = rotateSpeed * h;
            transform.eulerAngles = Vector3.up * Mathf.Lerp(transform.eulerAngles.y,
                                        transform.eulerAngles.y + targetRotation, Time.deltaTime);

            float currentSpeed = moveSpeed;

            // ����
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("Walking", false);
                animator.SetBool("Running", true);
                currentSpeed += accelerationSpeed;
            }
            else
            {
                animator.SetBool("Walking", true);
                animator.SetBool("Running", false);
                currentSpeed += moveSpeed;
            }

            rigid.MovePosition(transform.position +
                               transform.TransformDirection(direction) * Time.fixedDeltaTime * currentSpeed);
        }
        else
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Walking", false);
        }
    }

    //��Ծ
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround && canJump)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walking", false);
            animator.SetTrigger("Jump");
            rigid.AddForce(Vector3.up * jumpForce);
            isGround = false;
            canJump = false; // ����Ϊfalse��������Ծ
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGround)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGround = true;
                canJump = true; // �����ɫ��½�ڵ����ϣ�������Ծ

            }
        }
    }



    //����
    void KickBall()
    {
        // ��ȡ�����������
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

        if (balls.Length > 0)
        {
            GameObject closestBall = FindClosestBall(balls);

            if (closestBall != null)
            {
                float distance = Vector3.Distance(transform.position, closestBall.transform.position);

                if (distance <= kickRange)
                {

                    canJump = true; // ������ǰ�ָ���Ծ����
                    animator.SetTrigger("KickBall"); // �������򶯻�

                    StartCoroutine(KickAfterAnimation(closestBall)); // �ڶ���������Ϻ�����
                }
            }

        }
    }
    //�ҵ����б�ǩΪBall������
    GameObject FindClosestBall(GameObject[] balls)
    {
        GameObject closestBall = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject ball in balls)
        {
            float distance = Vector3.Distance(transform.position, ball.transform.position);

            if (distance < closestDistance)
            {
                closestBall = ball;
                closestDistance = distance;
            }
        }

        return closestBall;

    }

    IEnumerator KickAfterAnimation(GameObject ball)
    {
        yield return new WaitForSeconds(0.5f); // ���趯������ʱ��Ϊ0.5�룬����ʵ���������

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * kickForce, ForceMode.Impulse); // �ڽ�ɫǰ��ʩ������
        canJump = true; // �����ָ���Ծ����


    }

    //��������ʾUI
    void BallUI()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

        if (balls.Length > 0)
        {
            GameObject closestBall = FindClosestBall(balls);

            if (closestBall != null)
            {
                float distance = Vector3.Distance(transform.position, closestBall.transform.position);

                if (distance <= kickRange)
                {
                    ballUI.gameObject.SetActive(true); // Show UI when near the ball
                }
                else
                {
                    ballUI.gameObject.SetActive(false); // Hide UI when not near the ball
                }
            }
        }
    }
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    private Rigidbody rigid; // ��Ҹ���
//    public float moveSpeed = 2f; // ��ʼ�ƶ��ٶ�
//    public float rotateSpeed = 90f;
//    public float accelerationSpeed = 2f; // ��ʼ���ٶ�
//    private bool isGround;
//    public float jumpForce = 5f;
//    private Animator animator; // ��Ҷ�����
//    public float kickForce = 10f; // ��������
//    public float kickRange = 1f; // ��ɫ�����������
//    private bool canJump = true; // ���ƽ�ɫ�Ƿ������Ծ
//    public GameObject ballUI;

//    private static PlayerController instance;

//    public static PlayerController Instance
//    {
//        get
//        {
//            if (instance == null)
//            {
//                instance = FindObjectOfType<PlayerController>();
//            }
//            return instance;
//        }
//    }

//    private void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    void Start()
//    {
//        rigid = GetComponent<Rigidbody>();
//        animator = GetComponent < Animator>();
//        ballUI.gameObject.SetActive(false);
//    }

//    void Update()
//    {
//        Move();
//        Jump();

//        if (Input.GetKeyDown(KeyCode.F))
//        {
//            KickBall();
//        }
//        BallUI(); // �����������ľ���
//    }

//    // �ƶ�
//    void Move()
//    {
//        float h = Input.GetAxis("Horizontal"); // ��ȡˮƽ/��ֱ�ƶ�����
//        float v = Input.GetAxis("Vertical");
//        Vector3 direction = new Vector3(h, 0, v);

//        if (h != 0 || v != 0)
//        {
//            animator.SetBool("Idle", false);
//            animator.SetBool("Walking", true);

//            float targetRotation = rotateSpeed * h;
//            transform.eulerAngles = Vector3.up * Mathf.Lerp(transform.eulerAngles.y,
//                                        transform.eulerAngles.y + targetRotation, Time.deltaTime);

//            float currentSpeed = moveSpeed;

//            // ����
//            if (Input.GetKey(KeyCode.LeftShift))
//            {
//                animator.SetBool("Walking", false);
//                animator.SetBool("Running", true);
//                currentSpeed += accelerationSpeed;
//            }
//            else
//            {
//                animator.SetBool("Walking", true);
//                animator.SetBool("Running", false);
//                currentSpeed += moveSpeed;
//            }

//            rigid.MovePosition(transform.position +
//                               transform.TransformDirection(direction) * Time.fixedDeltaTime * currentSpeed);
//        }
//        else
//        {
//            animator.SetBool("Idle", true);
//            animator.SetBool("Walking", false);
//        }
//    }

//    // ��Ծ
//    void Jump()
//    {
//        if (Input.GetKeyDown(KeyCode.Space) && isGround && canJump)
//        {
//            animator.SetBool("Idle", false);
//            animator.SetBool("Walking", false);
//            animator.SetTrigger("Jump");
//            rigid.AddForce(Vector3.up * jumpForce);
//            isGround = false;
//            canJump = false; // ����Ϊfalse��������Ծ
//        }
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (!isGround)
//        {
//            if (collision.gameObject.CompareTag("Ground"))
//            {
//                isGround = true;
//                canJump = true; // �����ɫ��½�ڵ����ϣ�������Ծ
//            }
//        }
//    }

//    // ����
//    void KickBall()
//    {
//        // ��ȡ�����������
//        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

//        if (balls.Length > 0)
//        {
//            GameObject closestBall = FindClosestBall(balls);

//            if (closestBall != null)
//            {
//                float distance = Vector3.Distance(transform.position, closestBall.transform.position);

//                if (distance <= kickRange)
//                {
//                    canJump = true; // ������ǰ�ָ���Ծ����
//                    animator.SetTrigger("KickBall"); // �������򶯻�

//                    StartCoroutine(KickAfterAnimation(closestBall)); // �ڶ���������Ϻ�����
//                }
//            }
//        }
//    }

//    // �ҵ��������
//    GameObject FindClosestBall(GameObject[] balls)
//    {
//        GameObject closestBall = null;
//        float closestDistance = Mathf.Infinity;

//        foreach (GameObject ball in balls)
//        {
//            float distance = Vector3.Distance(transform.position, ball.transform.position);

//            if (distance < closestDistance)
//            {
//                closestBall = ball;
//                closestDistance = distance;
//            }
//        }

//        return closestBall;
//    }

//    IEnumerator KickAfterAnimation(GameObject ball)
//    {
//        yield return new WaitForSeconds(0.5f); // ���趯������ʱ��Ϊ0.5�룬����ʵ���������

//        Rigidbody rb = ball.GetComponent<Rigidbody>();
//        rb.AddForce(transform.forward * kickForce, ForceMode.Impulse); // �ڽ�ɫǰ��ʩ������
//        canJump = true; // �����ָ���Ծ����
//    }

//    // ��������ʾUI
//    void BallUI()
//    {
//        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

//        if (balls.Length > 0)
//        {
//            GameObject closestBall = FindClosestBall(balls);

//            if (closestBall != null)
//            {
//                float distance = Vector3.Distance(transform.position, closestBall.transform.position);

//                if (distance <= kickRange)
//                {
//                    ballUI.gameObject.SetActive(true); // ������ʱ��ʾUI
//                }
//                else
//                {
//                    ballUI.gameObject.SetActive(false); // ��������ʱ����UI
//                }
//            }
//        }
//    }
//}