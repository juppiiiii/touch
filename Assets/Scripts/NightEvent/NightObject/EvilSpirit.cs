using System.Collections;
using UnityEngine;

public abstract class EvilSpirit : NightEvent
{
    public enum SpiritState
    {
        Forwarding,  // 전진
        Retreating, // 후퇴
        Eroding     // 침식
    }
    
    [Header("악령 기본 속성")]
    [SerializeField] protected int forwardSpeed;
    [SerializeField] protected int retreatSpeed;
    [SerializeField] protected float erosionAmount;
    [SerializeField] protected Vector3 size;

    // 현재 악령 상태
    public SpiritState CurrentSpiritState { get; protected set; } = SpiritState.Forwarding;
    
    // 악령 시각 효과 컴포넌트
    [SerializeField] protected Renderer spiritRenderer;
    
    // 악령 타입 이름 (추상 프로퍼티로 변경)
    public abstract string SpiritTypeName { get; }

    // 자식 클래스에서 오버라이딩할 수 있도록 가상 메서드로 선언
    protected virtual void Awake()
    {
    }
    
    protected virtual void Start()
    {
        // 기본 크기 설정
        transform.localScale = size;
        
        // 상태 머신 시작
        StartCoroutine(StateMachine());
    }
    
    private IEnumerator StateMachine()
    {
        // 악령이 활성화되면 전진 상태로 시작
        Activate();
        CurrentSpiritState = SpiritState.Forwarding;
        
        // 전진 페이즈
        float forwardTime = Random.Range(3f, 5f);
        yield return StartCoroutine(ForwardePhase(forwardTime));
        
        // 침식 페이즈 (플레이어가 막지 않았다고 가정)
        CurrentSpiritState = SpiritState.Eroding;
        float erosionTime = Random.Range(2f, 4f);
        yield return StartCoroutine(ErodePhase(erosionTime));
        
        // 후퇴 페이즈
        CurrentSpiritState = SpiritState.Retreating;
        float retreatTime = Random.Range(2f, 3f);
        yield return StartCoroutine(RetreatPhase(retreatTime));
        
        // 이벤트 종료
        EndEvent();
    }
    
    private IEnumerator ForwardePhase(float duration)
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
    
    private IEnumerator ErodePhase(float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            // 침식 게이지 증가
            GameManager.Instance.ErosionGauge += erosionAmount * Time.deltaTime;
            
            // 침식 시각 효과 (예: 깜빡임, 색상 변경 등)
            if (spiritRenderer != null)
            {
                float pulse = Mathf.PingPong(elapsed * 5f, 1f);
                spiritRenderer.material.color = Color.Lerp(Color.red, Color.black, pulse);
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    private IEnumerator RetreatPhase(float duration)
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
    
    // 플레이어가 악령을 물리치는 메서드
    public void Defeat()
    {
        StopAllCoroutines();
        // 물리칠 때 효과 (예: 애니메이션, 파티클 등)
        EndEvent();
    }
} 