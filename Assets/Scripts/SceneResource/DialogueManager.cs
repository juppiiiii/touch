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
public class DialogueItem
{
    public string key;
    public DialogueData data;
}

[System.Serializable]
public class DialogueDatabase
{
    public List<DialogueItem> items;
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

        if (database.items == null)
        {
            Debug.LogError("JSON 파싱 실패! items가 null입니다.");
            return;
        }

        // List를 Dictionary로 변환
        dialogueDictionary = new Dictionary<string, DialogueData>();
        foreach (DialogueItem item in database.items)
        {
            if (!dialogueDictionary.ContainsKey(item.key))
            {
                dialogueDictionary.Add(item.key, item.data);
            }
        }

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
        if (dialogueDictionary == null)
        {
            Debug.LogError("DialogueManager: JSON 데이터가 아직 로드되지 않았습니다!");
            return null;
        }

        if (!dialogueDictionary.ContainsKey(objectName))
        {
            Debug.LogError($"'{objectName}'에 대한 대화 데이터가 JSON에 없습니다.");
            return null;
        }

        return dialogueDictionary[objectName];
    }
}
