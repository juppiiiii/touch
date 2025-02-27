using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyMovement : MonoBehaviour {

	public GameObject hand;
	public GameObject selected;
	private bool isDragging = false;
	public string selectedTag;
	private Camera mainCamera;
	public bool leftHold = false;
	// 입력이 무시되는 상태 여부
	private bool disabled = false;

	// 입력 비활성화 지속 시간 (초)
	public float disableDuration = 5.0f;

	// 이동 속도 (유닛/초)
	public float moveSpeed = 5f;
	// 충돌 감지 시 여유를 주기 위한 거리
	public float collisionMargin = 0.1f;

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
		//gm = GameObject.Find("GameObject");
		//gameManager = gm.GetComponent<GameManager>();
	}

	private void Update()
	{
		//밤에만 마우스 이동이 가능하도록 한정.
		//if (gameManager.IsNight)
		{
			if (disabled) // 속박당한 경우
			{
				Interrupted(disableDuration);
			}

			LeftControl(); // 손길 잡기 시도

			RightControl(); // 장난감을 선택 시도
			
			if (Input.GetKeyDown("q")) //Q버튼 입력을 받은 경우 장난감 선택 해제
			{
				CancelToyCTRL();
			}
			
			if (leftHold) //좌클릭으로 손길 이속 디버프
			{
				HoldHand();
			}
			
			if (selected != null) //선택된 오브젝트가 있을 경우 이동 가능
			{
				WASDMovement();
			}
		}
	}

	void LeftControl() // 손길 이동 지연
	{
		// 마우스 좌클릭 처리
		if (Input.GetMouseButtonDown(0))
		{
			FindLeftClick(); // 손길의 이동을 지연시키는 동작
		}
		if (Input.GetMouseButtonUp(0))
		{
			LostLeftClick(); // 이동 지연 종료
		}
	}

	// 좌클릭 감지: 손길 여부 판별 & 손길이 들어와 있는 경우 해당 손길 속도 30% 저하
	void FindLeftClick()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit) && !leftHold)
		{
			if (hit.transform != null && hit.transform.tag == "Hand") // 태그로 손길을 판단하려고 이렇게 함.
			{
				hand = hit.transform.gameObject; // 클릭한 오브젝트 저장
				//Debug.Log("Selected Object: " + selected.name); // 디버깅용
			}
		}
	}

	// 좌클릭 해제 감지 : 손길 오브젝트를 null로 설정. 속도 감소 제거
	void LostLeftClick()
	{
		if (hand != null)
		{
			hand = null;
			leftHold = false;
			Debug.Log("손 잡기 해제");
		}
	}

	//wasd 움직임
	void WASDMovement()
	{
		// WASD 입력을 개별적으로 읽음
		float horizontal = 0f;
		if (Input.GetKey(KeyCode.A))
			horizontal -= 1f;
		if (Input.GetKey(KeyCode.D))
			horizontal += 1f;

		float vertical = 0f;
		if (Input.GetKey(KeyCode.W))
			vertical += 1f;
		if (Input.GetKey(KeyCode.S))
			vertical -= 1f;

		// 입력 벡터 생성 (대각선 입력 가능)
		Vector3 inputVector = new Vector3(horizontal, 0, vertical);

		// 입력된 값이 있다면 처리
		if (inputVector != Vector3.zero)
		{
			// 각 축마다 이동이 가능한지 독립적으로 확인하여 allowedMovement에 누적
			Vector3 allowedMovement = Vector3.zero;
			float frameMove = moveSpeed * Time.deltaTime;
			float checkDistance = frameMove + collisionMargin;

			// 수평 이동 체크 (X축)
			if (Mathf.Abs(horizontal) > 0)
			{
				Vector3 horizDir = new Vector3(horizontal, 0, 0).normalized;
				// 현재 위치에서 수평 방향으로 ray를 쏩니다.
				if (!Physics.Raycast(transform.position, horizDir, checkDistance))
				{
					allowedMovement.x = horizontal;
				}
				else
				{
					// 디버그용:
					// Debug.Log("수평 이동 차단: " + horizDir);
				}
			}
			// 수직 이동 체크 (Z축)
			if (Mathf.Abs(vertical) > 0)
			{
				Vector3 vertDir = new Vector3(0, 0, vertical).normalized;
				if (!Physics.Raycast(transform.position, vertDir, checkDistance))
				{
					allowedMovement.z = vertical;
				}
				else
				{
					// Debug.Log("수직 이동 차단: " + vertDir);
				}
			}
			// allowedMovement가 (1, 0, 1) 또는 (-1,0, -1) 등 대각선 입력이면, 크기가 √2가 되므로
			// 전체 이동 속도가 증가하는 것을 막기 위해 정규화.
			if (allowedMovement.magnitude > 1)
			{
				allowedMovement = allowedMovement.normalized;
			}

			// 최종 이동 값 계산 후 이동 (World space 기준)
			Vector3 translation = allowedMovement * moveSpeed * Time.deltaTime;

			selected.transform.Translate(translation, Space.World);
		}
	}

	//손을 잡고 있으면 속도를 느리게 할 수 있다.
	//이건 어떻게 전달이 오는거지?
	void HoldHand()
	{
		//hand gameobject의 이동 제한
	}

	//손길에 잡히면 확률적으로 방해를 받는다.
	// 외부에서 호출하여 입력을 일정 시간 동안 막는 함수
	public void Interrupted(float duration)
	{
		StartCoroutine(DisableInputCoroutine(duration));
	}

	private IEnumerator DisableInputCoroutine(float duration) // coroutine으로 일정시간 속박
	{
		yield return new WaitForSeconds(duration);
		disabled = false;
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
			gameManager.ResetInteractionGauge();
		}
	}

	void FindRightClick() // 장난감 여부 판별 후 장난감을 선택한 상태로 변경
	{
		RaycastHit hit;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit))
		{
			if (hit.transform != null && hit.transform.tag == "Toy")
			{
				selected = hit.transform.gameObject; // 클릭한 오브젝트 저장
				//Debug.Log("Selected Object: " + selected.name); // 디버깅용
			}
		}
	}

	void CancelToyCTRL() // 장난감 선택 해제(임시...)
	{
		if (selected != null)
		{
			selected = null;
		}
	}
}
