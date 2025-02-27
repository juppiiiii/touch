using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{   
    [SerializeField] private NightEventManager nightEventManager;

    #region 싱글톤
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
    #endregion

    #region 상수
    private const float PREPARATION_TIME = 3f;  // 준비 시간 추가
    private const float MAX_INTERACTION_GAUGE = 100f;  // 상호작용 게이지 최댓값
    private const float MAX_EROSION_GAUGE = 180f;      // 침식 게이지 최댓값

    // 웨이브별 시간 설정을 위한 딕셔너리
    private readonly Dictionary<int, float> waveDayDurations = new Dictionary<int, float>()
    {
        { 1, 30f },  // 웨이브 1 낮 시간
        { 2, 30f },  // 웨이브 2 낮 시간
        { 3, 30f },  // 웨이브 3 낮 시간
        { 4, 30f }   // 웨이브 4 낮 시간
    };

    private readonly Dictionary<int, float> waveNightDurations = new Dictionary<int, float>()
    {
        { 1, 30f },  // 웨이브 1 밤 시간
        { 2, 30f },  // 웨이브 2 밤 시간
        { 3, 30f },  // 웨이브 3 밤 시간
        { 4, 30f }   // 웨이브 4 밤 시간
    };

    // 각 웨이브별 아이의 나이
    private const int WAVE1_AGE = 4;
    private const int WAVE2_AGE = 7;
    private const int WAVE3_AGE = 12;
    private const int WAVE4_AGE = 16;
    #endregion

    #region 프로퍼티
    public int CurrentWave { get; private set; }
    public float TimerElapsed { get; private set; }
    public bool IsNight { get; private set; }

    [SerializeField] private float interactionGauge = 0f;
    public float InteractionGauge
    {
        get => interactionGauge;
        set => interactionGauge = Mathf.Clamp(value, 0f, MAX_INTERACTION_GAUGE);
    }

    [SerializeField]private float erosionGauge = 0f;
    public float ErosionGauge
    {
        get => erosionGauge;
        set => erosionGauge = Mathf.Clamp(value, 0f, MAX_EROSION_GAUGE);
    }
    #endregion

    #region 프라이빗 변수
    private bool isPaused = false;
    private Coroutine currentTimerCoroutine;
    private Coroutine interactionGaugeCoroutine;  // 상호작용 게이지 채우기 코루틴
    private Coroutine erosionGaugeCoroutine;      // 침식 게이지 채우기 코루틴
    private Child childInstance;
    #endregion

    #region 이벤트
    public event Action OnNightStarted;
    public event Action OnNightEnded;
    #endregion

    #region 게임 진행 관련 메서드
    void Start()
    {
        nightEventManager.Initialize(this);
        CurrentWave = 1;  // 웨이브 1로 초기화
        StartWave();
    }

    public void StartWave()
    {
        StartDay();
    }

    public void StartDay()
    {
        // 현재 실행 중인 타이머가 있다면 중지
        if (currentTimerCoroutine != null)
        {
            StopCoroutine(currentTimerCoroutine);
            currentTimerCoroutine = null;
        }
        
        // 타이머 초기화
        TimerElapsed = 0f;
        
        StartCoroutine(PrepareForDay());
        IsNight = false;
    }

    private IEnumerator PrepareForDay()
    {
        Debug.Log("낮 준비 시간");
        // 자동 대기 시간 제거
        yield break;
    }

    public void StartNight()
    {
        // 현재 실행 중인 타이머가 있다면 중지
        if (currentTimerCoroutine != null)
        {
            StopCoroutine(currentTimerCoroutine);
            currentTimerCoroutine = null;
        }
        
        // 타이머 초기화
        TimerElapsed = 0f;

        StartCoroutine(PrepareForNight());
        IsNight = true;
    }

    private IEnumerator PrepareForNight()
    {
        Debug.Log("밤 준비 시간");
        // 자동 대기 시간 제거
        yield break;
    }

    private IEnumerator GameTimer(float duration)
    {
        TimerElapsed = duration;  // 시작 시간을 최대값으로 설정
        while (TimerElapsed > 0)  // 0보다 클 때까지 실행
        {
            if (!isPaused)
            {
                TimerElapsed -= Time.deltaTime;
            }
            yield return null;
        }
        TimerElapsed = 0f;  // 음수 방지
        OnTimeOver();
    }

    private void OnTimeOver()
    {
        if (CurrentWave == 4 && IsNight)
        {
            // NightEventManager에 밤 종료 알림
            if (nightEventManager != null)
            {
                nightEventManager.StopAllNightEvents();
            }
            EndGame();
        }
        else if (IsNight)
        {
            // NightEventManager에 밤 종료 알림
            if (nightEventManager != null)
            {
                nightEventManager.StopAllNightEvents();
            }
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
    #endregion

    #region 디버그 및 
    private void PrintCurrentState()
    {
        string timeOfDay = IsNight ? "밤" : "낮";
        string pauseState = isPaused ? "타이머 일시정지" : "타이머 진행중";
        Debug.Log($"[게임 상태] {CurrentWave}웨이브 / {timeOfDay} / {pauseState}");
        Debug.Log($"[타이머] 남은 시간: {TimerElapsed:F1}초");
    }

    
    // Update 호출 시, I 키를 누르면 현재 상태를 출력
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PrintCurrentState();
        }

        // 타이머 일시정지/재개 토글 (추후 삭제)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isPaused)
            {
                ResumeTimer();
                Debug.Log("타이머 재개");
            }
            else
            {
                PauseTimer();
                Debug.Log("타이머 일시정지");
            }
        }

        // 패널티 테스트
        if (Input.GetKeyDown(KeyCode.P))
        {
            ReduceTimer(5f);
            Debug.Log("패널티 적용");
        }

        // 스페이스바 누르면 낮/밤 전환 (디버그용)
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (IsNight)
            {
                if (CurrentWave < 4)
                {
                    CurrentWave++;
                    StartWave();
                    Debug.Log($"웨이브 {CurrentWave}의 낮으로 이동");
                }
                else
                {
                    EndGame();
                    Debug.Log("게임 종료");
                }
            }
            else
            {
                StartNight();
                Debug.Log("밤으로 이동");
            }
        }

        // E키 누르면 현재 준비시간 종료
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsNight)
            {
                FinishNightPreparation();
            }
            else
            {
                FinishDayPreparation();
            }
            Debug.Log("준비 시간 종료");
        }
    }
    #endregion

    #region 상태 관리
    // 다른 매니저에서 남은 시간을 가져다 쓰고 싶다면 아래 메서드를 사용
    public float GetTimerElapsed()
    {
        return TimerElapsed;
    }

    // 타이머 일시정지
    public void PauseTimer()
    {   
        isPaused = true;
    }

    // 타이머 재개
    public void ResumeTimer()
    {
        isPaused = false;
    }

    // 현재 웨이브의 아이 나이 반환
    public int GetCurrentChildAge()
    {
        return CurrentWave switch
        {
            1 => WAVE1_AGE,
            2 => WAVE2_AGE,
            3 => WAVE3_AGE,
            4 => WAVE4_AGE,
            _ => WAVE1_AGE // 기본값
        };
    }
    #endregion

    #region 게이지 관리
    public void ResetInteractionGauge()
    {
        InteractionGauge = 0f;
        StopFillingInteractionGauge();
    }

    public void ResetErosionGauge()
    {
        ErosionGauge = 0f;
        StopFillingErosionGauge();
    }

    public void ResetAllGauges()
    {
        ResetInteractionGauge();
        ResetErosionGauge();
    }

    // 상호작용 게이지 채우기 코루틴
    private IEnumerator FillInteractionGauge(float duration, float amount)
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        float elapsed = 0f;
        while (elapsed < duration)
        {   
            InteractionGauge += amount;
            elapsed += Time.deltaTime;
            yield return wait;
        }
    }

    public void StartFillingInteractionGauge(float duration, float amount)
    {
        if (interactionGaugeCoroutine != null)
        {
            StopCoroutine(interactionGaugeCoroutine);
        }
        interactionGaugeCoroutine = StartCoroutine(FillInteractionGauge(duration, amount));
    }

    public void StopFillingInteractionGauge()
    {
        if (interactionGaugeCoroutine != null)
        {
            StopCoroutine(interactionGaugeCoroutine);
            interactionGaugeCoroutine = null;
        }
    }

    // 침식 게이지 채우기 코루틴
    private IEnumerator FillErosionGauge(float duration, float amount)
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            ErosionGauge += amount;
            elapsed += Time.deltaTime;
            yield return wait;
        }
    }

    public void StartFillingErosionGauge(float duration, float amount)
    {
        if (erosionGaugeCoroutine != null)
        {
            StopCoroutine(erosionGaugeCoroutine);
        }
        erosionGaugeCoroutine = StartCoroutine(FillErosionGauge(duration, amount));
    }

    public void StopFillingErosionGauge()
    {
        if (erosionGaugeCoroutine != null)
        {
            StopCoroutine(erosionGaugeCoroutine);
            erosionGaugeCoroutine = null;
        }
    }
    #endregion

    #region 패널티
    public void ReduceTimer(float amount)
    {
        TimerElapsed = Mathf.Max(TimerElapsed - amount, 0f);
    }
    #endregion

    #region 낮/밤 준비 종료 메서드
    public void FinishDayPreparation()
    {
        if (currentTimerCoroutine != null)
        {
            StopCoroutine(currentTimerCoroutine);
        }
        ResumeTimer();
        float duration = waveDayDurations[CurrentWave];
        currentTimerCoroutine = StartCoroutine(GameTimer(duration));

        OnNightEnded?.Invoke();
        Debug.Log($"낮 시작 (지속시간: {duration}초)");
    }

    public void FinishNightPreparation()
    {
        if (currentTimerCoroutine != null)
        {
            StopCoroutine(currentTimerCoroutine);
        }
        ResumeTimer();
        float duration = waveNightDurations[CurrentWave];
        currentTimerCoroutine = StartCoroutine(GameTimer(duration));
        
        OnNightStarted?.Invoke();
        Debug.Log($"밤 시작 (지속시간: {duration}초)");
    }
    #endregion

    public Child GetChildInstance()
    {
        return childInstance;
    }
}
