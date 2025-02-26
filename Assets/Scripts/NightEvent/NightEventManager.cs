using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이벤트 타입 열거형 추가
public enum NightEventType
{
    EvilSpirit, // 악령
    GhostlyHand, // 손길
    Tossing, // 뒤척이기
    SleepTalking // 잠꼬대
}

// 이벤트 설정을 위한 구조체들
[System.Serializable]
public class EvilSpiritSettings
{
    public EvilSpirit normalPrefab;
    public EvilSpirit hungryPrefab;
    public float spawnChance = 0.3f;
    public float spawnInterval = 30f;
    public float hungrySpawnRate = 0.3f;
}

[System.Serializable]
public class GhostlyHandSettings
{
    public GhostlyHand sidePrefab;
    public GhostlyHand ceilingPrefab;
    public float spawnChance = 0.3f;
    public float spawnInterval = 45f;
    public float ceilingSpawnRate = 0.7f;
}

[System.Serializable]
public class TossingSettings
{
    public float occurrenceChance = 0.2f;
    public float occurrenceCycle = 60f;
    public float basePenalty = 2f;
}

[System.Serializable]
public class SleepTalkingSettings
{
    public float occurrenceChance = 0.15f;
    public float interval = 90f;
    public float duration = 10f;
    public float penaltyTime = 5f;
}

public class NightEventManager : MonoBehaviour
{
    public event System.Action OnAllEventsCleared;

    [Header("이벤트 설정")]
    [SerializeField] private EvilSpiritSettings evilSpiritSettings;
    [SerializeField] private GhostlyHandSettings handSettings;
    [SerializeField] private TossingSettings tossingSettings;
    [SerializeField] private SleepTalkingSettings sleepTalkingSettings;

    // 활성화된 이벤트들 추적
    private List<NightEvent> activeEvents = new List<NightEvent>();
    
    // 코루틴 참조 저장
    private Dictionary<string, Coroutine> eventCoroutines = new Dictionary<string, Coroutine>();

    #region 초기화 및 이벤트 관리
    public void Initialize(GameManager gameManager)
    {
        gameManager.OnNightStarted += StartNightEvents;
        gameManager.OnNightEnded += StopAllNightEvents;
    }

    public void StartNightEvents()
    {
        int currentWave = GameManager.Instance.CurrentWave;
        
        // 웨이브 1은 기본적으로 이벤트 없음
        if (currentWave < 2) return;

        // 모든 이벤트 시작 코루틴
        StartEventRoutine(NightEventType.EvilSpirit, currentWave);
        StartEventRoutine(NightEventType.GhostlyHand, currentWave);
        StartEventRoutine(NightEventType.Tossing, currentWave);
        StartEventRoutine(NightEventType.SleepTalking, currentWave);
    }

    public void StopAllNightEvents()
    {
        // 모든 코루틴 정지
        foreach (var coroutine in eventCoroutines.Values)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
        eventCoroutines.Clear();

        // 모든 활성 이벤트 정리
        foreach (var nightEvent in activeEvents)
        {
            if (nightEvent != null && nightEvent.gameObject != null)
                Destroy(nightEvent.gameObject);
        }
        activeEvents.Clear();

        OnAllEventsCleared?.Invoke();
    }
    #endregion

    #region 이벤트 생성 및 난이도 조정
    private void StartEventRoutine(NightEventType eventType, int wave)
    {
        switch (eventType)
        {
            case NightEventType.EvilSpirit:
                StartCoroutine(CreateEventSpawnRoutine(
                    () => SpawnEvilSpirit(wave),
                    GetScaledChance(evilSpiritSettings.spawnChance, wave),
                    GetScaledInterval(evilSpiritSettings.spawnInterval, wave),
                    eventType));
                break;
            case NightEventType.GhostlyHand:
                StartCoroutine(CreateEventSpawnRoutine(
                    () => SpawnGhostlyHand(wave),
                    GetScaledChance(handSettings.spawnChance, wave),
                    GetScaledInterval(handSettings.spawnInterval, wave),
                    eventType));
                break;
            case NightEventType.Tossing:
                StartCoroutine(CreateEventSpawnRoutine(
                    TriggerTossingEvent,
                    tossingSettings.occurrenceChance,
                    tossingSettings.occurrenceCycle,
                    eventType));
                break;
            case NightEventType.SleepTalking:
                StartCoroutine(CreateEventSpawnRoutine(
                    () => StartSleepTalkingEvents(wave),
                    GetScaledChance(sleepTalkingSettings.occurrenceChance, wave),
                    GetScaledInterval(sleepTalkingSettings.interval, wave),
                    eventType));
                break;
        }
    }

