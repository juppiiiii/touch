using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // 씬 전환시에도 유지
    }

    // 게임 상수
    private const float DAY_DURATION = 90f;
    private const float NIGHT_DURATION = 60f;
    private const float SCENARIO_DURATION = 30f;
    private const float NIGHT_DURATION_WAVE2 = 90f;

    // 게임 변수, 프로퍼티, 가져다 쓰는 메서드
    public int CurrentWave { get; private set; } = 1;    
    public float TimerElapsed { get; private set; }
    public bool IsNight { get; private set; }

    // 게임 변수, 프로퍼티, 업데이트 가능
    public float SanityGauge { get; set; } = 100f;
    public float EndingGauge { get; set; } = 0f;

    void Start()
    {
        StartWave();
    }

    public void StartWave()
    {
        if (CurrentWave == 1 || CurrentWave == 6)
        {
            StartScenario();
        }
        else
        {
            StartDay();
        }
    }

    private void StartScenario()
    {
        StartCoroutine(GameTimer(SCENARIO_DURATION));
    }

    public void StartDay()
    {
        IsNight = false;
        StartCoroutine(GameTimer(DAY_DURATION));
    }

    public void StartNight()
    {
        IsNight = true;
        float duration = (CurrentWave == 2) ? NIGHT_DURATION_WAVE2 : NIGHT_DURATION;
        StartCoroutine(GameTimer(duration));
    }

    // 기존 WaitForSeconds를 사용한 초 단위 업데이트 대신,
    // Time.deltaTime을 이용해 매 프레임마다 타이머를 업데이트합니다.
    private IEnumerator GameTimer(float duration)
    {
        TimerElapsed = 0f;  // 타이머 초기화
        while (TimerElapsed < duration)
        {
            TimerElapsed += Time.deltaTime; // 매 프레임 경과 시간 누적
            yield return null;
        }
        TimerElapsed = duration;  // 정확한 값 보정
        OnTimeOver();
    }

    private void OnTimeOver()
    {
        if (CurrentWave == 6 && IsNight)
        {
            EndGame();
        }
        else if (IsNight)
        {
            CurrentWave++;
            StartWave();
        }
        else
        {
            StartNight();
        }
    }

    private void EndGame()
    {
        Debug.Log("게임 종료");
    }

    // 현재 밤/낮 상태와 현재 웨이브를 출력하는 메서드
    private void PrintCurrentState()
    {
        string timeOfDay = IsNight ? "밤" : "낮";
        Debug.Log($"현재는 {timeOfDay}이며, {CurrentWave} 웨이브입니다.");
    }

    // Update 호출 시, I 키를 누르면 현재 상태를 출력
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PrintCurrentState();
        }
    }

    // 다른 매니저에서 타이머 경과시간을 가져다 쓰고 싶다면 아래 메서드를 사용해도 
    public float GetTimerElapsed()
    {
        return TimerElapsed;
    }
}
