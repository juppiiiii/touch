using UnityEngine;
using System.Collections;
using NUnit.Framework.Interfaces;
using UnityEngine.UI;

public class G3Manager : MonoBehaviour
{
    public GameObject G3;
    public Player player; // Player 스크립트 연결
    public Bar[] bars;       // Bar 스크립트 연결
    public float timeLimit = 30f; // 제한 시간 (초)
    public Image timerGaze;
    
    public Image failCircle;   // 실패 시 표시할 Image UI
    public Image successCircle;// 성공 시 표시할 Image UI

    private bool isGame = true;
    private bool isGameOver = false; // 게임 종료 여부 플래그 추가
    private float timer;

    public bool win;

    private void Start()
    {
        if (failCircle != null)
            failCircle.gameObject.SetActive(false);

        if (successCircle != null)
            successCircle.gameObject.SetActive(false);
        timer = timeLimit; // 제한 시간 초기화
        StartCoroutine(TimerCountdown()); // 제한 시간 카운트 시작
    }

    private void Update()
    {
        if (isGameOver) return; // 게임이 종료되었으면 추가 호출 방지

        else if (player.IsWin() == isGame)
        {
            isGame = false;
            isGameOver = true; // 중복 호출 방지
            StartCoroutine(Success());
        }

        else if (player.IsFall() == isGame)
        {
            isGameOver = true; // 중복 호출 방지
            isGame = false;
            StartCoroutine(Fail());
        }
    }

    IEnumerator TimerCountdown()
    {
        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);
            timer -= 1f;
            timerGaze.fillAmount = timer / timeLimit;

            // 시간이 0 이하가 되면 게임 오버
            if (timer <= 0 && !isGameOver)
            {
                isGameOver = true; // 중복 호출 방지
                StartCoroutine(Fail());
                yield break;
            }
        }
    }

    IEnumerator Success()
    {
        win = true;
        Debug.Log("Success");
        player.Stop();
        StopBars();
        successCircle.gameObject.SetActive(true);

        yield return new WaitForSeconds(3);
        successCircle.gameObject.SetActive(false);
        G3.SetActive(false);
    }

    IEnumerator Fail()
    {
        win = false;
        Debug.Log("Fail");
        player.Stop();
        StopBars();
        failCircle.gameObject.SetActive(true);

        yield return new WaitForSeconds(3);
        failCircle.gameObject.SetActive(false);
        G3.SetActive(false);
    }

    private void StopBars()
    {
        foreach (Bar bar in bars)
        {
            bar.Stop();
        }
    }
}
