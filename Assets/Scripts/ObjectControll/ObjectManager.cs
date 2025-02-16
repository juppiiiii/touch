using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject selected;

	private void Start()
	{
	}
	private void Update()
	{
		if (Input.GetMouseButtonDown(0)) // 마우스 좌클릭 감지하면 GameObject 변수 selected에 좌클릭한 GameObject가 할당됨.
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit))
			{
				if (hit.transform != null)
				{
					selected = hit.transform.gameObject; // 클릭한 게임오브젝트를 변수에 할당
					Debug.Log("Selected Object: " + selected.name);
				}
			}
		}
		//null이 아닌 경우에만 이동 가능.
		if (selected != null)
		{
			//wasd이동. 칸 밖으로 못나가고(미구현), 물건을 넘어서 움직일 수 없어야 하며(미구현), s와 d움직임에 대해서는 trash와 washing으로 넘어갈 수 있어야 한다(미구현).
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
