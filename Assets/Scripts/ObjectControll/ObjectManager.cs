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

	// === ��Ŭ�� ��� �����⸦ ���� ������ ===
	public bool ableInterection = false;  // 3�� ���� ��Ŭ�� ���� �� true�� ������
	public string interactionItem = "";    // ��ȣ�ۿ� ���� �׸��� �˸��� ���� ���ڿ�
	private float rightClickTimer = 0f;
	public float rightClickHoldTime = 3f;
	private bool isRightClickHeld = false;

	// Canvas/UI�� ���� Radial Gauge Image (Inspector �Ҵ�)
	public Image radialGaugeImage;

	private void Start()
	{
		mainCamera = Camera.main;
		if (radialGaugeImage != null)
			radialGaugeImage.fillAmount = 0f;  // �ʱ� ä��� ���� 0
	}

	private void Update()
	{
		// --- ��Ŭ�� ó�� ---
		if (Input.GetMouseButtonDown(0))
		{
			FindLeftClick();
		}
		if (Input.GetMouseButtonUp(0))
		{
			LostLeftClick();
		}

		// --- ��Ŭ�� ��� ������ ó�� ---
		if (Input.GetMouseButtonDown(1))
		{
			isRightClickHeld = true;
			rightClickTimer = 0f;
		}

		if (isRightClickHeld)
		{
			rightClickTimer += Time.deltaTime;

			// UI Radial Gauge�� fillAmount ����
			if (radialGaugeImage != null)
			{
				float fill = Mathf.Clamp01(rightClickTimer / rightClickHoldTime);
				radialGaugeImage.fillAmount = fill;
			}

			// 3�� �̻� �����Ǿ����� ��ȣ�ۿ� �����ϵ��� �����ϰ� �α� ����
			if (rightClickTimer >= rightClickHoldTime && !ableInterection)
			{
				ableInterection = true;
				interactionItem = "YourInteraction";  // ���ϴ� ��ȣ�ۿ� �׸� ���ڿ��� �����ϼ���.
				Debug.Log("Able interaction triggered! " + interactionItem);
				// 3�� ���� �� �߰� ó���� �ʿ��ϸ� ���⿡ ���� (��: ������ �ִϸ��̼� ���� ��)
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

		// ������Ʈ �ı� ����: �ı��ǰ� ���� WellDestroyed() ȣ�� ������ ��� 1ȸ�� ����
		if (selected == null && !wellDestroyedCalled)
		{
			wellDestroyedCalled = true;
			isCorrect = WellDestroyed();
			Debug.Log("WellDestroyed ���: " + isCorrect);
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
			Debug.Log("�巡�� ����");
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
