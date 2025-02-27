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
	// �Է��� ���õǴ� ���� ����
	private bool disabled = false;

	// �Է� ��Ȱ��ȭ ���� �ð� (��)
	public float disableDuration = 5.0f;

	// �̵� �ӵ� (����/��)
	public float moveSpeed = 5f;
	// �浹 ���� �� ������ �ֱ� ���� �Ÿ�
	public float collisionMargin = 0.1f;

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
		//gm = GameObject.Find("GameObject");
		//gameManager = gm.GetComponent<GameManager>();
	}

	private void Update()
	{
		//�㿡�� ���콺 �̵��� �����ϵ��� ����.
		//if (gameManager.IsNight)
		{
			if (disabled) // �ӹڴ��� ���
			{
				Interrupted(disableDuration);
			}

			LeftControl(); // �ձ� ��� �õ�

			RightControl(); // �峭���� ���� �õ�
			
			if (Input.GetKeyDown("q")) //Q��ư �Է��� ���� ��� �峭�� ���� ����
			{
				CancelToyCTRL();
			}
			
			if (leftHold) //��Ŭ������ �ձ� �̼� �����
			{
				HoldHand();
			}
			
			if (selected != null) //���õ� ������Ʈ�� ���� ��� �̵� ����
			{
				WASDMovement();
			}
		}
	}

	void LeftControl() // �ձ� �̵� ����
	{
		// ���콺 ��Ŭ�� ó��
		if (Input.GetMouseButtonDown(0))
		{
			FindLeftClick(); // �ձ��� �̵��� ������Ű�� ����
		}
		if (Input.GetMouseButtonUp(0))
		{
			LostLeftClick(); // �̵� ���� ����
		}
	}

	// ��Ŭ�� ����: �ձ� ���� �Ǻ� & �ձ��� ���� �ִ� ��� �ش� �ձ� �ӵ� 30% ����
	void FindLeftClick()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit) && !leftHold)
		{
			if (hit.transform != null && hit.transform.tag == "Hand") // �±׷� �ձ��� �Ǵ��Ϸ��� �̷��� ��.
			{
				hand = hit.transform.gameObject; // Ŭ���� ������Ʈ ����
				//Debug.Log("Selected Object: " + selected.name); // ������
			}
		}
	}

	// ��Ŭ�� ���� ���� : �ձ� ������Ʈ�� null�� ����. �ӵ� ���� ����
	void LostLeftClick()
	{
		if (hand != null)
		{
			hand = null;
			leftHold = false;
			Debug.Log("�� ��� ����");
		}
	}

	//wasd ������
	void WASDMovement()
	{
		// WASD �Է��� ���������� ����
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

		// �Է� ���� ���� (�밢�� �Է� ����)
		Vector3 inputVector = new Vector3(horizontal, 0, vertical);

		// �Էµ� ���� �ִٸ� ó��
		if (inputVector != Vector3.zero)
		{
			// �� �ึ�� �̵��� �������� ���������� Ȯ���Ͽ� allowedMovement�� ����
			Vector3 allowedMovement = Vector3.zero;
			float frameMove = moveSpeed * Time.deltaTime;
			float checkDistance = frameMove + collisionMargin;

			// ���� �̵� üũ (X��)
			if (Mathf.Abs(horizontal) > 0)
			{
				Vector3 horizDir = new Vector3(horizontal, 0, 0).normalized;
				// ���� ��ġ���� ���� �������� ray�� ���ϴ�.
				if (!Physics.Raycast(transform.position, horizDir, checkDistance))
				{
					allowedMovement.x = horizontal;
				}
				else
				{
					// ����׿�:
					// Debug.Log("���� �̵� ����: " + horizDir);
				}
			}
			// ���� �̵� üũ (Z��)
			if (Mathf.Abs(vertical) > 0)
			{
				Vector3 vertDir = new Vector3(0, 0, vertical).normalized;
				if (!Physics.Raycast(transform.position, vertDir, checkDistance))
				{
					allowedMovement.z = vertical;
				}
				else
				{
					// Debug.Log("���� �̵� ����: " + vertDir);
				}
			}
			// allowedMovement�� (1, 0, 1) �Ǵ� (-1,0, -1) �� �밢�� �Է��̸�, ũ�Ⱑ ��2�� �ǹǷ�
			// ��ü �̵� �ӵ��� �����ϴ� ���� ���� ���� ����ȭ.
			if (allowedMovement.magnitude > 1)
			{
				allowedMovement = allowedMovement.normalized;
			}

			// ���� �̵� �� ��� �� �̵� (World space ����)
			Vector3 translation = allowedMovement * moveSpeed * Time.deltaTime;

			selected.transform.Translate(translation, Space.World);
		}
	}

	//���� ��� ������ �ӵ��� ������ �� �� �ִ�.
	//�̰� ��� ������ ���°���?
	void HoldHand()
	{
		//hand gameobject�� �̵� ����
	}

	//�ձ濡 ������ Ȯ�������� ���ظ� �޴´�.
	// �ܺο��� ȣ���Ͽ� �Է��� ���� �ð� ���� ���� �Լ�
	public void Interrupted(float duration)
	{
		StartCoroutine(DisableInputCoroutine(duration));
	}

	private IEnumerator DisableInputCoroutine(float duration) // coroutine���� �����ð� �ӹ�
	{
		yield return new WaitForSeconds(duration);
		disabled = false;
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
			gameManager.ResetInteractionGauge();
		}
	}

	void FindRightClick() // �峭�� ���� �Ǻ� �� �峭���� ������ ���·� ����
	{
		RaycastHit hit;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit))
		{
			if (hit.transform != null && hit.transform.tag == "Toy")
			{
				selected = hit.transform.gameObject; // Ŭ���� ������Ʈ ����
				//Debug.Log("Selected Object: " + selected.name); // ������
			}
		}
	}

	void CancelToyCTRL() // �峭�� ���� ����(�ӽ�...)
	{
		if (selected != null)
		{
			selected = null;
		}
	}
}
