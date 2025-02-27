using UnityEngine;

public class QuadOutlineControl : MonoBehaviour {

	private Material material;
	private float originOutlineWidth;
	public float highlightOutlineWidth = 1f;

	void Start()
	{
		Renderer rend = GetComponent<Renderer>();
		material = new Material(rend.material);
		rend.material = material;
		// ���� ���, 3000�� �̻��� ������
		material.renderQueue = 3100;

		originOutlineWidth = material.GetFloat("_OutlineWidth");
	}


	void Update()
	{
		CheckAndDestroy();
	}

	void OnMouseOver()
	{
		material.SetFloat("_OutlineWidth", highlightOutlineWidth);
		Debug.Log("���콺 ����");
		// �߰�: Outline �÷���, �ٸ� ȿ��(��: Emission)�� �Բ� �����ϸ� �� ������ ȿ���� �� �� ����.
	}

	void OnMouseExit()
	{
		Debug.Log("���콺 �ƿ�");
		material.SetFloat("_OutlineWidth", originOutlineWidth);
	}

	void CheckAndDestroy()
	{
		if (Input.GetKeyDown("x"))
		{
			Destroy(gameObject);
		}
	}
}
