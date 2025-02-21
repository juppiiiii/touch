using UnityEngine;
using UnityEngine.UI;

public class GazeManager : MonoBehaviour
{
    public Image gaze;
    public float currAmount = 180f;
    public void gazeChange(float amount) {
        currAmount -= amount;
        gaze.fillAmount = currAmount;
    }
}