    private IEnumerator CreateEventSpawnRoutine(System.Action spawnAction, float chance, float interval, NightEventType eventType)
    {
        while (GameManager.Instance.IsNight)
        {
            yield return new WaitForSeconds(interval);
            if (Random.value <= chance)
            {
                spawnAction();
            }
        }
    }

    private float GetScaledChance(float baseChance, int wave)
    {
        return Mathf.Min(baseChance + (wave - 2) * 0.1f, 1f);
    }

    private float GetScaledInterval(float baseInterval, int wave)
    {
        return Mathf.Max(baseInterval - (wave - 2) * 2f, baseInterval * 0.5f);
    }
    #endregion

    #region 악령 이벤트
    private void SpawnEvilSpirit(int wave)
    {
        bool isHungry = Random.value > 0.7f;
        
        EvilSpirit prefabToSpawn = isHungry ? evilSpiritSettings.hungryPrefab : evilSpiritSettings.normalPrefab;
        EvilSpirit spawnedSpirit = Instantiate(prefabToSpawn);
        
        activeEvents.Add(spawnedSpirit);
        
        spawnedSpirit.OnEventEnded += () => RemoveActiveEvent(spawnedSpirit);
    }
    #endregion

    #region 손길 이벤트
    private void SpawnGhostlyHand(int wave)
    {
        // 천장형과 측면형 중 랜덤 선택
        bool isCeilingType = Random.value < handSettings.ceilingSpawnRate;
        
        GhostlyHand prefabToSpawn = isCeilingType ? handSettings.ceilingPrefab : handSettings.sidePrefab;
        GhostlyHand spawnedHand = Instantiate(prefabToSpawn);
        
        activeEvents.Add(spawnedHand);
        
        spawnedHand.OnEventEnded += () => RemoveActiveEvent(spawnedHand);
    }
    #endregion

    #region 뒤척이기 이벤트
    private void TriggerTossingEvent()
    {
        Debug.Log("뒤척이기 이벤트 발생!");
        // 여기서 아이가 뒤척이는 애니메이션이나 효과 실행
        // 예: 카메라 흔들림, 화면 효과, 소리 등
        
        // 예시: 타이머 감소 페널티
        GameManager.Instance.ReduceTimer(tossingSettings.basePenalty);
    }
    #endregion

    #region 잠꼬대 이벤트
    private void StartSleepTalkingEvents(int wave)
    {
        float duration = sleepTalkingSettings.duration + (wave - 2);
        float penalty = sleepTalkingSettings.penaltyTime + (wave - 2);
        
        StartCoroutine(ActivateSleepTalking(duration, penalty));
    }

    private IEnumerator ActivateSleepTalking(float duration, float penaltyTime)
    {
        Debug.Log($"잠꼬대 이벤트 시작! (지속시간: {duration}초)");
        // 잠꼬대 효과 활성화 (UI, 소리 등)
        
        // 대응하지 않으면 페널티
        bool responded = false;
        
        // TODO: UI에서 플레이어가 대응 버튼 누르면 responded = true 설정
        
        yield return new WaitForSeconds(duration);
        
        if (!responded)
        {
            Debug.Log($"잠꼬대에 대응하지 않아 타이머 {penaltyTime}초 감소!");
            GameManager.Instance.ReduceTimer(penaltyTime);
        }
    }
    #endregion

    #region 유틸리티
    private void RemoveActiveEvent(NightEvent nightEvent)
    {
        if (activeEvents.Contains(nightEvent))
        {
            activeEvents.Remove(nightEvent);
        }
    }
    #endregion
}