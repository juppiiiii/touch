using UnityEngine;

public class FOHoverDetect : MonoBehaviour {
	// �θ� ��ũ��Ʈ ���� (Inspector���� �Ҵ��ϰų� Start���� �ڵ����� ã��)
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