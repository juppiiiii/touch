using UnityEngine;

public class NeedleCollision : MonoBehaviour
{
    public bool hasCollided = false; // 충돌 감지 여부
    private bool isBlack = false;
    private bool isWhite = false;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌 감지됨: " + other.name);
        hasCollided = true; // 충돌 감지 플래그 설정    

        if (other.name == "Black") // 특정 오브젝트와 충돌하면
        {
            isBlack = true;
        }
        else if (other.name == "White") // 특정 오브젝트와 충돌하면
        {
            isWhite = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Black")
        {
            isBlack = false;
        }
        else if (other.name == "White")
        {
            isWhite = false;
        }
    }

    // ✅ G1Manager에서 접근할 수 있도록 Getter 함수 추가
    public bool IsWhiteHit() { return isWhite; }
    public bool IsBlackHit() { return isBlack; }
    public void ResetCollision() { hasCollided = false; isBlack = false; isWhite = false; }
}
