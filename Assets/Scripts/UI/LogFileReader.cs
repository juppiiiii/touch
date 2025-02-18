using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogFileReader : MonoBehaviour
{
    public Text logTxt;           // 로그를 출력할 UI 텍스트
    public Button nextLineButton; // 다음 줄을 출력할 버튼

    private List<string> logLines = new List<string>(); // 텍스트 파일에서 불러온 줄들을 저장
    private int currentLineIndex = 0; // 현재 출력할 줄의 인덱스

    void Start()
    {
        LoadTextFile("logfile"); // "logfile.txt"를 로드 (Resources 폴더 내)
        nextLineButton.onClick.AddListener(ShowNextLine); // 버튼 클릭 이벤트 등록
    }

    // 🔹 Resources 폴더에서 텍스트 파일 불러와서 리스트에 저장
    void LoadTextFile(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);

        if (textAsset != null)
        {
            logLines.AddRange(textAsset.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None));
            ShowNextLine(); // 첫 번째 줄 먼저 출력
        }
        else
        {
            logTxt.text = "❌ 로그 파일을 찾을 수 없습니다!";
        }
    }

    // 🔹 버튼을 클릭하면 다음 줄을 출력
    void ShowNextLine()
    {
        if (logLines.Count > 0 && currentLineIndex < logLines.Count)
        {
            logTxt.text = logLines[currentLineIndex]; // 현재 줄 출력
            currentLineIndex++; // 다음 줄로 이동
        }
        else
        {
            logTxt.text = "✅ 로그 끝!";
        }
    }
}
