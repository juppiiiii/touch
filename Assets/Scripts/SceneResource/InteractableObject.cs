using UnityEngine;
using System.Collections.Generic;

public class InteractableObject : MonoBehaviour
{
    private DialogueManager dialogueManager;
    private Dictionary<string, bool> interactedObjects = new Dictionary<string, bool>(); // ✅ 클릭한 오브젝트 기록
    private Camera mainCamera;

    private void Start()
    {
        dialogueManager = DialogueManager.Instance;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 좌클릭 감지
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.transform.gameObject;
            Debug.Log($"클릭한 오브젝트: {clickedObject.name}");

            if (dialogueManager != null)
            {
                ShowDialogueForObject(clickedObject);
            }
        }
    }

    void ShowDialogueForObject(GameObject clickedObject)
    {
        string objectName = clickedObject.name;

        // ✅ 이미 한 번 본 오브젝트라면 대화창을 다시 띄우지 않음
        if (interactedObjects.ContainsKey(objectName) && interactedObjects[objectName])
        {
            Debug.Log($"{objectName}의 대화는 이미 본 적이 있습니다.");
            return;
        }

        DialogueData dialogueData = dialogueManager.GetDialogueData(objectName);

        if (dialogueData != null)
        {
            GameObject targetCanvas = GameObject.Find(dialogueData.canvasName);
            if (targetCanvas != null)
            {
                DialogueUI dialogueUI = targetCanvas.GetComponent<DialogueUI>();
                if (dialogueUI != null)
                {
                    interactedObjects[objectName] = true; // ✅ 오브젝트를 한 번 본 것으로 기록
                    dialogueUI.ShowDialogue(dialogueData.dialogues);
                }
            }
            else
            {
                Debug.LogError($"Canvas '{dialogueData.canvasName}'를 찾을 수 없습니다!");
            }
        }
    }
}
