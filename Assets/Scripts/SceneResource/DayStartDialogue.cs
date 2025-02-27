using UnityEngine;
using System.Collections;

public class DayStartDialogue : MonoBehaviour
{
    public CanvasGroup dialogueCanvas; // 대화 CanvasGroup (투명도 조절)
    public TextTypingEffect textTypingEffect; // 대화 출력 관리
    public CanvasFadeOut canvasFadeOut; // Canvas 사라지는 효과

    private bool isDialogueShown = false; // ✅ 대화창이 한 번만 나타나도록 하는 플래그

    private void Start()
    {
        // GameManager 찾기
        GameManager gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null)
        {
            // 🔥 GameManager 수정 없이 이벤트 감지
            gameManager.OnNightEnded += ActivateDialogueCanvas;
        }
        else
        {
            Debug.LogError("GameManager를 찾을 수 없습니다!");
        }
    }

    private void OnDestroy()
    {
        // 이벤트 해제 (메모리 누수 방지)
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnNightEnded -= ActivateDialogueCanvas;
        }
    }

    // 🔹 밤이 끝나고 낮이 시작되면 대화 창 활성화 (한 번만 실행)
    private void ActivateDialogueCanvas()
    {   
        Debug.Log("낯이 밝았습니다!");
        if (!isDialogueShown) // 🚀 낮이 지속되는 동안 다시 나타나지 않도록 설정
        {
            isDialogueShown = true; // ✅ 한 번만 실행되도록 설정

            dialogueCanvas.gameObject.SetActive(true); // Canvas 활성화
            dialogueCanvas.alpha = 1; // 완전히 보이도록 설정
            textTypingEffect.StartDialogue(this); // 대화 시작
        }
    }

    // 🔹 대화가 끝나면 실행되는 메서드
    public void OnDialogueEnd()
    {
        StartCoroutine(FadeOutCanvas()); // Canvas 서서히 사라지기
    }

    IEnumerator FadeOutCanvas()
    {
        float fadeDuration = 1f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            dialogueCanvas.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }

        dialogueCanvas.gameObject.SetActive(false); // 완전히 숨기기
    }
}
