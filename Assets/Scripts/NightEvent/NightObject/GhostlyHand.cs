using System.Collections;
using UnityEngine;

public abstract class GhostlyHand : NightEvent
{
    public enum HandState
    {
        Advancing,  // 전진
        Grabbing,   // 잡기
        Eroding,    // 침식
        Retreating  // 후퇴
    }
    
    [Header("손길 기본 속성")]
    [SerializeField] protected int advanceSpeed;
    [SerializeField] protected int retreatSpeed;
    [SerializeField] protected float bindTime;
    [SerializeField] protected float objectBindChance;
    [SerializeField] protected float toyBindChance;
    [SerializeField] protected Vector3 size;
    
    // 현재 손길 상태
    public HandState CurrentHandState { get; protected set; } = HandState.Advancing;
    
    // 손길 시각 효과 컴포넌트
    [SerializeField] protected Renderer handRenderer;
    
    // 추상 프로퍼티 - 손길 이름
    public abstract string HandTypeName { get; }
    
    protected virtual void Awake()
    {
        // 자식 클래스에서 오버라이딩할 수 있도록 가상 메서드로 선언
    }
    
    protected virtual void Start()
    {
        // 기본 크기 설정
        transform.localScale = size;
        
        // 상태 머신 시작
        StartCoroutine(StateMachine());
    }
    
    // 각 손길 타입에 맞는 상태 머신을 구현하는 추상 메서드
    protected abstract IEnumerator StateMachine();
    
    // 공통 메서드들은 그대로 유지
    protected bool TryGrabObject()
    {   
        // 장난감이 있다면 우선 장난감 잡기 시도
        bool toyPresent = true; // 장난감이 있는지 체크하는 로직 필요
        if (toyPresent && Random.value < toyBindChance)
        {
            Debug.Log("손길이 장난감을 잡았습니다!");
            return true;
        }
        
        // 다른 오브젝트 잡기 시도
        if (Random.value < objectBindChance)
        {
            Debug.Log("손길이 오브젝트를 잡았습니다!");
            return true;
        }
        
        Debug.Log("손길이 아무것도 잡지 못했습니다.");
        return false;
    }
    
    protected IEnumerator AdvancePhase(float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.forward * 5f; // 앞으로 5 유닛 이동
        
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    protected IEnumerator GrabPhase(float duration)
    {
        Debug.Log($"손길이 {duration}초 동안 물체를 잡고 있습니다.");
        // 잡은 오브젝트를 이동 불가능하게 만드는 로직
        
        yield return new WaitForSeconds(duration);
        
        Debug.Log("손길이 물체를 놓았습니다.");
        // 오브젝트 이동 가능하게 복구
    }
    
    protected IEnumerator RetreatPhase(float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos - Vector3.forward * 7f; // 뒤로 7 유닛 이동
        
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    // 플레이어가 손길을 물리치는 메서드
    public void Defeat()
    {
        StopAllCoroutines();
        // 물리칠 때 효과 (예: 애니메이션, 파티클 등)
        EndEvent();
    }
} 