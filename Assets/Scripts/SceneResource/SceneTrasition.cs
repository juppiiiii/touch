using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup; // 현재 Canvas의 CanvasGroup
    public float fadeDuration = 1.5f; // 서서히 사라지는 시간
    public string nextSceneName; // 이동할 씬 이름

    public void StartSceneTransition()
    {
        StartCoroutine(FadeOutAndLoadScene());
    }

    IEnumerator FadeOutAndLoadScene()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }

        // 🔹 새로운 Scene 로드
        SceneManager.LoadScene(nextSceneName);
    }
}
