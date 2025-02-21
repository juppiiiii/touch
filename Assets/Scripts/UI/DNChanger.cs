using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DNChanger : MonoBehaviour
{
    public GameManager gameManager;
    public Text timerTxt;
    public GameObject day;
    public GameObject night;
    public bool currentNight = false;
    

    // Update is called once per frame
    void Update()
    {
        timerTxt.text = Math.Truncate(gameManager.GetTimerElapsed() / 1).ToString();
        if (gameManager.IsNight != currentNight) {
            if (gameManager.IsNight) {
                currentNight = true;
                NowNight();
            }
            else {
                currentNight = false;
                NowDay();
            }
        }
    }
    private void NowDay() {
        night.SetActive(false);
        day.SetActive(true);
    }

    private void NowNight() {
        day.SetActive(false);
        night.SetActive(true);
    }


}