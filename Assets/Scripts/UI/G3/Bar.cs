using UnityEngine;

public class Bar : MonoBehaviour
{
    public float minX = -110f;  // 최소 X 위치
    public float maxX = 110f;   // 최대 X 위치
    public float speed = 2f;    // 이동 속도 (높을수록 빠름)

    private bool isStopped = false; // 정지 여부
    private float t = 0f; // Lerp를 위한 시간 변수

    private void Update()
    {
        if (!isStopped)
        {
            Move();
        }
    }

    private void Move()
    {
        // t 값을 0~1 사이에서 반복적으로 변화시키기 위해 Mathf.PingPong 사용
        t += Time.deltaTime * speed;
        float lerpValue = Mathf.PingPong(t, 1f); // 0~1 사이 반복

        // minX와 maxX 사이를 왕복하도록 Lerp 적용
        float newX = Mathf.Lerp(minX, maxX, lerpValue);

        // 새로운 위치 적용 (Y, Z는 유지)
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    // 바 이동을 멈추는 함수
    public void Stop()
    {
        isStopped = true;
    }
}
