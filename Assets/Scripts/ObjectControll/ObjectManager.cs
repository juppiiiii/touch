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

	// �̵� ���� ���� �Ѱ�
	private float maxZ = -0.5f;
	private float minZ = -19.5f;
	private float minX = 0.5f;
	private float maxX = 19.5f;

	// ��������� ��Ź���� ����
	private float washMinX = 0;
	private float washMaxX = 10;
	private float washMinZ = -20;
	private float washMaxZ = -10;
	private float trashMinX = 10;
	private float trashMaxX = 20;
	private float trashMinZ = -10;
	private float trashMaxZ = 0;

	// WellDestroyed�� 1ȸ�� ȣ��ǵ��� �����ϴ� �÷��� ����
	private bool wellDestroyedCalled = false;

	// ��Ŭ�� ��� ������ ���� ������
	public bool ableInterection = false;           // 3�� ���� ��Ŭ�� ���� �� true�� ��
	public string interactionWith = "";             // ��ȣ�ۿ� ���� �׸��� �˸��� ���� ���ڿ� ����
	private float rightClickTimer = 0f;             // ��Ŭ�� ���ӽð� üũ�� Ÿ�̸�
	public float rightClickHoldTime = 3f;           // 3�ʰ� �Ǿ�� ȿ�� �߻�
	private bool isRightClickHeld = false;          // ���� ��Ŭ���� �����ǰ� �ִ��� ����

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

	void LeftControl()
	{
		// ���콺 ��Ŭ�� ó��
		if (Input.GetMouseButtonDown(0))
		{
			FindLeftClick();
		}
		if (Input.GetMouseButtonUp(0))
		{
			LostLeftClick();
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
				//Debug.Log("Selected Object: " + selected.name); // ������

				// �巡�� ���� ����
				isDragging = true;
				selectedTag = selected.tag;
				leftHold = true;

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
			leftHold = false;
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

	// WASD Ű �̵� ó��(�峭�� ����.)
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

	//���콺 ��Ŭ�� ����
	void RightControl()
	{
		if (Input.GetMouseButton(1))
			FindRightClick();

		// ���콺 ��Ŭ�� ��� ������ ó��, selected�� �־�� �ǹ̰� ����.
		if (Input.GetMouseButtonDown(1) && selected != null)
		{
			isRightClickHeld = true;
			rightClickTimer = 0f;
			// ���⼭ ���� ������ ��ü�� ������ �� ����.
			gameManager.StartFillingInteractionGauge(3, 3.3f);
			
		}

		if (isRightClickHeld)
		{
			rightClickTimer += Time.deltaTime;

			// 3�� �̻� ������ ���� ��ȣ�ۿ� ������ �ȵ� ���
			if (rightClickTimer >= rightClickHoldTime && !ableInterection)
			{
				interactionWith = $"inter_{selected.name}";
				Debug.Log($"inter_{selected.name}");
				ableInterection = true;
				// 3�� ���� �� �������� �� �̻� ������� ����
				isRightClickHeld = false;
			}
		}

		if (Input.GetMouseButtonUp(1))
		{
			// ��Ŭ�� ���� �� Ÿ�̸ӿ� ���� �ʱ�ȭ
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
				selected = hit.transform.gameObject; // Ŭ���� ������Ʈ ����
				//Debug.Log("Selected Object: " + selected.name); // ������

				// ���콺 ���� ��ġ �ʱ�ȭ
				lastMousePosition = Input.mousePosition;
			}
			else
			{
				selected = null;
			}
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
}
