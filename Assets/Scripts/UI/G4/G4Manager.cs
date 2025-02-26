using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class G4Manager : MonoBehaviour
{
    public DiceManager diceManager; // 주사위 관리
    public Sprite[] sprites; // 실패 시 바뀌는 이미지 배열
    public Image image; // UI에 표시할 이미지
    public Text text; // 실패 횟수를 표시할 텍스트
    public Image spaceBar; // 스페이스 바 UI 이미지
    public int goal = 13; // 목표 값
    public int maxFails = 3; // 최대 실패 횟수
    private int failCount = 0; // 실패 횟수 카운트
    private bool isRolling;

    public GameObject G4; // 게임 오브젝트 그룹
    private Vector3 originalG4Position; // G4의 원래 위치 저장

    public Color normalColor = Color.white; // 기본 색상
    public Color failColor = Color.red; // 실패 시 색상

    private void Start()
    {
        originalG4Position = G4.transform.localPosition; // G4의 초기 위치 저장
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스 바로 주사위 굴리기
        {
            StartCoroutine(CheckDiceResults());
        }
    }
    private IEnumerator CheckDiceResults()
    {
        if (isRolling) yield break; // 이미 진행 중이면 실행 방지
        isRolling = true; // 실행 중 상태 설정

        diceManager.RollDice(); // 주사위 굴리기

        // 모든 주사위가 멈출 때까지 대기
        while (!diceManager.AllDicesStopped())
        {
            yield return new WaitForSeconds(0.5f); // 0.5초 간격으로 체크
        }

        // 모든 주사위 값이 저장될 때까지 대기
        while (!AllResultsStored())
        {
            yield return new WaitForSeconds(0.5f); // 0.5초 간격으로 값 저장 여부 확인
        }

        int totalValue = diceManager.GetTotalDiceValue(); // 주사위 총합 계산

        if (totalValue >= goal)
        {
            StartCoroutine(Success());
        }
        else
        {
            failCount++;
            Debug.Log($"실패 {failCount}회 (총합: {totalValue})");

            UpdateColors(); // 실패 횟수에 따라 색상 변경
            StartCoroutine(ShakeG4()); // G4 흔들기

            if (failCount <= sprites.Length)
            {
                image.sprite = sprites[failCount - 1]; // 실패 이미지 변경
            }

            if (failCount >= maxFails)
            {
                StartCoroutine(Fail());
            }
        }

        isRolling = false; // 모든 과정이 끝나면 다시 실행 가능 상태로 변경
}

    private void UpdateColors()
    {
        float failRatio = (float)failCount / maxFails; // 실패 횟수 비율 계산
        Color newColor = Color.Lerp(normalColor, failColor, failRatio); // 점점 더 붉어지게 설정

        // 주사위 색상 변경
        foreach (Dice dice in diceManager.dices)
        {
            MeshRenderer meshRenderer = dice.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material.color = newColor; // 주사위 메테리얼 색상 변경
            }
        }

        // 텍스트 색상 변경
        if (text != null)
        {
            text.color = newColor;
        }

        // 스페이스 바 UI 색상 변경
        if (spaceBar != null)
        {
            spaceBar.color = newColor;
        }
    }

    private IEnumerator ShakeG4()
    {
        float shakeDuration = 0.5f; // 흔들리는 시간
        float shakeMagnitude = 0.1f * failCount; // 실패 횟수에 따라 흔들리는 강도 증가
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float xOffset = Random.Range(-shakeMagnitude, shakeMagnitude);
            float yOffset = Random.Range(-shakeMagnitude, shakeMagnitude);

            G4.transform.localPosition = originalG4Position + new Vector3(xOffset, yOffset, 0);
            elapsed += Time.deltaTime;

            yield return null; // 한 프레임 대기
        }

        G4.transform.localPosition = originalG4Position; // 흔들림이 끝나면 원래 위치로 복귀
    }

    private IEnumerator Success()
    {
        Debug.Log("성공!");
        yield return new WaitForSeconds(3);
        ResetColors(); // 게임 성공 시 색상 초기화
        G4.SetActive(false);
    }

    private IEnumerator Fail()
    {
        Debug.Log("실패!");
        yield return new WaitForSeconds(3);
        ResetColors(); // 게임 실패 시 색상 초기화
        G4.SetActive(false);
    }

    private void ResetColors()
    {
        foreach (Dice dice in diceManager.dices)
        {
            MeshRenderer meshRenderer = dice.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material.color = normalColor; // 주사위 색상 초기화
            }
        }

        if (text != null)
        {
            text.color = normalColor; // 텍스트 색상 초기화
        }

        if (spaceBar != null)
        {
            spaceBar.color = normalColor; // 스페이스 바 색상 초기화
        }
    }

    private bool AllResultsStored()
    {
        foreach (Dice dice in diceManager.dices)
        {
            if (!diceManager.HasDiceResult(dice)) // 값이 저장되지 않은 주사위가 있으면 false 반환
            {
                return false;
            }
        }
        return true; // 모든 주사위 값이 저장됨
    }
}
