using UnityEngine;

public class FOHoverDetect : MonoBehaviour {
	// 부모 스크립트 참조 (Inspector에서 할당하거나 Start에서 자동으로 찾기)
	public FOControl parentControl;

	void OnMouseOver()
	{
		parentControl.HandleMouseOver();
	}

	void OnMouseExit()
	{
		parentControl.HandleMouseExit();
	}
}