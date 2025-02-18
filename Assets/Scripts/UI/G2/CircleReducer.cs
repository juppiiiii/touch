using UnityEngine;

public class CircleReducer : MonoBehaviour
{
    public float reductionSpeed = 0.5f; // 줄어드는 속도
    public float minScale = 0.1f; // 최소 크기 제한

    private bool isReducing = true; // 감소 여부
    private Vector3 initialScale; // 원래 크기 저장

    void Start()
    {
        initialScale = transform.localScale; // 초기 크기 저장
    }

    // 🔹 원 크기를 줄이는 함수 (Scale 감소)
    public void Reduce()
    {
        if (transform.localScale.x > minScale && transform.localScale.y > minScale)
        {
            transform.localScale -= Vector3.one * reductionSpeed * Time.deltaTime;
        }
        else
        {
            StopReduce(); // 최소 크기에 도달하면 멈춤
        }
    }

    // 🔹 원 크기 줄이기 정지
    public void StopReduce()
    {
        isReducing = false;
    }

    // 🔹 원이 현재 줄어들고 있는지 확인
    public bool IsMoving()
    {
        return isReducing;
    }

    // 🔹 원 크기를 다시 초기 상태로 복구
    public void ResetCircle()
    {
        transform.localScale = initialScale;
        isReducing = true;
    }
    public float NowScale() { return transform.localScale.x;}
}
