using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
	//프리팹 호출
	private bool[] calledPrefeb = new bool[3];
	private int prefebCnt = 0;
	public GameObject d1, d2, d3;
	// 미니게임 호출과 회상씬
	public int[] callMiniGame = { 1, 2, 4};
	public bool showInteraction = false;

	//미니게임 게임오브젝트
	public GameObject g1, g2, g3, g4;

	private bool isCorrect = true;
	public GameObject selected;
	private bool isDragging = false;
	private Vector3 lastMousePosition;
	private Vector3 latestPos;
	public string selectedTag;
	private Camera mainCamera;
	public bool leftHold = false;

	//쌓기 가능한 물건을 미리 정해둠
	private List<string> stackAble = new List<string>(){ "CleanBed", "Bookcase", "LowBookcase", "WrappedBox", "WrappedSB", "BottleWater", "OpenedBook", "FO", "NT"};

	// 이동 가능 영역 한계
	private float maxZ = -0f;
	private float minZ = -20f;
	private float minX = 0f;
	private float maxX = 20f;

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
	}

	private void Update()
	{
        //미니게임 디버깅
        /*if (Input.GetKeyDown("m"))
        {
			PlayMiniGame();
        }*/
        //낮에만 마우스 이동이 가능하도록 한정.
        if (!GameManager.Instance.IsNight)
		{
			//낮이 되었을 때 프리팹 1회 호출
			if (prefebCnt < 3)
			{
				 if (!calledPrefeb[prefebCnt]) // 최초 1회만.
				{

					calledPrefeb[prefebCnt] = true;
					switch (prefebCnt)
					{
						case 0:
							Vector3 p1 = new Vector3(8.4f, 5.1f, 10.2f);
							Instantiate(d1, p1, Quaternion.identity);
							return;
						case 1:
							Destroy(d1);
							Vector3 p2 = new Vector3(-8.83f, 6.2f, 4f);
							Instantiate(d2, p2, Quaternion.identity);
							return;
						case 2:
							Destroy(d2);
							Vector3 p3 = new Vector3(2.9f, 12.3f, -32.2f);
							Instantiate(d3, p3, Quaternion.identity);
							return;
					}
				}
			}

			LeftControl();
			RightControl();
			//선택된 오브젝트가 있을 경우 이동 처리
			if (selected != null)
			{
				if (isDragging)
				{
					DragMove();
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
		//else selectd = null;
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

		int ignoreLayers = LayerMask.GetMask("Ignore Raycast", "Hover");
		int layerMask = ~ignoreLayers;  // 위 두 레이어를 제외한 모든 레이어

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

		if (hits.Length > 0 && Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
		{
			if (hit.transform != null && hit.transform.tag != "CannotMove")
			{
				selected = hit.transform.gameObject; // 클릭한 오브젝트 저장
				//Debug.Log("Selected Object: " + selected.name); // 디버깅용

				// 드래그 시작 설정-> tag로 옮길 수 있는 물건인지 판별
				if (selected.tag != "FO")
				{
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
	}

	// 좌클릭 해제 감지
	void LostLeftClick()
	{
		if (isDragging)
		{
			isDragging = false;
			leftHold = false;
			Debug.Log("드래그 종료");
			ResolveOverlaps();
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
			GameManager.Instance.StartFillingInteractionGauge(3, 3.3f);
			
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
			//상호작용이 가능하다면 미니게임 실행
			if (ableInterection)
			{
				if (selected.tag == "SC" || selected.tag == "ST")
				{
					Debug.Log("Sc or ST");
					PlayMiniGame();
				}
			}
			// 우클릭 해제 시 타이머와 상태 초기화
			isRightClickHeld = false;
			rightClickTimer = 0f;
			interactionWith = "";
			ableInterection = false;
			Debug.Log($"gauge : {GameManager.Instance.InteractionGauge}");
			GameManager.Instance.ResetInteractionGauge();
		}
	}

	//미니게임을 실행. 이기면 미니게임 상호작용 출력 가능하게 불값 반환
	void PlayMiniGame()
	{
		int chooseGame = callMiniGame[Random.Range(1, 100) % 3];
		GameManager.Instance.PauseTimer();
		switch (chooseGame) 
		{
			case 1:
				Debug.Log("1번 게임 실행");
				G1Manager g1Manager = new G1Manager();
				g1.SetActive(true);
				showInteraction = g1Manager.win;
				return;
			case 2:
				Debug.Log("2번 게임 실행");
				G1Manager g2Manager = new G1Manager();
				g2.SetActive(true);
				showInteraction = g2Manager.win;
				return;
			case 3:// 3번 게임은 오류 발생으로 실행 불가
				return;
			case 4:
				Debug.Log("4번 게임 실행");
				G1Manager g4Manager = new G1Manager();
				g4.SetActive(true);
				showInteraction = g4Manager.win;
				return;
		}
		GameManager.Instance.ResumeTimer();
	}

	void FindRightClick()
	{
		RaycastHit hit;

		int ignoreLayers = LayerMask.GetMask("Ignore Raycast", "Hover");
		int layerMask = ~ignoreLayers;  // 위 두 레이어를 제외한 모든 레이어

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

		if (hits.Length > 0 && Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
		{
			if (hit.transform != null)
			{
				selected = hit.transform.gameObject; // 클릭한 오브젝트 저장
													 //Debug.Log("Selected Object: " + selected.name); // 디버깅용

				// 마우스 이전 위치 초기화
				lastMousePosition = Input.mousePosition;
			}
		}
		else selected = null;
	}

	// 오브젝트 위치 제한
	void Clamping()
	{
		Vector3 pos = selected.transform.position;
		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
		selected.transform.position = latestPos = pos;
	}

	//오브젝트의 겹침 방지
	void ResolveOverlaps()
	{
		// selected 오브젝트가 반드시 Collider를 가지고 있어야 함
		Collider selectedCollider = selected.GetComponent<Collider>();
		if (selectedCollider == null)
		{
			return;
		}
			

		// selected의 현재 위치에서 겹치는 다른 Collider들을 찾음
		Collider[] overlappingColliders = Physics.OverlapBox(
			selectedCollider.bounds.center,
			selectedCollider.bounds.extents,
			selected.transform.rotation);

		// 각 겹치는 오브젝트에 대해 분리 벡터를 계산
		foreach (Collider other in overlappingColliders)
		{
			if (other.gameObject == selected)
				continue; // 자기 자신은 제외

			Vector3 direction;
			Debug.Log($"othername : {other.name} : {other.transform.position.x} {other.transform.position.y} {other.transform.position.z}");
			float distance;
			// 두 Collider가 겹치는지 검사하고, 겹침 정도를 계산
			bool isOverlapping = Physics.ComputePenetration(
				selectedCollider,
				selected.transform.position,
				selected.transform.rotation,
				other,
				other.transform.position,
				other.transform.rotation,
				out direction,
				out distance);

			if (isOverlapping)
			{
				if (stackAble.Contains(selected.name) && stackAble.Contains(other.name))
				{
					Debug.Log(distance);
					Vector3 dist = new Vector3(selected.transform.localScale.x - other.transform.localScale.x, other.transform.localScale.z, selected.transform.localScale.z - other.transform.localScale.z);
					Vector3 gap = new Vector3(selected.transform.position.x - other.transform.position.x, 0, selected.transform.position.z - other.transform.position.z);
					Debug.Log($"other : {direction.x} {direction.y} {direction.z}");
					// direction(법선)와 distance를 이용해 selected 오브젝트를 밀어냄
					selected.transform.position += (dist - gap) * (distance + 1f);
				}
				//쌓기가 불가능한 경우 - 옆으로 밀어낸다.(겹치지 않도록)(이건 other과 selected의 이름이 동일하지 않은 경우에만 적용.)
				/*else if (selected.name != other.name)
				{
					Vector3 gap = new Vector3(selected.transform.position.x - other.transform.position.x, 0, selected.transform.position.z - other.transform.position.z);Debug.Log($"{gap.x}, {gap.z}");
					Vector3 dist = -gap.x < gap.z ? new Vector3(other.transform.localScale.x, 0, 0) : new Vector3(0, 0, -other.transform.localScale.y);
					
					Debug.Log($"other : {direction.x} {direction.y} {direction.z}");
					// direction(법선)와 distance를 이용해 selected 오브젝트를 밀어냄
					selected.transform.position += (dist - gap) * (distance + 1f);
				}*/
			}
		}
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
