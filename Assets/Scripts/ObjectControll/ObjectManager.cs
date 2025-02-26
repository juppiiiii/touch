using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
	private bool isCorrect = true;
	public GameObject selected;
	private bool isDragging = false;
	private Vector3 lastMousePosition;
	private Vector3 latestPos;
	public string selectedTag;
	private Camera mainCamera;
	public bool leftHold = false;

	public GameObject gm;
	public GameManager gameManager;

	// 이동 가능 영역 한계
	private float maxZ = -0.5f;
	private float minZ = -19.5f;
	private float minX = 0.5f;
	private float maxX = 19.5f;

	// 쓰레기통과 세탁기의 영역
	private float washMinX = 0;
	private float washMaxX = 10;
	private float washMinZ = -20;
	private float washMaxZ = -10;
	private float trashMinX = 10;
	private float trashMaxX = 20;
	private float trashMinZ = -10;
	private float trashMaxZ = 0;

	// WellDestroyed가 1회만 호출되도록 제어하는 플래그 변수
	private bool wellDestroyedCalled = false;

	// 우클릭 길게 누르기 위한 변수들
	public bool ableInterection = false;           // 3초 동안 우클릭 유지 시 true가 됨
	public string interactionWith = "";             // 상호작용 실행 항목을 알리기 위한 문자열 변수
	private float rightClickTimer = 0f;             // 우클릭 지속시간 체크용 타이머
	public float rightClickHoldTime = 3f;           // 3초가 되어야 효과 발생
	private bool isRightClickHeld = false;          // 현재 우클릭이 유지되고 있는지 여부

	private void Start()
	{
		mainCamera = Camera.main;
		gm = GameObject.Find("GameObject");
		gameManager = gm.GetComponent<GameManager>();
	}

	private void Update()
	{
		LeftControl();
		RightControl();

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

		// 오브젝트 파괴 감지: 파괴되고 아직 WellDestroyed()를 호출하지 않았다면 1회만 실행
		if (selected == null && !wellDestroyedCalled)
		{
			wellDestroyedCalled = true;
			isCorrect = WellDestroyed();
			Debug.Log("WellDestroyed 결과: " + isCorrect);
		}
	}

	void LeftControl()
	{
		// 마우스 좌클릭 처리
		if (Input.GetMouseButtonDown(0))
		{
			FindLeftClick();
		}
		if (Input.GetMouseButtonUp(0))
		{
			LostLeftClick();
		}
	}

	// 좌클릭 감지: 오브젝트 선택 및 드래그 시작
	void FindLeftClick()
	{
		RaycastHit hit;
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit))
		{
			if (hit.transform != null)
			{
				selected = hit.transform.gameObject; // 클릭한 오브젝트 저장
				//Debug.Log("Selected Object: " + selected.name); // 디버깅용

				// 드래그 시작 설정
				isDragging = true;
				selectedTag = selected.tag;
				leftHold = true;

				// 마우스 이전 위치 초기화
				lastMousePosition = Input.mousePosition;

				// 새 오브젝트 선택 시 WellDestroyed 호출 플래그 초기화
				wellDestroyedCalled = false;
			}
		}
	}

	// 좌클릭 해제 감지
	void LostLeftClick()
	{
		if (isDragging)
		{
			isDragging = false;
			leftHold = false;
			Debug.Log("드래그 종료");
		}
	}

	// 드래그 이동 처리
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

	// WASD 키 이동 처리(장난감 한정.)
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

	//마우스 우클릭 제어
	void RightControl()
	{
		if (Input.GetMouseButton(1))
			FindRightClick();

		// 마우스 우클릭 길게 누르기 처리, selected가 있어야 의미가 있음.
		if (Input.GetMouseButtonDown(1) && selected != null)
		{
			isRightClickHeld = true;
			rightClickTimer = 0f;
			// 여기서 원형 게이지 객체를 생성할 수 있음.
			gameManager.StartFillingInteractionGauge(3, 3.3f);
			
		}

		if (isRightClickHeld)
		{
			rightClickTimer += Time.deltaTime;

			// 3초 이상 눌렀고 아직 상호작용 실행이 안된 경우
			if (rightClickTimer >= rightClickHoldTime && !ableInterection)
			{
				interactionWith = $"inter_{selected.name}";
				Debug.Log($"inter_{selected.name}");
				ableInterection = true;
				// 3초 도달 후 게이지는 더 이상 진행되지 않음
				isRightClickHeld = false;
			}
		}

		if (Input.GetMouseButtonUp(1))
		{
			// 우클릭 해제 시 타이머와 상태 초기화
			isRightClickHeld = false;
			rightClickTimer = 0f;
			interactionWith = "";
			ableInterection = false;
			Debug.Log($"gauge : {gameManager.InteractionGauge}");
		}
	}

	void FindRightClick()
	{
		RaycastHit hit;
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit))
		{
			if (hit.transform != null)
			{
				selected = hit.transform.gameObject; // 클릭한 오브젝트 저장
				//Debug.Log("Selected Object: " + selected.name); // 디버깅용

				// 마우스 이전 위치 초기화
				lastMousePosition = Input.mousePosition;
			}
			else
			{
				selected = null;
			}
		}
	}

	// 오브젝트 위치 제한
	void Clamping()
	{
		Vector3 pos = selected.transform.position;
		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
		selected.transform.position = latestPos = pos;
	}

	// 올바른 위치에 도달했는지 검사하는 함수
	bool WellDestroyed()
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
