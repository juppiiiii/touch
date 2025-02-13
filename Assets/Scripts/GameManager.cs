using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public ObjectManager objectManager;

    // 게임 상수
    private const float DAY_DURATION = 90f;
    private const float NIGHT_DURATION = 60f;
    private const float SCENARIO_DURATION = 30f;

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
        uiManager.UpdateGameState("Scenario");
        objectManager.InitializeObjects("Scenario");
        StartCoroutine(GameTimer(SCENARIO_DURATION));
    }

    public void StartDay()
    {
        isNight = false;
        uiManager.UpdateGameState("Day");
        objectManager.InitializeObjects("Day");
        StartCoroutine(GameTimer(DAY_DURATION));
    }

    public void StartNight()
    {
        isNight = true;
        uiManager.UpdateGameState("Night");
        objectManager.InitializeObjects("Night");
        float duration = (currentWave == 2) ? 90f : NIGHT_DURATION;
        StartCoroutine(GameTimer(duration));
    }

    private IEnumerator GameTimer(float duration)
    {
        float remainingTime = duration;
        while (remainingTime > 0)
        {
            uiManager.UpdateTimer(remainingTime);
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
        uiManager.ShowGameResult();
    }

    void Update()
    {
        HandleMovement();
    }
}
