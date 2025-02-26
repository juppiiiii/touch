using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightEventManager : MonoBehaviour
{
    [Header("악령 이벤트 설정")]
    [SerializeField] private EvilSpirit normalEvilSpiritPrefab;
    [SerializeField] private EvilSpirit hungryEvilSpiritPrefab;
    [SerializeField] private float evilSpiritSpawnChance = 0.3f;
    [SerializeField] private float evilSpiritSpawnInterval = 30f;

    [Header("손길 이벤트 설정")]
    [SerializeField] private GhostlyHand sideHandPrefab;
    [SerializeField] private GhostlyHand ceilingHandPrefab;
    [SerializeField] private float handSpawnChance = 0.3f;
    [SerializeField] private float handSpawnInterval = 45f;

    [Header("뒤척이기 이벤트 설정")]
    [SerializeField] private float tossingOccurrenceChance = 0.2f;
    [SerializeField] private float tossingOccurrenceCycle = 60f;

    [Header("잠꼬대 이벤트 설정")]
    [SerializeField] private float sleepTalkingChance = 0.15f;
    [SerializeField] private float sleepTalkingInterval = 90f;
    [SerializeField] private float sleepTalkingDuration = 10f;
    [SerializeField] private float sleepTalkingPenaltyTime = 5f;

    // 활성화된 이벤트들 추적
    private List<NightEvent> activeEvents = new List<NightEvent>();
    
    // 코루틴 참조 저장
    private Dictionary<string, Coroutine> eventCoroutines = new Dictionary<string, Coroutine>();

    // public으로 변경하여 GameManager에서 접근 가능하도록 함
    public void Initialize(GameManager gameManager)
    {
        gameManager.OnNightStarted += StartNightEvents;
        gameManager.OnDayStarted += StopAllNightEvents;
    }

    public void StartNightEvents()
    {
        int currentWave = GameManager.Instance.CurrentWave;
        
        // 웨이브 1은 기본적으로 이벤트 없음
        if (currentWave < 2) return;

        // 모든 이벤트 시작 코루틴
        StartEvilSpiritEvents(currentWave);
        StartHandEvents(currentWave);
        StartTossingEvents(currentWave);
        StartSleepTalkingEvents(currentWave);
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
    }

    #region 악령 이벤트
    private void StartEvilSpiritEvents(int wave)
    {   
        eventCoroutines["EvilSpirit"] = StartCoroutine(EvilSpiritSpawnRoutine(spawnChance, spawnInterval, wave));
    }

    private IEnumerator EvilSpiritSpawnRoutine(float chance, float interval, int wave)
    {
        while (GameManager.Instance.IsNight)
        {
            yield return new WaitForSeconds(interval);
            
            if (Random.value <= chance)
            {
                SpawnEvilSpirit(wave);
            }
        }
    }

    private void SpawnEvilSpirit(int wave)
    {
        bool isHungry = Random.value > 0.7f;
        
        EvilSpirit prefabToSpawn = isHungry ? hungryEvilSpiritPrefab : normalEvilSpiritPrefab;
        EvilSpirit spawnedSpirit = Instantiate(prefabToSpawn);
        
        activeEvents.Add(spawnedSpirit);
        
        spawnedSpirit.OnEventEnded += () => RemoveActiveEvent(spawnedSpirit);
    }
    #endregion

    #region 손길 이벤트
    private void StartHandEvents(int wave)
    {
        eventCoroutines["GhostlyHand"] = StartCoroutine(HandSpawnRoutine(handSpawnChance, handSpawnInterval, wave));
    }

    private IEnumerator HandSpawnRoutine(float chance, float interval, int wave)
    {
        while (GameManager.Instance.IsNight)
        {
            yield return new WaitForSeconds(interval);
            
            if (Random.value <= chance)
            {
                SpawnGhostlyHand(wave);
            }
        }
    }

    private void SpawnGhostlyHand(int wave)
    {
        // 천장형과 측면형 중 랜덤 선택
        bool isCeilingType = Random.value < 0.7f;
        
        GhostlyHand prefabToSpawn = isCeilingType ? ceilingHandPrefab : sideHandPrefab;
        GhostlyHand spawnedHand = Instantiate(prefabToSpawn);
        
        activeEvents.Add(spawnedHand);
        
        spawnedHand.OnEventEnded += () => RemoveActiveEvent(spawnedHand);
    }
    #endregion

    #region 뒤척이기 이벤트
    private void StartTossingEvents(int wave)
    {
        eventCoroutines["Tossing"] = StartCoroutine(TossingRoutine(occurrenceChance, occurrenceCycle));
    }

    private IEnumerator TossingRoutine(float chance, float cycle)
    {
        while (GameManager.Instance.IsNight)
        {
            yield return new WaitForSeconds(cycle);
            
            if (Random.value <= chance)
            {
                TriggerTossingEvent();
            }
        }
    }

    private void TriggerTossingEvent()
    {
        Debug.Log("뒤척이기 이벤트 발생!");
        // 여기서 아이가 뒤척이는 애니메이션이나 효과 실행
        // 예: 카메라 흔들림, 화면 효과, 소리 등
        
        // 예시: 타이머 감소 페널티
        GameManager.Instance.ReduceTimer(2f);
    }
    #endregion

    #region 잠꼬대 이벤트
    private void StartSleepTalkingEvents(int wave)
    {
        eventCoroutines["SleepTalking"] = StartCoroutine(SleepTalkingRoutine(occurrenceChance, interval, wave));
    }

    private IEnumerator SleepTalkingRoutine(float chance, float interval, int wave)
    {
        while (GameManager.Instance.IsNight)
        {
            yield return new WaitForSeconds(interval);
            
            if (Random.value <= chance)
            {
                float duration = sleepTalkingDuration + (wave - 2);
                float penalty = sleepTalkingPenaltyTime + (wave - 2);
                
                StartCoroutine(ActivateSleepTalking(duration, penalty));
            }
        }
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

    private void RemoveActiveEvent(NightEvent nightEvent)
    {
        if (activeEvents.Contains(nightEvent))
        {
            activeEvents.Remove(nightEvent);
        }
    }
}