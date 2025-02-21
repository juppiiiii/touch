using UnityEngine;
using UnityEngine.UI;

public class UIDebuging : MonoBehaviour
{
    [SerializeField] private GameObject g1;
    [SerializeField] private GameObject g2;
    [SerializeField] private GameObject g3;
    [SerializeField] private GameObject g4;

    [SerializeField] private Button bG1;
    [SerializeField] private Button bG2;
    [SerializeField] private Button bG3;
    [SerializeField] private Button bG4;

    private void Start()
    {
        // 버튼 클릭 이벤트 추가
        bG1.onClick.AddListener(() => ToggleGameObject(g1));
        bG2.onClick.AddListener(() => ToggleGameObject(g2));
        bG3.onClick.AddListener(() => ToggleGameObject(g3));
        bG4.onClick.AddListener(() => ToggleGameObject(g4));
    }

    private void ToggleGameObject(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }
}
