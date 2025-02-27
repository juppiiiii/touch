using UnityEngine;

public class NTControl : MonoBehaviour {
	public ObjectManager objectManager;
	public GameObject manager;
	public GameObject obj;


	/// 쓰레기통과 세탁기의 영역
	private float washMinX = 0;
	private float washMaxX = 10;
	private float washMinZ = -20;
	private float washMaxZ = -10;
	private float trashMinX = 10;
	private float trashMaxX = 20;
	private float trashMinZ = -10;
	private float trashMaxZ = 0;

	void Start()
	{
		manager = GameObject.Find("ObjectManager");
		objectManager = manager.GetComponent<ObjectManager>();
	}
	private void Update()
	{
		DestroyNC();
		Vector3 pos = obj.transform.position;
		if (!objectManager.leftHold)
		{
			//정리가 잘된 경우: 오브젝트 제거(NT기준)
			if (pos.x <= trashMaxX && pos.x >= trashMinX && pos.z <= trashMaxZ && pos.z >= trashMinZ)
			{
				Destroy(gameObject);
				obj = null;
				return;
			}
			//정리가 잘못된 경우 - 오브젝트 제거(사실 하는 일 똑같은데 보기 어지러워서, 혹시 다른 기능이 생길 수 있으므로 따로 분리)

			if (pos.x <= washMaxX && pos.x >= washMinX && pos.z <= washMaxZ && pos.z >= washMinZ)
			{
				//오브젝트 제거 및 obj null로 변경
				Destroy(gameObject);
				obj = null;
				return;
			}
		}
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
