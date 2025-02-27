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
		// 예를 들어, 3000번 이상의 값으로
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
		Debug.Log("마우스 오버");
		// 추가: Outline 컬러나, 다른 효과(예: Emission)도 함께 조절하면 더 빛나는 효과를 줄 수 있음.
	}

	void OnMouseExit()
	{
		Debug.Log("마우스 아웃");
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
