using UnityEngine;

public class FOControl : MonoBehaviour {
	private Material material;

	void Start()
	{
		// 오브젝트의 Material 복제하기 
		Renderer rend = GetComponent<Renderer>();
		material = new Material(rend.material);
		rend.material = material;

		// 뒤 오브젝트 선택을 위해 부모는 레이캐스트 무시
		gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

		// 시작 시 불투명 모드 적용
		SetOpaqueMode();
		Color initColor = material.color;
		initColor.a = 1f;
		material.color = initColor;
	}

	// 자식의 FOHoverDetector가 호출할 메서드
	public void HandleMouseOver()
	{
		SetTransparentMode();
		Color color = material.color;
		color.a = 0.3f; // 반투명도
		material.color = color;
		//Debug.Log("마우스 오버: 투명 모드 활성화, 현재 알파: " + material.color.a);
	}

	public void HandleMouseExit()
	{
		// 마우스가 벗어나면 불투명 모드 복원
		Color color = material.color;
		color.a = 1f;
		material.color = color;
		SetOpaqueMode();
		//Debug.Log("마우스 종료: 불투명 모드 복원, 현재 알파: " + material.color.a);
	}

	// 투명 효과 설정
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

	// 불투명 효과 설정
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