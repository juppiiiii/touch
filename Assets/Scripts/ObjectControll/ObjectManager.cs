using JetBrains.Annotations;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
	public GameObject selected;
	//public Rigidbody selectedRb;
	private bool isDragging = false;
	private Vector3 lastMousePosition;
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
					//selectedRb = hit.rigidbody;
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
				//selectedRb.useGravity = true;
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
			selected.transform.position = pos;
		}
	}
}
