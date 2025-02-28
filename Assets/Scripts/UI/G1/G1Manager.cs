using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class G1Manager : MonoBehaviour
{
    public ObjectRotate objectRotate;  // 게이지 회전
    public ObjectRotate object2Rotate;  // 게이지 안 하얀색 회전
    public NeedleRotator needleRotator;  // 바늘 회전
    public NeedleCollision needleCollision; // 점수 시스템

    public Image failCircle; // 실패 시 표시할 Image UI
    public Image successCircle; // 성공 시 표시할 Image UI

    public GameObject G1; // 전체 UI 요소 (게임 종료 시 비활성화)
    public Image Circle; // Circle UI의 Image 컴포넌트

    public int successScore = 5; // 성공 기준 점수
    public int failScore = 3; // 실패 기준 점수

    private int sScore = 0; // 성공 점수
    private int fScore = 0; // 실패 점수

    private bool isProcessing = false; // 회전 처리 중인지 확인

    public bool win;
    void Start()
    {
        // 게임이 시작될 때 회전 초기화
        if (objectRotate != null)
            objectRotate.SetRandomRotation();

        if (object2Rotate != null)
            object2Rotate.SetRandomRotation();

        // 성공/실패 UI 초기 상태를 비활성화
        if (failCircle != null)
            failCircle.gameObject.SetActive(false);

        if (successCircle != null)
            successCircle.gameObject.SetActive(false);

        Debug.Log("초기 성공 점수: " + sScore);
    }
    
    void Update()
    {
        if (needleRotator != null)
        {
            needleRotator.Rotate();
        }
        
        // 마우스 클릭 시 바늘 회전 정지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            needleRotator.StopRotation();
        }

        if (!needleRotator.IsMoving() && !isProcessing)
        {
            isProcessing = true;

            if (needleCollision != null && needleCollision.hasCollided)
            {
                if (needleCollision.IsWhiteHit())
                {
                    sScore += 2;
                    Debug.Log("White 충돌! +2점 / 현재 점수: " + sScore);
                }
                else if (needleCollision.IsBlackHit())
                {
                    sScore += 1;
                    Debug.Log("Black 충돌! +1점 / 현재 점수: " + sScore);
                }
                needleCollision.ResetCollision(); // 충돌 정보 초기화
            }
            else
            {
                fScore++;
                Debug.Log("Miss! 실패 점수: " + fScore);
                needleCollision.ResetCollision(); // 충돌 정보 초기화
            }

            // 충돌 감지 및 점수 업데이트 완료 후 1초 후 회전
            StartCoroutine(SetRotation());
        }

        if (sScore >= successScore)
        {
            Debug.Log("S");
            StartCoroutine(Success());
        }
        else if (fScore >= failScore)
        {
            Debug.Log("F");
            StartCoroutine(Fail());
        }

        
    }

    // ✅ 성공 시 처리 (Image UI를 활성화)
    IEnumerator Success()
    {
        win = true; // send to ObjectManager
        isProcessing = true; // SetRotation 실행 방지

        if (needleRotator != null)
            needleRotator.StopRotation(); // 바늘 회전 멈추기

        if (successCircle != null)
            successCircle.gameObject.SetActive(true); // 성공 UI 표시
        
        yield return new WaitForSeconds(3); // 3초 대기

        if (G1 != null)
            G1.SetActive(false); // G1 UI 비활성화

        if (successCircle != null)
            successCircle.gameObject.SetActive(false); // 성공 UI 숨기기
    }

    // ✅ 실패 시 처리 (Image UI를 활성화)
    IEnumerator Fail()
    {
        win = false; // 오브젝트매니저에 보낼 값
        isProcessing = true; // SetRotation 실행 방지

        if (needleRotator != null)
            needleRotator.StopRotation(); // 바늘 회전 멈추기

        if (failCircle != null)
            failCircle.gameObject.SetActive(true); // 실패 UI 표시

        yield return new WaitForSeconds(3); // 3초 대기

        if (G1 != null)
            G1.SetActive(false); // G1 UI 비활성화

        if (failCircle != null)
            failCircle.gameObject.SetActive(false); // 실패 UI 숨기기
    }

    // ✅ 충돌 감지 후 1초 후 회전 (중복 실행 방지)
    IEnumerator SetRotation()
    {
        yield return new WaitForSeconds(1); // 1초 대기
        if (object2Rotate != null && objectRotate != null)
        {
            object2Rotate.transform.rotation = objectRotate.transform.rotation;
        }
        if (objectRotate != null)
            objectRotate.SetRandomRotation();

        if (object2Rotate != null)
            object2Rotate.SetRandomRotation();

        if (needleRotator != null)
            needleRotator.ResumeRotation();

        isProcessing = false; // 회전 완료 후 다시 활성화
    }
}
