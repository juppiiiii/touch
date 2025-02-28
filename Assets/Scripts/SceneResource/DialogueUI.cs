using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    public Text dialogueText; // UI 텍스트 (Legacy Text 사용)
    public Button nextButton; // 다음 문장 버튼
    public CanvasGroup canvasGroup; // CanvasGroup을 활용한 페이드 효과
    public float typingSpeed = 0.05f; // 글자 타이핑 속도
    private Queue<string> sentences; // 대사 큐

    private void Start()
    {
        canvasGroup.alpha = 0; // 처음에는 안 보이게 설정
        gameObject.SetActive(false);
        sentences = new Queue<string>();

        nextButton.onClick.AddListener(DisplayNextSentence);
    }

    public void ShowDialogue(List<string> dialogues)
    {
        sentences.Clear();
        foreach (string sentence in dialogues)
        {
            sentences.Enqueue(sentence);
        }

        gameObject.SetActive(true);
        StartCoroutine(FadeInCanvas());
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            StartCoroutine(FadeOutCanvas());
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator FadeInCanvas()
    {
        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / 0.5f);
            yield return null;
        }
    }

    IEnumerator FadeOutCanvas()
    {
        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / 0.5f);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
