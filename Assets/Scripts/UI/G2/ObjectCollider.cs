using UnityEngine;

public class ObjectCollider : MonoBehaviour
{
public bool hasCollided = false; // 충돌 감지 여부
    private bool isCircle = false;
    private bool isFail = true;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌 감지됨: " + other.name);
        hasCollided = true; // 충돌 감지 플래그 설정    

        if (other.name == "movingCircle") // 특정 오브젝트와 충돌하면
        {
            isCircle = true;
        }
        else if (other.name == "failColl") // 특정 오브젝트와 충돌하면
        {
            isFail = true;
        }
        
    }
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("충돌 탈출됨: " + other.name);
        if (other.name == "movingCircle")
        {
            isCircle = false;
            isFail = true;
        }
        else if (other.name == "failColl")
        {
            isFail = false;
        }
    }

    // ✅ G1Manager에서 접근할 수 있도록 Getter 함수 추가
    public bool isCircleHit() { return isCircle; }
    public bool isFailHit() { return isFail; }
    public void ResetCollision() { hasCollided = false; isCircle = false; isFail = false;}
}
