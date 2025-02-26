using System.Collections;
using UnityEngine;

public class GhostlyHandCeiling : GhostlyHand
{
    public override string HandTypeName => "천장형 손길";
    private float waitTime;
    
    [SerializeField] private float erosionSpeed;  // 천장형 손길만의 침식 속도

    protected override void Awake()
    {
        base.Awake();
        advanceSpeed = 8;
        retreatSpeed = 15;

        waitTime = 10f;
        bindTime = 5f;
    
        objectBindChance = 0.65f;
        toyBindChance = 0.9f;
    }
    
    protected override IEnumerator StateMachine()
    {
        // 손길이 활성화되면 전진 상태로 시작
        Activate();
        CurrentHandState = HandState.Advancing;
        
        // 전진 페이즈
        float advanceTime = Random.Range(2f, 4f);
        yield return StartCoroutine(AdvancePhase(advanceTime));
        
        // 천장형: 대기 후 잡기 시도
        yield return new WaitForSeconds(waitTime);
        
        CurrentHandState = HandState.Grabbing;
        if (TryGrabObject())
        {
            yield return StartCoroutine(GrabPhase(bindTime));
        }
        else
        {
            // 잡기 실패 시 짧게 대기
            yield return new WaitForSeconds(1f);
        }
        
        // 후퇴 페이즈
        CurrentHandState = HandState.Retreating;
        float retreatTime = Random.Range(1.5f, 3f);
        yield return StartCoroutine(RetreatPhase(retreatTime));
        
        // 이벤트 종료
        EndEvent();
    }
} 