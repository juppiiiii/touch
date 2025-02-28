using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class DialogueData
{
    public string canvasName;  // 오브젝트의 Canvas 이름
    public List<string> dialogues;  // 대사 목록
}

[System.Serializable]
public class DialogueDatabase
{
    public Dictionary<string, DialogueData> dialogues;
}

public class DialogueManager : MonoBehaviour
{
    private Dictionary<string, DialogueData> dialogueDictionary;
    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log("DialogueManager가 정상적으로 생성됨.");
        LoadDialogueData();
    }

    void LoadDialogueData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "dialogues.json");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"대화 JSON 파일이 존재하지 않습니다! 경로 확인: {filePath}");
            return;
        }

        string jsonData = File.ReadAllText(filePath);
        Debug.Log("JSON 파일 로드 성공! 원본 데이터: " + jsonData);

        DialogueDatabase database = JsonUtility.FromJson<DialogueDatabase>(jsonData);

        if (database == null)
        {
            Debug.LogError("JSON 파싱 실패! 데이터베이스가 null입니다.");
            return;
        }

        if (database.dialogues == null)
        {
            Debug.LogError("JSON 파싱 실패! dialogues가 null입니다.");
            return;
        }

        dialogueDictionary = database.dialogues;

        if (dialogueDictionary == null || dialogueDictionary.Count == 0)
        {
            Debug.LogError("JSON 데이터가 비어 있습니다!");
        }
        else
        {
            Debug.Log("JSON 데이터 로드 성공! 총 " + dialogueDictionary.Count + "개의 대화가 등록됨.");
        }
    }

    public DialogueData GetDialogueData(string objectName)
    {
        return dialogueDictionary.ContainsKey(objectName) ? dialogueDictionary[objectName] : null;
    }
}
