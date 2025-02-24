using UnityEngine;

public class NCControl : MonoBehaviour
{
	private Material material;
	private float originOutLineWidth;
	private float highlightOutLineWidth = 0.03f;
    public GameObject obj;
	//washing 과 trash 의 위치는 병합되고서 실제 washing과 trash의 위치에 맞춰 변동되어야 함
	private Vector3 washing = new Vector3(7, 0, 0);
	private Vector3 trash = new Vector3(3, 0, -4);

	void Start()
	{
		// 오브젝트의 Material 복제하기
		material = new Material(GetComponent<Renderer>().material);
		GetComponent<Renderer>().material = material;
		originOutLineWidth = material.GetFloat("_OutlineWidth");
	}
	private void Update()
	{
		DestroyNC();
		//정리가 잘된 경우: 오브젝트 제거
		if (obj.transform.position == washing)
		{
			//오브젝트 제거 및 obj null로 변경
			Destroy(gameObject);
			obj = null;
			return;
		}
		//정리가 잘못된 경우 - 비어있는 임의 위치로 재배치(미구현). 제한시간 5초 감소를 위한 매개인자 전달(미구현)
		if (obj.transform.position == trash)
		{
			Destroy(gameObject);
			obj = null;
			return;
		}
	}

	void OnMouseOver()
	{
		material.SetFloat("_OutlineWidth", highlightOutLineWidth);
		Debug.Log("마우스 감지. 현재 테두리 두께: " + material.GetFloat("_OutlineWidth"));
	}

	void OnMouseExit()
	{
		material.SetFloat("_OutlineWidth", originOutLineWidth);
		Debug.Log("마우스 감지 종료. 현재 테두리 두께: " + material.GetFloat("_OutlineWidth"));
	}

	void DestroyNC()
	{
		if (Input.GetKeyDown("x"))
		{
			Destroy(gameObject);
			obj = null;
		}
	}
}
