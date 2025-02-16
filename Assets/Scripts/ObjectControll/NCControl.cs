using UnityEngine;

public class NCControl : MonoBehaviour
{
    public GameObject obj;
	//washing 과 trash 의 위치는 병합되고서 실제 washing과 trash의 위치에 맞춰 변동되어야 함
	private Vector3 washing = new Vector3(7, 0, 0);
	private Vector3 trash = new Vector3(3, 0, -4);
	private void Update()
	{
		//정리가 잘된 경우 - 이때는 어떤 이득이 있는건지?(미구현) + 오브젝트 제거
		if (obj.transform.position == washing)
		{
			//???
			//오브젝트 제거 및 obj null로 변경
			Destroy(gameObject);
			obj = null;
			return;
		}
		//정리가 잘못된 경우 - 비어있는 임의 위치로 재배치(미구현). 제한시간 5초 감소를 위한 매개인자 전달(미구현)
		if (obj.transform.position == trash)
		{

		}
	}

	private void OnMouseOver()
	{
		//마우스가 위에 올라가있는 경우 테두리가 빛남(미구현)
        Debug.Log("마우스 감지.");
	}
	private void OnMouseExit()
	{
		//마우스가 위에서 내려왔을 경우 테두리가 빛나는게 꺼짐.(미구현)
		Debug.Log("마우스 감지 종료");
	}
}
