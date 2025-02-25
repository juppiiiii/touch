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

	// �̵� ���� ���� �Ѱ�
	public float maxZ = -1;
	public float minZ = -20.5f;
	public float minX = 1;
	public float maxX = 20.5f;

	// ��������� ��Ź���� ����
	public float washMinX = 0;
	public float washMaxX = 10;
	public float washMinZ = -20;
	public float washMaxZ = -10;
	public float trashMinX = 10;
	public float trashMaxX = 20;
	public float trashMinZ = -10;
	public float trashMaxZ = 0;

	// WellDestroyed�� 1ȸ�� ȣ��ǵ��� �����ϴ� �÷��� ����
	private bool wellDestroyedCalled = false;

	private void Start()
	{
		mainCamera = Camera.main;
	}

	private void Update()
	{
		// ���콺 �Է¿� ���� ó��
		if (Input.GetMouseButtonDown(0))
		{
			FindLeftClick();
		}

		if (Input.GetMouseButtonUp(0))
		{
			LostLeftClick();
		}

		// ���õ� ������Ʈ�� ���� ��� �̵� ���� ó��
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

		// ������Ʈ �ı� ����: �ı��� �����ǰ� ���� WellDestroyed()�� ȣ����� �ʾҴٸ� 1ȸ�� ����
		// Unity�� �ı��� ������Ʈ�� ���� 'selected == null'�� true�� ��ȯ�մϴ�.
		if (selected == null && !wellDestroyedCalled)
		{
			wellDestroyedCalled = true;
			isCorrect = WellDestroyed();
			Debug.Log("WellDestroyed ���: " + isCorrect);
		}
	}

	void FindLeftClick() // ���콺 ��Ŭ�� ����
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

				// �� ������Ʈ�� �����ϸ� ȣ�� �÷��׸� ���� (���ο� ������Ʈ �ı��� �ٽ� ȣ���)
				wellDestroyedCalled = false;
			}
		}
	}

	void LostLeftClick() // ���콺 ��Ŭ�� ���� ����
	{
		if (isDragging)
		{
			isDragging = false;
			selectedTag = null;
			Debug.Log("�巡�� ����");
		}
	}

	void DragMove() // �巡�� �̵�
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

	void WASDMove() // WASD �̵� ó��
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

	void Clamping() // ������Ʈ ��ġ ����
	{
		Vector3 pos = selected.transform.position;
		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
		selected.transform.position = latestPos = pos;
	}

	bool WellDestroyed() // �ùٸ� ��ġ�� ������ �˻��ϴ� �Լ�
	{
		Debug.Log($"{lastMousePosition.x} {lastMousePosition.y} {lastMousePosition.z}");
		// ���õ� �±׿� ���� �˻�
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
