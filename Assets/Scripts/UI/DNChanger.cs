using System;
using UnityEngine;
using UnityEngine.UI;

public class DNChanger : MonoBehaviour
{
    public GameManager gameManager;
    public Text timerTxt;


    // Update is called once per frame
    void Update()
    {
        timerTxt.text = Math.Truncate(gameManager.GetTimerElapsed() / 1).ToString();

    }


}