using System;
using UnityEngine;

public abstract class NightEvent : MonoBehaviour
{
    // 이벤트가 종료됐을 때 호출되는 이벤트
    public event Action OnEventEnded;
    
    // 이벤트의 상태 enum
    public enum EventState
    {
        Inactive,    // 비활성화
        Active,     // 활성화
        Ended       // 종료
    }
    
    // 현재 이벤트 상태
    public EventState CurrentState { get; protected set; } = EventState.Inactive;
    
    // 이벤트 종료 처리
    protected virtual void EndEvent()
    {
        CurrentState = EventState.Ended;
        OnEventEnded?.Invoke();
    }
    
    // 이벤트 활성화
    public virtual void Activate()
    {
        CurrentState = EventState.Active;
    }
}