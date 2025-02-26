using UnityEngine;

public class FOControl : MonoBehaviour {
	private Material material;

	void Start()
	{
		// ������Ʈ�� Material �����ϱ� 
		Renderer rend = GetComponent<Renderer>();
		material = new Material(rend.material);
		rend.material = material;

		// �� ������Ʈ ������ ���� �θ�� ����ĳ��Ʈ ����
		gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

		// ���� �� ������ ��� ����
		SetOpaqueMode();
		Color initColor = material.color;
		initColor.a = 1f;
		material.color = initColor;
	}

	// �ڽ��� FOHoverDetector�� ȣ���� �޼���
	public void HandleMouseOver()
	{
		SetTransparentMode();
		Color color = material.color;
		color.a = 0.3f; // ������
		material.color = color;
		//Debug.Log("���콺 ����: ���� ��� Ȱ��ȭ, ���� ����: " + material.color.a);
	}

	public void HandleMouseExit()
	{
		// ���콺�� ����� ������ ��� ����
		Color color = material.color;
		color.a = 1f;
		material.color = color;
		SetOpaqueMode();
		//Debug.Log("���콺 ����: ������ ��� ����, ���� ����: " + material.color.a);
	}

	// ���� ȿ�� ����
	void SetTransparentMode()
	{
		material.SetFloat("_Mode", 3);
		material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		material.SetInt("_ZWrite", 0);
		material.DisableKeyword("_ALPHATEST_ON");
		material.EnableKeyword("_ALPHABLEND_ON");
		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = 3000;
	}

	// ������ ȿ�� ����
	void SetOpaqueMode()
	{
		material.SetFloat("_Mode", 0);
		material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
		material.SetInt("_ZWrite", 1);
		material.DisableKeyword("_ALPHATEST_ON");
		material.DisableKeyword("_ALPHABLEND_ON");
		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = -1;
	}
}