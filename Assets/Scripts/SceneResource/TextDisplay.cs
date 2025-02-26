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
    public CanvasFadeOut canvasFadeOut; // Canvas 서서히 사라지는 스크립트
    public GameStartFadeOut gameStartFadeOut;

    private List<string> sentences;  // 문장 리스트
    private int currentIndex = 0;  // 현재 출력 중인 문장 인덱스
    private bool isTyping = false;  // 타이핑 중인지 체크

    // public GameManager gameManager;

    void Start()
    {
        // GameManager.Instance.PauseTimer();

        // 텍스트 파일을 한 줄씩 저장
        if (textFile != null)
        {
            sentences = new List<string>(textFile.text.Split('\n'));
        }
        else
        {
            Debug.LogError("텍스트 파일이 연결되지 않았습니다!");
        }

        // 버튼 클릭 이벤트 추가
        nextButton.onClick.AddListener(OnClickNext);

        // 첫 번째 문장 출력
        DisplayNextSentence();
    }

    void OnClickNext()
    {
        if (!isTyping) // 타이핑이 끝났다면
        {
            DisplayNextSentence(); // 다음 문장 출력
        }
        else // 아직 타이핑 중이라면 즉시 출력 완료
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
            // 🔹 마지막 문장이 끝났다면 Canvas 서서히 사라지기 실행!
            Debug.Log("마지막 문장 도달! Canvas 서서히 사라짐.");

            if (canvasFadeOut != null)
            {
                // 🔥 CanvasFadeOut 오브젝트 활성화
                canvasFadeOut.gameObject.SetActive(true);
                canvasFadeOut.StartFadeOut(); // 이제 Coroutine 실행 가능!
            }

            else if (gameStartFadeOut != null)
            {
                // 🔥 CanvasFadeOut 오브젝트 활성화
                gameStartFadeOut.gameObject.SetActive(true);
                gameStartFadeOut.StartFadeOut(); // 이제 Coroutine 실행 가능!
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
