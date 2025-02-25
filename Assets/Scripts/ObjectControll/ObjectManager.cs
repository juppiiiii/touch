using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
	private bool isCorrect;
	public GameObject selected;
	private bool isDragging = false;
	private Vector3 lastMousePosition;
	private Vector3 latestPos;
	public string selectedTag;
	private Camera mainCamera;

	// 이동 가능 영역 한계
	public float maxZ = -1;
	public float minZ = -20.5f;
	public float minX = 1;
	public float maxX = 20.5f;

	// 쓰레기통과 세탁기의 영역
	public float washMinX = 0;
	public float washMaxX = 10;
	public float washMinZ = -20;
	public float washMaxZ = -10;
	public float trashMinX = 10;
	public float trashMaxX = 20;
	public float trashMinZ = -10;
	public float trashMaxZ = 0;

	// WellDestroyed가 1회만 호출되도록 제어하는 플래그 변수
	private bool wellDestroyedCalled = false;

	private void Start()
	{
		mainCamera = Camera.main;
	}

	private void Update()
	{
		// 마우스 입력에 따른 처리
		if (Input.GetMouseButtonDown(0))
		{
			FindLeftClick();
		}

		if (Input.GetMouseButtonUp(0))
		{
			LostLeftClick();
		}

		// 선택된 오브젝트가 있을 경우 이동 등을 처리
		if (selected != null)
		{
			if (isDragging)
			{
				DragMove();
			}
			else
			{
				WASDMove();
			}

			Clamping();
		}

		// 오브젝트 파괴 감지: 파괴가 감지되고 아직 WellDestroyed()가 호출되지 않았다면 1회만 실행
		// Unity는 파괴된 오브젝트에 대해 'selected == null'이 true를 반환합니다.
		if (selected == null && !wellDestroyedCalled)
		{
			wellDestroyedCalled = true;
			isCorrect = WellDestroyed();
			Debug.Log("WellDestroyed 결과: " + isCorrect);
		}
	}

	void FindLeftClick() // 마우스 좌클릭 감지
	{
		RaycastHit hit;
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit))
		{
			if (hit.transform != null)
			{
				selected = hit.transform.gameObject; // 클릭한 오브젝트 저장
				Debug.Log("Selected Object: " + selected.name);

				// 드래그 시작을 위한 설정
				isDragging = true;
				selectedTag = selected.tag;

				// 마우스 이전 위치 초기화
				lastMousePosition = Input.mousePosition;

				// 새 오브젝트를 선택하면 호출 플래그를 리셋 (새로운 오브젝트 파괴시 다시 호출됨)
				wellDestroyedCalled = false;
			}
		}
	}

	void LostLeftClick() // 마우스 좌클릭 해제 감지
	{
		if (isDragging)
		{
			isDragging = false;
			selectedTag = null;
			Debug.Log("드래그 종료");
		}
	}

	void DragMove() // 드래그 이동
	{
		Plane dragPlane = new Plane(Vector3.up, selected.transform.position);
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

		if (dragPlane.Raycast(ray, out float enter))
		{
			Vector3 hitPoint = ray.GetPoint(enter);
			Vector3 worldDelta = hitPoint - selected.transform.position;
			worldDelta.y = 0f;
			selected.transform.position += worldDelta;
		}

		lastMousePosition = selected.transform.position;
	}

	void WASDMove() // WASD 이동 처리
	{
		if (Input.GetKeyDown("w"))
		{
			Vector3 locate = selected.transform.position;
			selected.transform.position = new Vector3(locate.x, locate.y, locate.z + 1);
		}
		if (Input.GetKeyDown("a"))
		{
			Vector3 locate = selected.transform.position;
			selected.transform.position = new Vector3(locate.x - 1, locate.y, locate.z);
		}
		if (Input.GetKeyDown("s"))
		{
			Vector3 locate = selected.transform.position;
			selected.transform.position = new Vector3(locate.x, locate.y, locate.z - 1);
		}
		if (Input.GetKeyDown("d"))
		{
			Vector3 locate = selected.transform.position;
			selected.transform.position = new Vector3(locate.x + 1, locate.y, locate.z);
		}
	}

	void Clamping() // 오브젝트 위치 제한
	{
		Vector3 pos = selected.transform.position;
		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
		selected.transform.position = latestPos = pos;
	}

	bool WellDestroyed() // 올바른 위치로 갔는지 검사하는 함수
	{
		Debug.Log($"{lastMousePosition.x} {lastMousePosition.y} {lastMousePosition.z}");
		// 선택된 태그에 따라 검사
		if (selectedTag == "NC" || selectedTag == "SC")
		{
			if (lastMousePosition.x <= washMaxX && lastMousePosition.x >= washMinX &&
				lastMousePosition.z <= washMaxZ && lastMousePosition.z >= washMinZ)
				return true;
			else
				return false;
		}
		else
		{
			if (lastMousePosition.x <= trashMaxX && lastMousePosition.x >= trashMinX &&
				lastMousePosition.z <= trashMaxZ && lastMousePosition.z >= trashMinZ)
				return true;
			else
				return false;
		}
	}
}
