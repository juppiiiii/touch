using UnityEngine;

public class NCControl : MonoBehaviour
{
	public ObjectManager objectManager;
	public GameObject manager;
	private Material material;
	private float originOutLineWidth;
	private float highlightOutLineWidth = 0.03f;
    public GameObject obj;
	//washing 과 trash 의 위치는 병합되고서 실제 washing과 trash의 위치에 맞춰 변동되어야 함. ObjectManager에도 동일한 코드가 있으므로 수정이 필요하면 그쪽도 수정을 해주어야함.
	public float washMinX = 0;
	public float washMaxX = 10;
	public float washMinZ = -20;
	public float washMaxZ = -10;
	public float trashMinX = 10;
	public float trashMaxX = 20;
	public float trashMinZ = -10;
	public float trashMaxZ = 0;

	void Start()
	{
		// 오브젝트의 Material 복제하기
		material = new Material(GetComponent<Renderer>().material);
		GetComponent<Renderer>().material = material;
		originOutLineWidth = material.GetFloat("_OutlineWidth");
		manager = GameObject.Find("ObjectManager");
		objectManager = manager.GetComponent<ObjectManager>();
	}
	private void Update()
	{
		DestroyNC();
		Vector3 pos = obj.transform.position;
		if (objectManager.selectedTag == null)
		{
			//정리가 잘된 경우: 오브젝트 제거(NC기준)
			if (pos.x <= washMaxX && pos.x >= washMinX && pos.z <= washMaxZ && pos.z >= washMinZ)
			{
				//오브젝트 제거 및 obj null로 변경
				Destroy(gameObject);
				obj = null;
				return;
			}
			//정리가 잘못된 경우 - 오브젝트 제거(사실 하는 일 똑같은데 보기 어지러워서, 혹시 다른 기능이 생길 수 있으므로 따로 분리)
			if (pos.x <= trashMaxX && pos.x >= trashMinX && pos.z <= trashMaxZ && pos.z >= trashMinZ)
			{
				Destroy(gameObject);
				obj = null;
				return;
			}
		}
	}

	void OnMouseOver()
	{
		material.SetFloat("_OutlineWidth", highlightOutLineWidth);
		//Debug.Log("마우스 감지. 현재 테두리 두께: " + material.GetFloat("_OutlineWidth"));
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
