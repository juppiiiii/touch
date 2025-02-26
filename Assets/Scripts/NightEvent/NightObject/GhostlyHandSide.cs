using System.Collections;
using UnityEngine;

public class GhostlyHandSide : GhostlyHand
{
    public override string HandTypeName => "측면형 손길";
    private int erosionSpeed;
    
    protected override void Awake()
    {
        base.Awake();
        
        advanceSpeed = 10;
        retreatSpeed = 12;
        erosionSpeed = 7;

        bindTime = 5f;

        objectBindChance = 0.4f;
        toyBindChance = 0.7f;
    }
    
    protected override IEnumerator StateMachine()
    {
        // 손길이 활성화되면 전진 상태로 시작
        Activate();
        CurrentHandState = HandState.Advancing;
        
        // 전진 페이즈
        float advanceTime = Random.Range(2f, 4f);
        yield return StartCoroutine(AdvancePhase(advanceTime));
        
        // 측면형: 잡기 시도 후 침식 단계
        if (TryGrabObject())
        {
            // 잡기 성공
            CurrentHandState = HandState.Grabbing;
            // yield return StartCoroutine(GrabPhase(bindTime));
        }
        else
        {
            // 잡기 실패 시 침식 단계
            CurrentHandState = HandState.Eroding;
            // yield return StartCoroutine(ErodePhase(erosionTime));
        }
        
        // 후퇴 페이즈
        CurrentHandState = HandState.Retreating;
        float retreatTime = Random.Range(1.5f, 3f);
        yield return StartCoroutine(RetreatPhase(retreatTime));
        
        // 이벤트 종료
        EndEvent();
    }
} 