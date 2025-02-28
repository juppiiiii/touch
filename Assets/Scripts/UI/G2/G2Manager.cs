using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class G2Manager : MonoBehaviour
{
    public CircleReducer circleReducer;  // 원 축소
    public ObjectCollider circleColl;    // 성공 충돌 감지
    public ObjectCollider failColl;      // 실패 충돌 감지

    public Sprite[] qwerSprites; // Q, W, E, R 키 입력 시 변경할 스프라이트 배열
    public Image qwerDisplayImage; // 변경할 UI 이미지

    public Image failCircle;   // 실패 시 표시할 Image UI
    public Image successCircle;// 성공 시 표시할 Image UI

    public GameObject G2; // 전체 UI 요소 (게임 종료 시 비활성화)

    public int successScore = 3; // 성공 기준 점수
    public int failScore = 3;    // 실패 기준 시도 횟수 (tryScore 기준)

    private int sScore = 0; // 성공 점수
    private int tryScore = 0; // 실패 점수
    private KeyCode currentKey; // 현재 입력해야 할 키

    private bool isBreak = false; // 성공 또는 실패 시 중단
    private bool isProcessing = false; // 키 입력 중인지 확인
    private bool hasReducedTooSmall = false; // NowScale 체크 후 중복 실행 방지

    public bool win;

    void Start()
    {
        if (failCircle != null)
            failCircle.gameObject.SetActive(false);

        if (successCircle != null)
            successCircle.gameObject.SetActive(false);

        if (qwerDisplayImage != null)
            qwerDisplayImage.sprite = null; // 시작 시 기본 이미지 비우기

        StartGame(); // 게임 시작 시 랜덤 스프라이트 설정
    }

    void Update()
    {
        if (circleReducer.IsMoving())
        {
            circleReducer.Reduce();
        }

        if (circleReducer.NowScale() <= 1.1f && !hasReducedTooSmall)
        {
            tryScore++;
            hasReducedTooSmall = true; // 중복 실행 방지
            StartCoroutine(HandleKeyPress());
        }
        else if (circleReducer.NowScale() > 1.1f)
        {
            hasReducedTooSmall = false; // 크기가 다시 커지면 다시 체크 가능하도록 설정
        }

        if (!isProcessing && !isBreak) 
        {
            if (Input.anyKeyDown) // 아무 키나 입력하면 확인
            {
                CheckKeyInput();
            }
        }

        if (sScore >= successScore)
        {
            isProcessing = true;
            isBreak = true;
            sScore = 0;
            StartCoroutine(Success());
        }
        else if (tryScore - sScore >= failScore && !isBreak)
        {
            isBreak = true;
            tryScore = 0; 
            StartCoroutine(Fail());
        }
    }

    void StartGame()
    {
        SetRandomKey(); // 게임 시작 시 랜덤 스프라이트 설정
    }

    void SetRandomKey()
    {
        if (qwerSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, qwerSprites.Length); // 랜덤 스프라이트 선택
            qwerDisplayImage.sprite = qwerSprites[randomIndex];

            switch (randomIndex)
            {
                case 0: currentKey = KeyCode.Q; break;
                case 1: currentKey = KeyCode.W; break;
                case 2: currentKey = KeyCode.E; break;
                case 3: currentKey = KeyCode.R; break;
                default: currentKey = KeyCode.None; break;
            }

            RandomizeQWERPosition(); // 🔹 QWER 이미지 위치 랜덤 변경
        }
    }

    void RandomizeQWERPosition()
    {
        if (qwerDisplayImage != null)
        {
            RectTransform rectTransform = qwerDisplayImage.rectTransform;

            // UI 내에서 랜덤한 위치 설정 (예: 화면 중앙에서 ±200px 범위 내)
            float randomX = Random.Range(-200f, 200f);
            float randomY = Random.Range(-150f, 150f);

            rectTransform.anchoredPosition = new Vector2(randomX, randomY);
        }
    }

    void CheckKeyInput()
    {
        if (Input.GetKeyDown(currentKey)) // 올바른 키 입력 시
        {
            isProcessing = true;
            ProcessKeyPress();
        }
        else // 잘못된 키 입력 시 실패 처리
        {
            tryScore++;
            StartCoroutine(HandleKeyPress());
        }
    }

    void ProcessKeyPress()
    {
        isProcessing = true;

        if (circleColl != null && circleColl.hasCollided)
        {
            if (circleColl.isCircleHit())
            {
                sScore += 1;
            }
            circleColl.ResetCollision();
        }

        StartCoroutine(HandleKeyPress());
    }

    IEnumerator HandleKeyPress()
    {
        isProcessing = true;
        circleReducer.StopReduce();

        yield return new WaitForSeconds(1);

        if (!isBreak)
        {
            circleReducer.ResetCircle();
            SetRandomKey(); // 원이 초기화되면 새로운 랜덤 키 설정
        }

        isProcessing = false;
    }

    IEnumerator Success()
    {
        win = true;
        isProcessing = true;
        circleReducer.StopReduce();
        successCircle.gameObject.SetActive(true);

        yield return new WaitForSeconds(3);

        G2.SetActive(false);
        successCircle.gameObject.SetActive(false);

        isBreak = false;
        circleReducer.ResetCircle();
        SetRandomKey(); // 성공 후 새로운 키 설정
        isProcessing = false;
    }

    IEnumerator Fail()
    {
        win = false;
        isProcessing = true;
        circleReducer.StopReduce();
        failCircle.gameObject.SetActive(true);

        yield return new WaitForSeconds(3);

        G2.SetActive(false);
        failCircle.gameObject.SetActive(false);

        isBreak = false;
        circleReducer.ResetCircle();
        SetRandomKey(); // 실패 후 새로운 키 설정
        isProcessing = false;
    }
}
