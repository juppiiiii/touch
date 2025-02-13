using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 게임 상수
    private const float DAY_DURATION = 90f;
    private const float NIGHT_DURATION = 60f;
    private const float SCENARIO_DURATION = 30f;
    private const float NIGHT_DURATION_WAVE2 = 90f;

    // 게임 변수
    private int currentWave = 1;
    private bool isNight = false;
    public float sanityGauge = 100f;
    public float endingGauge = 0f;

    void Start()
    {
        StartWave();
    }

    public void StartWave()
    {
        if (currentWave == 1 || currentWave == 6)
        {
            StartScenario();
        }
        else
        {
            StartDay();
        }
    }

private void StartScenario()
{
    StartCoroutine(GameTimer(SCENARIO_DURATION));
}

    public void StartDay()
    {
        isNight = false;
        StartCoroutine(GameTimer(DAY_DURATION));
    }

    public void StartNight()
    {
        isNight = true;
        float duration = (currentWave == 2) ? NIGHT_DURATION_WAVE2 : NIGHT_DURATION;
        StartCoroutine(GameTimer(duration));
    }

    private IEnumerator GameTimer(float duration)
    {
        float remainingTime = duration;
        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(1);
            remainingTime--;
        }
        OnTimeOver();
    }

    private void OnTimeOver()
    {
        if (currentWave == 6 && isNight)
        {
            EndGame();
        }
        else if (isNight)
        {
            currentWave++;
            StartWave();
        }
        else
        {
            StartNight();
        }
    }

    private void EndGame()
    {
    }

    // 현재 밤/낮 상태와 현재 웨이브를 출력하는 메서드
    private void PrintCurrentState()
    {
        string timeOfDay = isNight ? "밤" : "낮";
        Debug.Log($"현재는 {timeOfDay}이며, {currentWave} 웨이브입니다.");
    }

    // Update 호출 시, I 키를 누르면 현재 상태를 출력
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PrintCurrentState();
        }
    }
}
