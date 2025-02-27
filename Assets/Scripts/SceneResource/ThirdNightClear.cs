using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ThirdNightClear : MonoBehaviour
{
    public GameObject textCanvas; // 텍스트 출력용 캔버스
    public Text displayText; // 텍스트 UI
    public Button nextButton; // 다음 문장 버튼
    public Image backGround; // 뒷 배경
    public string jsonFileName = "ThirdNightClear.json"; // 낮 대사 JSON 파일

    public CanvasFadeOut canvasFadeOut; // Canvas 서서히 사라지는 스크립트

    private List<string> dayMessages;
    private int currentMessageIndex = 0;
    private bool isDisplaying = false;
    private bool hasDayTriggered = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        LoadTextFromJson();
        textCanvas.SetActive(true); // 처음에는 비활성화
        displayText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        // backGround.gameObject.SetActive(false);
        nextButton.onClick.AddListener(DisplayNextMessage);
        StartCoroutine(CheckDayRoutine());
    }

    void LoadTextFromJson()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            TextData data = JsonUtility.FromJson<TextData>(jsonContent);
            dayMessages = new List<string>(data.messages);
        }
        else
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다: " + filePath);
        }
    }

    IEnumerator CheckDayRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            // 낮이 되었고, 첫 번째 낮(웨이브 1)이 아니라면 Canvas를 띄운다
            if (!GameManager.Instance.IsNight && !hasDayTriggered && GameManager.Instance.CurrentWave > 3)
            {
                hasDayTriggered = true; // 낮 이벤트 한 번만 실행
                isDisplaying = true;
                StartDialogue();
            }
            else if (GameManager.Instance.IsNight)
            {
                hasDayTriggered = false; // 밤이 되면 다시 초기화
            }
        }
    }

    void StartDialogue()
    {
        textCanvas.SetActive(true);
        currentMessageIndex = 0;
        displayText.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        // backGround.gameObject.SetActive(true);
        DisplayNextMessage();
    }

    public void DisplayNextMessage()
    {
        if (currentMessageIndex < dayMessages.Count)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeTextEffect(dayMessages[currentMessageIndex]));
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
            yield return new WaitForSeconds(0.05f);
        }
    }

    void EndDialogue()
    {
        // textCanvas.SetActive(false);
        displayText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        // backGround.gameObject.SetActive(false);
        isDisplaying = false;
        canvasFadeOut.StartFadeOut();
    }
}

[System.Serializable]
public class TextDatThirdNightClear
{
    public string[] message;
}
