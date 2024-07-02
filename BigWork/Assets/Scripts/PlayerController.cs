using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rigid; //玩家刚体
    public float moveSpeed = 2f; // 初始移动速度
    public float rotateSpeed = 90f;
    public float accelerationSpeed = 2f; // 初始加速度
    private bool isGround;
    public float jumpForce = 5f;
    private Animator animator; //玩家动画机
    public float kickForce = 10f; // 踢球力量
    public float kickRange = 1f; // 角色与球的最大距离
    private bool canJump = true; // 控制角色是否可以跳跃
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

    //移动
    void Move()
    {
        float h = Input.GetAxis("Horizontal"); //获取水平/垂直移动参数
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

            // 加速
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

    //跳跃
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround && canJump)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walking", false);
            animator.SetTrigger("Jump");
            rigid.AddForce(Vector3.up * jumpForce);
            isGround = false;
            canJump = false; // 设置为false，禁用跳跃
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGround)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGround = true;
                canJump = true; // 如果角色着陆在地面上，允许跳跃

            }
        }
    }



    //踢球
    void KickBall()
    {
        // 获取所有球体对象
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

        if (balls.Length > 0)
        {
            GameObject closestBall = FindClosestBall(balls);

            if (closestBall != null)
            {
                float distance = Vector3.Distance(transform.position, closestBall.transform.position);

                if (distance <= kickRange)
                {

                    canJump = true; // 在踢球前恢复跳跃功能
                    animator.SetTrigger("KickBall"); // 播放踢球动画

                    StartCoroutine(KickAfterAnimation(closestBall)); // 在动画播放完毕后踢球
                }
            }

        }
    }
    //找到所有标签为Ball的物体
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
        yield return new WaitForSeconds(0.5f); // 假设动画播放时间为0.5秒，根据实际情况调整

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * kickForce, ForceMode.Impulse); // 在角色前方施加力量
        canJump = true; // 踢球后恢复跳跃功能


    }

    //靠近球显示UI
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
//    private Rigidbody rigid; // 玩家刚体
//    public float moveSpeed = 2f; // 初始移动速度
//    public float rotateSpeed = 90f;
//    public float accelerationSpeed = 2f; // 初始加速度
//    private bool isGround;
//    public float jumpForce = 5f;
//    private Animator animator; // 玩家动画机
//    public float kickForce = 10f; // 踢球力量
//    public float kickRange = 1f; // 角色与球的最大距离
//    private bool canJump = true; // 控制角色是否可以跳跃
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
//        BallUI(); // 持续检查与球的距离
//    }

//    // 移动
//    void Move()
//    {
//        float h = Input.GetAxis("Horizontal"); // 获取水平/垂直移动参数
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

//            // 加速
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

//    // 跳跃
//    void Jump()
//    {
//        if (Input.GetKeyDown(KeyCode.Space) && isGround && canJump)
//        {
//            animator.SetBool("Idle", false);
//            animator.SetBool("Walking", false);
//            animator.SetTrigger("Jump");
//            rigid.AddForce(Vector3.up * jumpForce);
//            isGround = false;
//            canJump = false; // 设置为false，禁用跳跃
//        }
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (!isGround)
//        {
//            if (collision.gameObject.CompareTag("Ground"))
//            {
//                isGround = true;
//                canJump = true; // 如果角色着陆在地面上，允许跳跃
//            }
//        }
//    }

//    // 踢球
//    void KickBall()
//    {
//        // 获取所有球体对象
//        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

//        if (balls.Length > 0)
//        {
//            GameObject closestBall = FindClosestBall(balls);

//            if (closestBall != null)
//            {
//                float distance = Vector3.Distance(transform.position, closestBall.transform.position);

//                if (distance <= kickRange)
//                {
//                    canJump = true; // 在踢球前恢复跳跃功能
//                    animator.SetTrigger("KickBall"); // 播放踢球动画

//                    StartCoroutine(KickAfterAnimation(closestBall)); // 在动画播放完毕后踢球
//                }
//            }
//        }
//    }

//    // 找到最近的球
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
//        yield return new WaitForSeconds(0.5f); // 假设动画播放时间为0.5秒，根据实际情况调整

//        Rigidbody rb = ball.GetComponent<Rigidbody>();
//        rb.AddForce(transform.forward * kickForce, ForceMode.Impulse); // 在角色前方施加力量
//        canJump = true; // 踢球后恢复跳跃功能
//    }

//    // 靠近球显示UI
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
//                    ballUI.gameObject.SetActive(true); // 靠近球时显示UI
//                }
//                else
//                {
//                    ballUI.gameObject.SetActive(false); // 不靠近球时隐藏UI
//                }
//            }
//        }
//    }
//}