using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class NightTextManager : MonoBehaviour
{
    public GameObject textCanvas; // 텍스트 출력용 캔버스
    public Text displayText; // 텍스트 UI
    public Button nextButton; // 다음 문장 버튼
    public string jsonFileName = "NightText.json"; // JSON 파일 이름

    public CanvasFadeOut canvasFadeOut; // Canvas 서서히 사라지는 스크립트

    private List<string> nightMessages;
    private int currentMessageIndex = 0;
    private bool isDisplaying = false;
    private bool hasNightTriggered = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        LoadTextFromJson();
        textCanvas.SetActive(true); // Canvas는 활성화하되 내부 UI는 숨김
        displayText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(DisplayNextMessage);
        StartCoroutine(CheckNightRoutine());
    }

    void LoadTextFromJson()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            TextData data = JsonUtility.FromJson<TextData>(jsonContent);
            nightMessages = new List<string>(data.messages);
        }
        else
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다: " + filePath);
        }
    }

    IEnumerator CheckNightRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (GameManager.Instance.IsNight && !hasNightTriggered)
            {
                hasNightTriggered = true; // 밤 이벤트 한 번만 실행
                isDisplaying = true;
                // GameManager.Instance.PauseTimer(); // 타이머 정지
                StartDialogue();
            }
            else if (!GameManager.Instance.IsNight)
            {
                hasNightTriggered = false; // 낮이 되면 다시 초기화
            }
        }
    }

    void StartDialogue()
    {
        currentMessageIndex = 0;
        displayText.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        DisplayNextMessage();
    }

    public void DisplayNextMessage()
    {
        if (currentMessageIndex < nightMessages.Count)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeTextEffect(nightMessages[currentMessageIndex]));
            currentMessageIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeTextEffect(string message)
    {
        displayText.text = "";
        foreach (char letter in message.ToCharArray())
        {
            displayText.text += letter;
            yield return new WaitForSeconds(0.05f); // 한 글자씩 출력 속도 조절
        }
    }

    void EndDialogue()
    {
        displayText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        // GameManager.Instance.ResumeTimer(); // 타이머 재개
        isDisplaying = false;
        canvasFadeOut.StartFadeOut();
    }
}

[System.Serializable]
public class TextData
{
    public string[] messages;
}
