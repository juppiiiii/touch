using UnityEngine;

public class DragObjectWithGrid : MonoBehaviour
{
    private Camera cam;
    private bool isDragging = false;
    private Vector3 offset;
    private float zCoord;
    public float minDistance = 0.5f; // 최소 이동 거리
    private Vector3 lastPosition;
    
    public int gridSize = 10; // 그리드 칸 개수
    public float gridSpacing = 1f; // 그리드 간격
    private GameObject gridX, gridY; // X축, Y축 그리드 오브젝트

    void Start()
    {
        cam = Camera.main;
        CreateGrid(); // 그리드 생성
    }

    void OnMouseDown()
    {
        zCoord = cam.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
        lastPosition = transform.position;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 targetPosition = GetMouseWorldPos() + offset;
            float moveDistance = Vector3.Distance(lastPosition, targetPosition);

            if (moveDistance >= minDistance)
            {
                Vector3 direction = (targetPosition - lastPosition).normalized;
                transform.position = lastPosition + direction * minDistance;
                lastPosition = transform.position;

                UpdateGrid(); // 그리드 위치 업데이트
            }
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return cam.ScreenToWorldPoint(mousePoint);
    }

    private void CreateGrid()
    {
        gridX = new GameObject("GridX");
        gridY = new GameObject("GridY");

        LineRenderer lineX = gridX.AddComponent<LineRenderer>();
        LineRenderer lineY = gridY.AddComponent<LineRenderer>();

        SetupGrid(lineX, Color.gray);
        SetupGrid(lineY, Color.gray);

        UpdateGrid();
    }

    private void SetupGrid(LineRenderer line, Color color)
    {
        line.positionCount = (gridSize * 2 + 1) * 2; // 그리드 개수 설정
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = color;
        line.endColor = color;
    }

    private void UpdateGrid()
    {
        float zOffset = -0.1f; // 오브젝트보다 뒤에 위치
        LineRenderer lineX = gridX.GetComponent<LineRenderer>();
        LineRenderer lineY = gridY.GetComponent<LineRenderer>();

        int index = 0;
        for (int i = -gridSize; i <= gridSize; i++)
        {
            float pos = i * gridSpacing;
            
            // X축 수직선
            lineX.SetPosition(index, new Vector3(pos, -gridSize * gridSpacing, transform.position.z + zOffset));
            lineX.SetPosition(index + 1, new Vector3(pos, gridSize * gridSpacing, transform.position.z + zOffset));
            
            // Y축 수평선
            lineY.SetPosition(index, new Vector3(-gridSize * gridSpacing, pos, transform.position.z + zOffset));
            lineY.SetPosition(index + 1, new Vector3(gridSize * gridSpacing, pos, transform.position.z + zOffset));

            index += 2;
        }

        // 그리드가 오브젝트 중심을 따라가도록 설정
        gridX.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + zOffset);
        gridY.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + zOffset);
    }
}
