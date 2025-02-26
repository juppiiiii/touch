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
    public float hungrySpawnRate = 0.3f;
}

[System.Serializable]
public class GhostlyHandSettings
{
    public GhostlyHand sidePrefab;
    public GhostlyHand ceilingPrefab;
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
                SpawnEvilSpirit(wave);
                break;
            case NightEventType.GhostlyHand:
                SpawnGhostlyHand(wave);
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
                    sleepTalkingSettings.occurrenceChance,
                    sleepTalkingSettings.interval,
                    eventType));
                break;
        }
    }

    private IEnumerator CreateEventSpawnRoutine(System.Action spawnAction, float chance, float interval, NightEventType eventType)
    {
        // 밤 시작 시 즉시 첫 이벤트 체크
        if (Random.value <= chance)
        {
            spawnAction();
        }

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
        string type = "악령";
        
        // 프리팹이 있을 때만 SpiritTypeName 사용
        if (isHungry && evilSpiritSettings.hungryPrefab != null)
        {
            type = evilSpiritSettings.hungryPrefab.SpiritTypeName;
        }
        else if (!isHungry && evilSpiritSettings.normalPrefab != null)
        {
            type = evilSpiritSettings.normalPrefab.SpiritTypeName;
        }
        
        Debug.Log($"[웨이브 {wave}] {type} 출현! (현재 활성 이벤트 수: {activeEvents.Count + 1})");
        
        if (isHungry && evilSpiritSettings.hungryPrefab != null)
        {
            EvilSpirit spawnedSpirit = Instantiate(evilSpiritSettings.hungryPrefab);
            activeEvents.Add(spawnedSpirit);
            spawnedSpirit.OnEventEnded += () => RemoveActiveEvent(spawnedSpirit);
        }
        else if (!isHungry && evilSpiritSettings.normalPrefab != null)
        {
            EvilSpirit spawnedSpirit = Instantiate(evilSpiritSettings.normalPrefab);
            activeEvents.Add(spawnedSpirit);
            spawnedSpirit.OnEventEnded += () => RemoveActiveEvent(spawnedSpirit);
        }
    }
    #endregion

    #region 손길 이벤트
    private void SpawnGhostlyHand(int wave)
    {
        bool isCeilingType = Random.value < handSettings.ceilingSpawnRate;
        string type = isCeilingType ? "천장형 손길" : "측면형 손길";
        
        Debug.Log($"[웨이브 {wave}] {type} 출현! (현재 활성 이벤트 수: {activeEvents.Count + 1})");
        
        if (isCeilingType && handSettings.ceilingPrefab != null)
        {
            GhostlyHand spawnedHand = Instantiate(handSettings.ceilingPrefab);
            activeEvents.Add(spawnedHand);
            spawnedHand.OnEventEnded += () => RemoveActiveEvent(spawnedHand);
        }
        else if (!isCeilingType && handSettings.sidePrefab != null)
        {
            GhostlyHand spawnedHand = Instantiate(handSettings.sidePrefab);
            activeEvents.Add(spawnedHand);
            spawnedHand.OnEventEnded += () => RemoveActiveEvent(spawnedHand);
        }
    }
    #endregion

    #region 뒤척이기 이벤트
    private void TriggerTossingEvent()
    {
        float penalty = tossingSettings.basePenalty;
        Debug.Log($"[뒤척이기 이벤트] 발생! (페널티: {penalty}초 감소)");
        GameManager.Instance.ReduceTimer(penalty);
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
        Debug.Log($"[잠꼬대 이벤트] 시작! (지속시간: {duration}초)");
        
        bool responded = false;
        yield return new WaitForSeconds(duration);
        
        if (!responded)
        {
            Debug.Log($"[잠꼬대 이벤트] 대응 실패! (페널티: {penaltyTime}초 감소)");
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