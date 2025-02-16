using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject selected;

	private void Start()
	{
	}
	private void Update()
	{
		if (Input.GetMouseButtonDown(0)) // ���콺 ��Ŭ�� �����ϸ� GameObject ���� selected�� ��Ŭ���� GameObject�� �Ҵ��.
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit))
			{
				if (hit.transform != null)
				{
					selected = hit.transform.gameObject; // Ŭ���� ���ӿ�����Ʈ�� ������ �Ҵ�
					Debug.Log("Selected Object: " + selected.name);
				}
			}
		}
		//null�� �ƴ� ��쿡�� �̵� ����.
		if (selected != null)
		{
			//wasd�̵�. ĭ ������ ��������(�̱���), ������ �Ѿ ������ �� ����� �ϸ�(�̱���), s�� d�����ӿ� ���ؼ��� trash�� washing���� �Ѿ �� �־�� �Ѵ�(�̱���).
			if (Input.GetKeyDown("w"))
			{
				Vector3 locate = selected.transform.position;
				selected.transform.position = new Vector3(locate.x, locate.y, locate.z+1);
			}
			if (Input.GetKeyDown("a"))
			{
				Vector3 locate = selected.transform.position;
				selected.transform.position = new Vector3(locate.x-1, locate.y, locate.z);
			}
			if (Input.GetKeyDown("s"))
			{
				Vector3 locate = selected.transform.position;
				selected.transform.position = new Vector3(locate.x, locate.y, locate.z-1);
			}
			if (Input.GetKeyDown("d"))
			{
				Vector3 locate = selected.transform.position;
				selected.transform.position = new Vector3(locate.x+1, locate.y, locate.z);
			}
		}
	}
}
