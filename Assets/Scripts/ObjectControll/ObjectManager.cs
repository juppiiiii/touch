using System.Runtime.Serialization;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
	//
	private bool isCorrect = true;
	public GameObject selected;
	private bool isDragging = false;
	private Vector3 lastMousePosition;
	private Vector3 latestPos;
	public string selectedTag;
	private Camera mainCamera;
	public bool leftHold = false;

<<<<<<< Updated upstream
	//���ӸŴ���
=======
	//쌓기 가능한 물건을 미리 정해둠
	private List<string> stackAble = new List<string>(){ "CleanBed", "Bookcase", "LowBookcase", "WrappedBox", "WrappedSB", "BottleWater", "OpenedBook", "FO", "NT"};

	//게임매니저
>>>>>>> Stashed changes
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
		//������ ���콺 �̵��� �����ϵ��� ����.
		//if (!gameManager.IsNight) >> ���� �ݿ��� ������ �����ϱ�...
		{
			LeftControl();
			RightControl();

			//���õ� ������Ʈ�� ���� ��� �̵� ó��
			if (selected != null)
			{
				if (isDragging)
				{
					DragMove();
				}
				/*else >> �㿡 �����̴� �峭������ ���� �̵�. �ű� ����
				{
					WASDMove();
				}*/
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

		int ignoreLayers = LayerMask.GetMask("Ignore Raycast", "Hover");
		int layerMask = ~ignoreLayers;  // �� �� ���̾ ������ ��� ���̾�

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

		if (hits.Length > 0 && Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
		{
			if (hit.transform != null)
			{
				selected = hit.transform.gameObject; // Ŭ���� ������Ʈ ����
				//Debug.Log("Selected Object: " + selected.name); // ������

				// �巡�� ���� ����-> tag�� �ű� �� �ִ� �������� �Ǻ�
				if (selected.tag != "FO")
				{
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

		int ignoreLayers = LayerMask.GetMask("Ignore Raycast", "Hover");
		int layerMask = ~ignoreLayers;  // �� �� ���̾ ������ ��� ���̾�

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

		if (hits.Length > 0 && Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
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

<<<<<<< Updated upstream
	// �ùٸ� ��ġ�� �����ߴ��� �˻��ϴ� �Լ�
=======

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
			//Debug.Log($"other : {other.transform.position.x} {other.transform.position.y} {other.transform.position.z}");
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
				Debug.Log(stackAble.Contains(selected.name));
				Debug.Log(stackAble.Contains(other.name));
				// 쌓기가 가능한 경우(어떻게 접근할지 고민중)
				if (stackAble.Contains(selected.name) && stackAble.Contains(other.name))
				{
					Debug.Log(distance);
					Vector3 dist = new Vector3(selected.transform.localScale.x - other.transform.localScale.x, other.transform.localScale.z, selected.transform.localScale.z - other.transform.localScale.z);
					Vector3 gap = new Vector3(selected.transform.position.x - other.transform.position.x, 0, selected.transform.position.z - other.transform.position.z);
					Debug.Log($"other : {direction.x} {direction.y} {direction.z}");
					// direction(법선)와 distance를 이용해 selected 오브젝트를 밀어냄
					selected.transform.position += (dist - gap) * (distance + 1f);
				}
				//쌓기가 불가능한 경우 - 옆으로 밀어낸다.(겹치지 않도록)
				else
				{
					Vector3 gap = new Vector3(selected.transform.position.x - other.transform.position.x, 0, selected.transform.position.z - other.transform.position.z);Debug.Log($"{gap.x}, {gap.z}");
					Vector3 dist = -gap.x < gap.z ? new Vector3(other.transform.localScale.x, 0, 0) : new Vector3(0, 0, -other.transform.localScale.y);
					
					Debug.Log($"other : {direction.x} {direction.y} {direction.z}");
					// direction(법선)와 distance를 이용해 selected 오브젝트를 밀어냄
					selected.transform.position += (dist - gap) * (distance + 1f);
				}
			}
		}
	}
	// 올바른 위치에 도달했는지 검사하는 함수
>>>>>>> Stashed changes
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
