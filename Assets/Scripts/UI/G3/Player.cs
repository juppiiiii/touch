using UnityEngine;

public class Player : MonoBehaviour
{
    public float jumpSpeed = 15f;  // 점프 초기 속도
    public float fallSpeed = 10f;  // 하강 속도
    private float verticalVelocity = 0f; // 현재 Y 이동 속도
    public float deadZone = -300f;

    private Collider playerCollider;
    private bool isJumping = false;
    private bool isPlay = false;
    private bool isWin = false;
    private bool isFall = false;
    private Transform originalParent;

    private void Start()
    {
        isPlay = true;
        playerCollider = GetComponent<Collider>();
        originalParent = transform.parent;
    }

    private void Update()
    {
        if (isPlay) 
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isJumping) 
            {
                Jump();
            }
        }

        // **점프 중이면 Y 위치 직접 변경**
        if (isJumping)
        {
            transform.position += new Vector3(0, verticalVelocity * Time.deltaTime, 0);
            verticalVelocity -= fallSpeed * Time.deltaTime; // 점점 느려지면서 하강

            if (verticalVelocity <= 0) 
            {
                playerCollider.isTrigger = false; // 내려올 때 충돌 활성화
            }
        }
    }

    private void Jump()
    {
        isJumping = true;
        transform.parent = originalParent; // 부모 해제

        // **즉시 점프 속도 적용**
        verticalVelocity = jumpSpeed;
        playerCollider.isTrigger = true; // 점프 중 충돌 OFF
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bar"))
        {
            isJumping = false;
            verticalVelocity = 0; // 착지 시 속도 초기화
            transform.parent = collision.transform; // Bar와 함께 이동
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.name == "Goal")
        {
            isWin = true;
        }
        if (other.gameObject.name == "Zone") {
            isFall = true;
        }
    }

    public void Stop() 
    {
        isPlay = false;
    }

    public bool IsWin() 
    {
        return isWin;
    }
    public bool IsFall()
    {   
        return isFall;
    }
}
