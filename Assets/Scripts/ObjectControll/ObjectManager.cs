using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
	private bool isCorret;
	public GameObject selected;
	private bool isDragging = false;
	private Vector3 lastMousePosition;
	private Vector3 latestPos;
	private string selectedTag;
	private Camera mainCamera;

	//이동 가능 영역 한계
	public float maxZ = -1;
	public float minZ = -20.5f;
	public float minX = 1;
	public float maxX = 20.5f;

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
					selectedTag = selected.tag;

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
				selectedTag = null;
				Debug.Log("드래그 종료");
			}
		}

		// 선택된 오브젝트가 있을 때
		if (selected != null)
		{
			// 드래그 이동 처리
			if (isDragging)
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

				lastMousePosition = Input.mousePosition;
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
			//clamp
			Vector3 pos = selected.transform.position;
			pos.x = Mathf.Clamp(pos.x, minX, maxX);
			pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
			selected.transform.position = latestPos = pos;
			
		}
		if (selected.IsDestroyed())
		{
			Debug.Log($"{selectedTag} 파괴감지"); // NC라는 태그 파괴 감지 성공.
		}
	}

}
