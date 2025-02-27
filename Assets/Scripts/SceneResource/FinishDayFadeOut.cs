using UnityEngine;
using System.Collections;

public class FinishDayFadeOut : MonoBehaviour
{
    public CanvasGroup canvasGroup; // 현재 Canvas의 CanvasGroup
    public float fadeDuration = 1f; // 서서히 사라지는 시간
    public CanvasGroup nextCanvas; // 다음 Canvas (필요할 때만 등장)

    public void StartFadeOut()
    {
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

        // 🔹 현재 Canvas 완전히 사라지면 비활성화
        canvasGroup.gameObject.SetActive(false);
        
        GameManager.Instance.FinishDayPreparation();
        
        // 🔹 다음 Canvas가 있다면, 활성화 후 페이드인 효과 적용
        if (nextCanvas != null)
        {
            nextCanvas.gameObject.SetActive(true); // 🔥 비활성화 상태였던 다음 Canvas를 활성화
            nextCanvas.alpha = 1; // 처음에는 투명
            // StartCoroutine(FadeInNextCanvas());
        }
    }

    IEnumerator FadeInNextCanvas()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            nextCanvas.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }
    }
}
