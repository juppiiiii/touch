using UnityEngine;
using System.Collections;

public class CanvasFadeOut : MonoBehaviour
{
    public CanvasGroup canvasGroup; // 현재 Canvas의 CanvasGroup
    public float fadeDuration = 1f; // 서서히 사라지는 시간

    public GameManager gameManager;

    public void StartFadeOut()
    {
        // gameManager.PauseTimer();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }

        // 캔버스 완전히 사라지면 비활성화
        canvasGroup.gameObject.SetActive(false);
        
        // gameManager.ResumeTimer();
    }
}
