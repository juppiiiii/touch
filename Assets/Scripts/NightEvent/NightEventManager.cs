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
    public GameObject normalPrefab;
    public GameObject hungryPrefab;
    public float hungrySpawnRate = 0.3f;
}

[System.Serializable]
public class GhostlyHandSettings
{
    public GameObject sidePrefab;
    public GameObject ceilingPrefab;
    public float ceilingSpawnRate = 0.7f;
}

[System.Serializable]
public class TossingSettings
{
    public float occurrenceChance = 1f;     // 50% 확률
    public float occurrenceCycle = 5f;        // 5초마다 체크
    public float basePenalty = 2f;
}

[System.Serializable]
public class SleepTalkingSettings
{
    public float occurrenceChance = 1f;     // 40% 확률
    public float interval = 3f;               // 3초마다 체크
    public float duration = 3f;               // 3초 동안 지속
    public float penaltyTime = 5f;            // 5초 패널티
}

public class NightEventManager : MonoBehaviour
{
    public event System.Action OnAllEventsCleared;

    [Header("이벤트 설정")]
    [SerializeField] private EvilSpiritSettings evilSpiritSettings;
    [SerializeField] private GhostlyHandSettings handSettings;
    [SerializeField] private TossingSettings tossingSettings;
    [SerializeField] private SleepTalkingSettings sleepTalkingSettings;

    [Header("Child Prefabs")]
    [SerializeField] private GameObject toddlerPrefab;    // 1-2웨이브용 아이
    [SerializeField] private GameObject teenagerPrefab;   // 3-4웨이브용 아이

    // 활성화된 이벤트들 추적
    private List<NightEvent> activeEvents = new List<NightEvent>();
    
    // 코루틴 참조 저장
    private Dictionary<string, Coroutine> eventCoroutines = new Dictionary<string, Coroutine>();

    private Child childInstance;

    #region 초기화 및 이벤트 관리
    public void Initialize(GameManager gameManager)
    {
        gameManager.OnNightStarted += StartNightEvents;
        gameManager.OnNightEnded += StopAllNightEvents;
        childInstance = gameManager.GetChildInstance(); // Child 인스턴스 참조 가져오기
    }

    public void StartNightEvents()
    {
        int currentWave = GameManager.Instance.CurrentWave;

        // 현재 웨이브에 맞는 아이 스폰
        SpawnChild(currentWave);

        // 아이 활성화
        if (childInstance != null)
        {
            childInstance.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Child 인스턴스가 없습니다!");
            return;
        }

        // 기존 이벤트 시작 코드
        StartEventRoutine(NightEventType.EvilSpirit, currentWave);
        StartEventRoutine(NightEventType.GhostlyHand, currentWave);
        StartEventRoutine(NightEventType.Tossing, currentWave);
    }

    public void StopAllNightEvents()
    {
        // 아이 비활성화
        if (childInstance != null)
        {
            childInstance.gameObject.SetActive(false);
        }

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
        string type = isHungry ? "배고픈 악령" : "일반 악령";
        
        Debug.Log($"[웨이브 {wave}] {type} 출현! (현재 활성 이벤트 수: {activeEvents.Count + 1})");
        
        if (isHungry && evilSpiritSettings.hungryPrefab != null)
        {
            EvilSpirit spawnedSpirit = Instantiate(evilSpiritSettings.hungryPrefab).GetComponent<EvilSpirit>();
            activeEvents.Add(spawnedSpirit);
            spawnedSpirit.OnEventEnded += () => RemoveActiveEvent(spawnedSpirit);
        }
        else if (!isHungry && evilSpiritSettings.normalPrefab != null)
        {
            EvilSpirit spawnedSpirit = Instantiate(evilSpiritSettings.normalPrefab).GetComponent<EvilSpirit>();
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
            GhostlyHand spawnedHand = Instantiate(handSettings.ceilingPrefab).GetComponent<GhostlyHand>();
            activeEvents.Add(spawnedHand);
            spawnedHand.OnEventEnded += () => RemoveActiveEvent(spawnedHand);
        }
        else if (!isCeilingType && handSettings.sidePrefab != null)
        {
            GhostlyHand spawnedHand = Instantiate(handSettings.sidePrefab).GetComponent<GhostlyHand>();
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
        
        if (childInstance != null)
        {
            childInstance.PlayTossingAnimation();
            StartCoroutine(ReturnToSleeping(0.5f));
            
            // 뒤척이기 후 잠꼬대 발생 확률 체크
            if (Random.value <= sleepTalkingSettings.occurrenceChance)
            {
                StartCoroutine(DelayedSleepTalking());
            }
        }
        
        GameManager.Instance.ReduceTimer(penalty);
    }

    // 뒤척이기 후 약간의 딜레이를 두고 잠꼬대 시작
    private IEnumerator DelayedSleepTalking()
    {
        // 뒤척이기 애니메이션이 끝나고 잠꼬대 시작
        yield return new WaitForSeconds(0.5f);
        StartSleepTalkingEvents(GameManager.Instance.CurrentWave);
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
        
        if (childInstance != null)
        {
            // 현재 웨이브가 1-2웨이브(toddler)인 경우 눈 비비기 애니메이션
            if (GameManager.Instance.CurrentWave <= 2)
            {
                childInstance.PlayRubbingEyesAnimation();
            }
            else
            {
                childInstance.PlaySleepingAnimation();
            }
        }
        
        bool responded = false;
        yield return new WaitForSeconds(duration);
        
        if (!responded)
        {
            Debug.Log($"[잠꼬대 이벤트] 대응 실패! (페널티: {penaltyTime}초 감소)");
            GameManager.Instance.ReduceTimer(penaltyTime);
        }

        // 이벤트 종료 후 기본 수면 상태로 복귀
        if (childInstance != null)
        {
            childInstance.PlaySleepingAnimation();
        }
    }
    #endregion

    #region 유틸리티
    // 기본 수면 상태로 돌아가는 코루틴
    private IEnumerator ReturnToSleeping(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (childInstance != null)
        {
            childInstance.PlaySleepingAnimation();
        }
    }

    private void RemoveActiveEvent(NightEvent nightEvent)
    {
        if (activeEvents.Contains(nightEvent))
        {
            activeEvents.Remove(nightEvent);
        }
    }

    private void SpawnChild(int currentWave)
    {
        // 기존 Child 인스턴스가 있다면 제거
        if (childInstance != null)
        {
            Destroy(childInstance.gameObject);
        }

        // Bed 오브젝트를 이름으로 찾기
        GameObject bed = GameObject.Find("Bed");
        if (bed == null)
        {
            Debug.LogError("Scene에서 Bed 오브젝트를 찾을 수 없습니다!");
            return;
        }

        // 웨이브에 따라 적절한 프리팹 선택
        GameObject prefabToSpawn = (currentWave <= 2) ? toddlerPrefab : teenagerPrefab;
        
        if (prefabToSpawn != null)
        {
            // Bed 위치에 아이 스폰
            GameObject childObject = Instantiate(prefabToSpawn, bed.transform.position, Quaternion.identity);
            childObject.transform.SetParent(bed.transform); // Bed의 자식으로 설정
            
            childInstance = childObject.GetComponent<Child>();
            
            if (childInstance != null)
            {
                childInstance.Initialize(currentWave);
                childInstance.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("생성된 프리팹에 Child 컴포넌트가 없습니다!");
            }
        }
        else
        {
            Debug.LogError($"Wave {currentWave}에 해당하는 Child 프리팹이 할당되지 않았습니다!");
        }
    }
    #endregion
}