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

	//�̵� ���� ���� �Ѱ�
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
		// ���콺 ��Ŭ�� ����
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit))
			{
				if (hit.transform != null)
				{
					selected = hit.transform.gameObject; // Ŭ���� ������Ʈ ����
					Debug.Log("Selected Object: " + selected.name);

					// �巡�� ������ ���� ����
					isDragging = true;
					selectedTag = selected.tag;

					// ���콺 ���� ��ġ �ʱ�ȭ
					lastMousePosition = Input.mousePosition;
				}
			}
		}

		// ���콺 ��Ŭ�� ���� ����
		if (Input.GetMouseButtonUp(0))
		{
			if (isDragging)
			{
				isDragging = false;
				selectedTag = null;
				Debug.Log("�巡�� ����");
			}
		}

		// ���õ� ������Ʈ�� ���� ��
		if (selected != null)
		{
			// �巡�� �̵� ó��
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
				// WASD �̵� ó�� (�巡�� ���� �ƴ� ��)
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
			Debug.Log($"{selectedTag} �ı�����"); // NC��� �±� �ı� ���� ����.
		}
	}

}
