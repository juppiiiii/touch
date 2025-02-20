using UnityEngine;

public class ObjectManager : MonoBehaviour {
	public GameObject selected;
	private bool isDragging = false;
	private Vector3 lastMousePosition;
	private Camera mainCamera;

	private void Start()
	{
		mainCamera = Camera.main;
	}

	private void Update()
	{
		// 마우스 좌클릭 감지
		if (Input.GetMouseButtonDown(0))
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

					// 마우스 이전 위치 초기화
					lastMousePosition = Input.mousePosition;
				}
			}
		}

		// 마우스 좌클릭 해제 감지
		if (Input.GetMouseButtonUp(0))
		{
			if (isDragging)
			{
				isDragging = false;
				Debug.Log("드래그 종료");
			}
		}

		// 선택된 오브젝트가 있을 때
		if (selected != null)
		{
			// 드래그 이동 처리
			if (isDragging)
			{
				// 현재 마우스 위치
				Vector3 currentMousePosition = Input.mousePosition;

				// 마우스 이동량 계산
				Vector3 deltaMousePosition = currentMousePosition - lastMousePosition;

				// 화면 좌표계에서 월드 좌표계로 변환
				Vector3 worldDelta = ScreenToWorldDelta(deltaMousePosition);

				// 오브젝트 위치 업데이트
				selected.transform.position += worldDelta;

				// 마우스 이전 위치 업데이트
				lastMousePosition = currentMousePosition;
			}
			else
			{
				// WASD 이동 처리 (드래그 중이 아닐 때)
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
		}
	}

	// 화면 좌표계의 이동량을 월드 좌표계의 이동량으로 변환하는 함수
	Vector3 ScreenToWorldDelta(Vector3 screenDelta)
	{
		// 오브젝트의 현재 위치를 화면 좌표로 변환
		Vector3 screenPosition = mainCamera.WorldToScreenPoint(selected.transform.position);

		// 새로운 화면 좌표 계산
		Vector3 newScreenPosition = screenPosition + screenDelta;

		// 새로운 화면 좌표를 월드 좌표로 변환
		Vector3 newWorldPosition = mainCamera.ScreenToWorldPoint(newScreenPosition);

		// 월드 좌표계에서의 이동량 계산
		Vector3 worldDelta = newWorldPosition - selected.transform.position;

		// y축 이동을 없애고, z축 이동으로 대체 (마우스 y 이동을 z축 이동으로 매핑)
		worldDelta.y = 0f;

		return worldDelta;
	}
}
