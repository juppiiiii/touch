using System.Collections;
using UnityEngine;

public class DiceZone : MonoBehaviour
{
    public DiceManager diceManager; // DiceManager 참조
    private bool isProcessing = false; // 현재 처리 중인지 확인

    private void OnCollisionStay(Collision collision)
    {
        Dice dice = collision.gameObject.GetComponent<Dice>(); // 충돌한 오브젝트가 주사위인지 확인
        if (dice == null) return;

        Rigidbody rb = dice.GetComponent<Rigidbody>();
        if (rb == null) return;

        // 주사위가 멈췄을 때만 결과 저장 (이미 처리 중이면 실행 안 함)
        if (!isProcessing && rb.linearVelocity.sqrMagnitude < 0.01f)
        {
            StartCoroutine(StoreDiceResult(dice));
        }
    }

    private IEnumerator StoreDiceResult(Dice dice)
    {
        if (isProcessing) yield break; // 이미 실행 중이면 종료
        isProcessing = true; // 실행 중 상태 설정

        yield return new WaitForSeconds(1f); // 1초 후 멈췄는지 다시 확인

        Rigidbody rb = dice.GetComponent<Rigidbody>();
        if (rb.linearVelocity.sqrMagnitude < 0.01f) // 다시 한 번 멈춘 상태 확인
        {
            // 주사위 값이 이미 저장된 경우 중복 저장 방지
            if (!diceManager.HasDiceResult(dice))
            {
                Transform highestFace = GetHighestFace(dice); // 가장 위쪽에 있는 면 찾기
                if (highestFace != null)
                {
                    string result = highestFace.gameObject.name;
                    diceManager.StoreDiceResult(dice, result); // DiceManager에 결과 전달
                }
            }
        }

        isProcessing = false; // 실행 완료 후 다시 실행 가능 상태로 변경
    }

    private Transform GetHighestFace(Dice dice)
    {
        Transform highestFace = null;
        float highestY = float.MinValue;

        foreach (Transform face in dice.transform) // 주사위의 모든 자식(면) 검사
        {
            if (face.position.y > highestY)
            {
                highestY = face.position.y;
                highestFace = face;
            }
        }

        return highestFace;
    }
}
