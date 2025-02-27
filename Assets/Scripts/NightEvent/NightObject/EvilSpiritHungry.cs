using UnityEngine;

public class EvilSpiritHungry : EvilSpirit
{
    public override string SpiritTypeName => "굶주린 부정적인 기운";
    
    protected override void Awake()
    {
        base.Awake();
        
        // 기본 속성값 직접 설정
        forwardSpeed = 15;
        retreatSpeed = 17;
        erosionAmount = 0.4f;
    }
} 