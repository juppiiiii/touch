using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TextTypingEffect : MonoBehaviour
{
    public TextAsset textFile;  // 불러올 텍스트 파일
    public Text displayText;  // 출력할 UI 텍스트
    public Button nextButton;  // 다음 문장 버튼
    public float typingSpeed = 0.05f; // 글자 타이핑 속도
    public CanvasFadeOut canvasFadeOut; // 기존 Canvas 사라지는 기능
    private List<string> sentences;  // 문장 리스트
    private int currentIndex = 0;  // 현재 출력 중인 문장 인덱스
    private bool isTyping = false;  // 타이핑 중인지 체크

    public FinishDayFadeOut finishDayFadeOut;
    public FinishNightFadeOut finishNightFadeOut;

    private DayStartDialogue dayStartDialogue; // 새로 추가: 낮 시작 이벤트용

    void Start()
    {
        if (textFile != null)
        {
            sentences = new List<string>(textFile.text.Split('\n'));
        }
        else
        {
            Debug.LogError("텍스트 파일이 연결되지 않았습니다!");
        }

        nextButton.onClick.AddListener(OnClickNext);
        DisplayNextSentence();
    }

    // 🔹 낮이 시작될 때 `DayStartDialogue`에서 호출하는 함수 (기존 방식과 분리)
    public void StartDialogue(DayStartDialogue dialogueController)
    {
        dayStartDialogue = dialogueController; // 낮 이벤트 매니저 저장
        currentIndex = 0; // 대화 처음부터 시작
        DisplayNextSentence();
    }

    void OnClickNext()
    {
        if (!isTyping)
        {
            DisplayNextSentence();
        }
        else
        {
            StopAllCoroutines();
            displayText.text = sentences[currentIndex - 1]; // 전체 문장 즉시 표시
            isTyping = false;
        }
    }

    void DisplayNextSentence()
    {
        if (sentences != null && currentIndex < sentences.Count)
        {
            displayText.text = "";
            StartCoroutine(TypeSentence(sentences[currentIndex]));
            currentIndex++;
        }
        else
        {
            Debug.Log("대화 종료!");

            // 🔹 낮이 시작된 경우: `DayStartDialogue` 호출
            if (dayStartDialogue != null)
            {
                dayStartDialogue.OnDialogueEnd();
            }
            // 🔹 기존 방식: `CanvasFadeOut` 호출
            else if (canvasFadeOut != null)
            {
                canvasFadeOut.StartFadeOut();
            }
            else if (finishDayFadeOut != null)
            {
                // 🔥 CanvasFadeOut 오브젝트 활성화
                finishDayFadeOut.gameObject.SetActive(true);
                finishDayFadeOut.StartFadeOut(); // 이제 Coroutine 실행 가능!
            }
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        displayText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            displayText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}
