using UnityEngine;

public class EvilSpiritNormal : EvilSpirit
{
    public override string SpiritTypeName => "부정적인 기운";
    
    protected override void Awake()
    {
        base.Awake();
        
        // 기본 속성값 직접 설정
        advanceSpeed = 10;
        retreatSpeed = 15;
        erosionAmount = 0.3f;
    }
} 