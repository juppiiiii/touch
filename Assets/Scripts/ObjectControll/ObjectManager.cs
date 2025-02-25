using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

	// === 우클릭 길게 누르기를 위한 변수들 ===
	public bool ableInterection = false;  // 3초 동안 우클릭 유지 시 true로 설정됨
	public string interactionItem = "";    // 상호작용 실행 항목을 알리기 위한 문자열
	private float rightClickTimer = 0f;
	public float rightClickHoldTime = 3f;
	private bool isRightClickHeld = false;

	// Canvas/UI를 위한 Radial Gauge Image (Inspector 할당)
	public Image radialGaugeImage;

	private void Start()
	{
		mainCamera = Camera.main;
		if (radialGaugeImage != null)
			radialGaugeImage.fillAmount = 0f;  // 초기 채우기 상태 0
	}

	private void Update()
	{
		// --- 좌클릭 처리 ---
		if (Input.GetMouseButtonDown(0))
		{
			FindLeftClick();
		}
		if (Input.GetMouseButtonUp(0))
		{
			LostLeftClick();
		}

		// --- 우클릭 길게 누르기 처리 ---
		if (Input.GetMouseButtonDown(1))
		{
			isRightClickHeld = true;
			rightClickTimer = 0f;
		}

		if (isRightClickHeld)
		{
			rightClickTimer += Time.deltaTime;

			// UI Radial Gauge의 fillAmount 갱신
			if (radialGaugeImage != null)
			{
				float fill = Mathf.Clamp01(rightClickTimer / rightClickHoldTime);
				radialGaugeImage.fillAmount = fill;
			}

			// 3초 이상 유지되었으면 상호작용 가능하도록 설정하고 로그 남김
			if (rightClickTimer >= rightClickHoldTime && !ableInterection)
			{
				ableInterection = true;
				interactionItem = "YourInteraction";  // 원하는 상호작용 항목 문자열로 수정하세요.
				Debug.Log("Able interaction triggered! " + interactionItem);
				// 3초 도달 후 추가 처리가 필요하면 여기에 구현 (예: 게이지 애니메이션 고정 등)
				isRightClickHeld = false;
			}
		}

		if (Input.GetMouseButtonUp(1))
		{
			isRightClickHeld = false;
			rightClickTimer = 0f;
			if (radialGaugeImage != null)
			{
				radialGaugeImage.fillAmount = 0f;
			}
		}

		// --- 선택된 오브젝트가 있을 경우 이동 처리 ---
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

		// 오브젝트 파괴 감지: 파괴되고 아직 WellDestroyed() 호출 안했을 경우 1회만 실행
		if (selected == null && !wellDestroyedCalled)
		{
			wellDestroyedCalled = true;
			isCorrect = WellDestroyed();
			Debug.Log("WellDestroyed 결과: " + isCorrect);
		}
	}

	void FindLeftClick()
	{
		RaycastHit hit;
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit))
		{
			if (hit.transform != null)
			{
				selected = hit.transform.gameObject;
				Debug.Log("Selected Object: " + selected.name);
				isDragging = true;
				selectedTag = selected.tag;
				lastMousePosition = Input.mousePosition;
				wellDestroyedCalled = false;
			}
		}
	}

	void LostLeftClick()
	{
		if (isDragging)
		{
			isDragging = false;
			selectedTag = null;
			Debug.Log("드래그 종료");
		}
	}

	void DragMove()
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

	void WASDMove()
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

	void Clamping()
	{
		Vector3 pos = selected.transform.position;
		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
		selected.transform.position = latestPos = pos;
	}

	bool WellDestroyed()
	{
		Debug.Log($"{lastMousePosition.x} {lastMousePosition.y} {lastMousePosition.z}");
		if (selectedTag == "NC" || selectedTag == "SC")
		{
			return lastMousePosition.x <= washMaxX && lastMousePosition.x >= washMinX &&
				   lastMousePosition.z <= washMaxZ && lastMousePosition.z >= washMinZ;
		}
		else
		{
			return lastMousePosition.x <= trashMaxX && lastMousePosition.x >= trashMinX &&
				   lastMousePosition.z <= trashMaxZ && lastMousePosition.z >= trashMinZ;
		}
	}
}
