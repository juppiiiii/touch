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
				Debug.Log("�巡�� ����");
			}
		}

		// ���õ� ������Ʈ�� ���� ��
		if (selected != null)
		{
			// �巡�� �̵� ó��
			if (isDragging)
			{
				// ���� ���콺 ��ġ
				Vector3 currentMousePosition = Input.mousePosition;

				// ���콺 �̵��� ���
				Vector3 deltaMousePosition = currentMousePosition - lastMousePosition;

				// ȭ�� ��ǥ�迡�� ���� ��ǥ��� ��ȯ
				Vector3 worldDelta = ScreenToWorldDelta(deltaMousePosition);

				// ������Ʈ ��ġ ������Ʈ
				selected.transform.position += worldDelta;

				// ���콺 ���� ��ġ ������Ʈ
				lastMousePosition = currentMousePosition;
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
		}
	}

	// ȭ�� ��ǥ���� �̵����� ���� ��ǥ���� �̵������� ��ȯ�ϴ� �Լ�
	Vector3 ScreenToWorldDelta(Vector3 screenDelta)
	{
		// ������Ʈ�� ���� ��ġ�� ȭ�� ��ǥ�� ��ȯ
		Vector3 screenPosition = mainCamera.WorldToScreenPoint(selected.transform.position);

		// ���ο� ȭ�� ��ǥ ���
		Vector3 newScreenPosition = screenPosition + screenDelta;

		// ���ο� ȭ�� ��ǥ�� ���� ��ǥ�� ��ȯ
		Vector3 newWorldPosition = mainCamera.ScreenToWorldPoint(newScreenPosition);

		// ���� ��ǥ�迡���� �̵��� ���
		Vector3 worldDelta = newWorldPosition - selected.transform.position;

		// y�� �̵��� ���ְ�, z�� �̵����� ��ü (���콺 y �̵��� z�� �̵����� ����)
		worldDelta.y = 0f;

		return worldDelta;
	}
}
