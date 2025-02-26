using UnityEngine;

public class NTControl : MonoBehaviour {
	public ObjectManager objectManager;
	public GameObject manager;
	private Material material;
	private float originOutLineWidth;
	private float highlightOutLineWidth = 0.03f;
	public GameObject obj;

	//  �ױ� ������ ������Ʈ���� �ο��Ǵ� �Ӽ�
	public bool ableStack = true;

	/// ��������� ��Ź���� ����
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
		// ������Ʈ�� Material �����ϱ�
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
		if (!objectManager.leftHold)
		{
			//������ �ߵ� ���: ������Ʈ ����(NT����)
			if (pos.x <= trashMaxX && pos.x >= trashMinX && pos.z <= trashMaxZ && pos.z >= trashMinZ)
			{
				Destroy(gameObject);
				obj = null;
				return;
			}
			//������ �߸��� ��� - ������Ʈ ����(��� �ϴ� �� �Ȱ����� ���� ����������, Ȥ�� �ٸ� ����� ���� �� �����Ƿ� ���� �и�)

			if (pos.x <= washMaxX && pos.x >= washMinX && pos.z <= washMaxZ && pos.z >= washMinZ)
			{
				//������Ʈ ���� �� obj null�� ����
				Destroy(gameObject);
				obj = null;
				return;
			}
		}
	}

	void OnMouseOver()
	{
		material.SetFloat("_OutlineWidth", highlightOutLineWidth);
		//Debug.Log("���콺 ����. ���� �׵θ� �β�: " + material.GetFloat("_OutlineWidth"));
	}

	void OnMouseExit()
	{
		material.SetFloat("_OutlineWidth", originOutLineWidth);
		//Debug.Log("���콺 ���� ����. ���� �׵θ� �β�: " + material.GetFloat("_OutlineWidth"));
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
