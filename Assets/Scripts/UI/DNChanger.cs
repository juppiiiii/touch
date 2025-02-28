using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DNChanger : MonoBehaviour
{
    public Text timerTxt;
    public GameObject day;
    public GameObject night;
    public bool currentNight = false;
    

    // Update is called once per frame
    void Update()
    {
        timerTxt.text = Math.Truncate(GameManager.Instance.GetTimerElapsed() / 1).ToString();
        if (GameManager.Instance.IsNight != currentNight) {
            if (GameManager.Instance.IsNight) {
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