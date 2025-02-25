using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;  // UI ��� ��

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

	// ============================
	// ��Ŭ�� ��� ������ ���� ������
	// ============================
	public bool ableInterection = false;           // 3�� ���� ��Ŭ�� ���� �� true�� ��
	public string interactionItem = "";             // ��ȣ�ۿ� ���� �׸��� �˸��� ���� ���ڿ� ����
	private float rightClickTimer = 0f;             // ��Ŭ�� ���ӽð� üũ�� Ÿ�̸�
	public float rightClickHoldTime = 3f;           // 3�ʰ� �Ǿ�� ȿ�� �߻�
	private bool isRightClickHeld = false;          // ���� ��Ŭ���� �����ǰ� �ִ��� ����

	// UI�� ����� ���� �������� ǥ���� ���, �Ʒ� �������� �̿��� �� ���� (���û���)
	// public Image circularGaugeImage;

	private void Start()
	{
		mainCamera = Camera.main;
	}

	private void Update()
	{
		// --- ���콺 ��Ŭ�� ó�� ---
		if (Input.GetMouseButtonDown(0))
		{
			FindLeftClick();
		}
		if (Input.GetMouseButtonUp(0))
		{
			LostLeftClick();
		}

		// --- ���콺 ��Ŭ�� ��� ������ ó�� ---
		if (Input.GetMouseButtonDown(1))
		{
			isRightClickHeld = true;
			rightClickTimer = 0f;
			// (�ɼ�) ���� UI prefab�� ����Ѵٸ� ���⼭ ���� ������ ��ü�� ������ �� ����.
		}

		if (isRightClickHeld)
		{
			rightClickTimer += Time.deltaTime;
			// (�ɼ�) UI Image�� ����ϴ� ���, �Ʒ��� ���� ä���� ������ ������Ʈ ����:
			// circularGaugeImage.fillAmount = rightClickTimer / rightClickHoldTime;

			// 3�� �̻� ������ ���� ��ȣ�ۿ� ������ �ȵ� ���
			if (rightClickTimer >= rightClickHoldTime && !ableInterection)
			{
				ableInterection = true;
				interactionItem = "YourInteraction"; // ���ϴ� ���ڿ��� ���� ����
				Debug.Log("Able interaction triggered! " + interactionItem);
				// 3�� ���� �� �������� �� �̻� ������� �ʵ��� ó���� �� ����
				isRightClickHeld = false;
			}
		}

		if (Input.GetMouseButtonUp(1))
		{
			// ��Ŭ�� ���� �� Ÿ�̸ӿ� ���� �ʱ�ȭ
			isRightClickHeld = false;
			rightClickTimer = 0f;
			// (�ɼ�) ������ ������ ��ü�� �ִٸ� ����
		}

		// --- ���õ� ������Ʈ�� ���� ��� �̵� ó�� ---
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

		// ������Ʈ �ı� ����: �ı��ǰ� ���� WellDestroyed()�� ȣ������ �ʾҴٸ� 1ȸ�� ����
		if (selected == null && !wellDestroyedCalled)
		{
			wellDestroyedCalled = true;
			isCorrect = WellDestroyed();
			Debug.Log("WellDestroyed ���: " + isCorrect);
		}
	}

	// ��Ŭ�� ����: ������Ʈ ���� �� �巡�� ����
	void FindLeftClick()
	{
		RaycastHit hit;
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit))
		{
			if (hit.transform != null)
			{
				selected = hit.transform.gameObject; // Ŭ���� ������Ʈ ����
				Debug.Log("Selected Object: " + selected.name);

				// �巡�� ���� ����
				isDragging = true;
				selectedTag = selected.tag;

				// ���콺 ���� ��ġ �ʱ�ȭ
				lastMousePosition = Input.mousePosition;

				// �� ������Ʈ ���� �� WellDestroyed ȣ�� �÷��� �ʱ�ȭ
				wellDestroyedCalled = false;
			}
		}
	}

	// ��Ŭ�� ���� ����
	void LostLeftClick()
	{
		if (isDragging)
		{
			isDragging = false;
			selectedTag = null;
			Debug.Log("�巡�� ����");
		}
	}

	// �巡�� �̵� ó��
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

	// WASD Ű �̵� ó��
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

	// ������Ʈ ��ġ ����
	void Clamping()
	{
		Vector3 pos = selected.transform.position;
		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
		selected.transform.position = latestPos = pos;
	}

	// �ùٸ� ��ġ�� �����ߴ��� �˻��ϴ� �Լ�
	bool WellDestroyed()
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

	// OnGUI�� �̿��� �ӽ÷� ���� �������� ������� ǥ�� (���� ������Ʈ �� Canvas/UI ��� ����)
	private void OnGUI()
	{
		if (isRightClickHeld)
		{
			float gaugeSize = 100;
			float posX = Screen.width - gaugeSize - 10;
			float posY = Screen.height - gaugeSize - 10;
			float fill = Mathf.Clamp01(rightClickTimer / rightClickHoldTime);

			// ������ �ڽ��� ���� ������ ä���� ������ ǥ��
			GUI.Box(new Rect(posX, posY, gaugeSize, gaugeSize), $"Fill: {(fill * 100):0}%");
		}
	}
}
