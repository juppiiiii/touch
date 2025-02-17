using UnityEngine;

public class ObjectRotate : MonoBehaviour
{
    public float minRotationZ; // 최소 회전 각도
    public float maxRotationZ; // 최대 회전 각도


    public void SetRandomRotation()
    {
        float randomZ = Random.Range(minRotationZ, maxRotationZ); // 랜덤한 Z축 값 설정
        transform.Rotate(0, 0, randomZ); // Z축 회전 적용
        Debug.Log("새로운 Z 회전 값: " + randomZ);
    }
}
