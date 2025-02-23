using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DiceManager : MonoBehaviour
{
    public Dice[] dices; // 여러 개의 주사위 배열
    private bool isRolling = false; // 주사위가 굴러가고 있는지 여부
    private Dictionary<Dice, string> diceResults = new Dictionary<Dice, string>(); // 주사위별 결과 저장

    private void Update()
    {
        // 스페이스 바를 누르면 모든 주사위를 굴리기
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling)
        {
            RollDice();
        }

        // 모든 주사위가 멈췄는지 확인
        if (isRolling && AllDicesStopped())
        {
            isRolling = false;
        }
    }

    public void RollDice()
    {
        if (!isRolling) // 모든 주사위가 멈춰 있을 때만 다시 굴리기 가능
        {
            diceResults.Clear(); // 이전 결과 초기화
            foreach (Dice dice in dices)
            {
                Rigidbody rb = dice.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ; // 회전 고정을 풀고 다시 굴리기 가능하게 설정
                }
                dice.DiceRoll();
            }
            isRolling = true;
        }
    }

    public bool AllDicesStopped()
    {
        foreach (Dice dice in dices)
        {
            if (dice.GetComponent<Rigidbody>().linearVelocity.sqrMagnitude > 0.01f) // 아직 움직이는 주사위가 있으면 false 반환
            {
                return false;
            }
        }
        return true; // 모든 주사위가 멈추면 true 반환
    }




    public void StoreDiceResult(Dice dice, string result)
    {
        if (!diceResults.ContainsKey(dice))
        {
            diceResults[dice] = result;
            Debug.Log($"주사위 {System.Array.IndexOf(dices, dice) + 1} 결과 저장: {result}");
            FreezeDiceRotation(dice);
        
        }
    }
    private void FreezeDiceRotation(Dice dice)
    {
        Rigidbody rb = dice.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.angularVelocity = Vector3.zero; // 회전 속도 정지
            rb.constraints = RigidbodyConstraints.FreezeRotation; // 회전 고정
        }
    }
    public int GetTotalDiceValue()
    {
        int total = 0;
        foreach (var dice in dices)
        {
            if (diceResults.ContainsKey(dice) && int.TryParse(diceResults[dice], out int value))
            {
                total += value;
            }
        }
        return total;
    }
    public bool HasDiceResult(Dice dice)
    {
        return diceResults.ContainsKey(dice); // 주사위 값이 Dictionary에 저장되었는지 확인
    }

}
