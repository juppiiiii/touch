using UnityEngine;


public class Dice : MonoBehaviour {

	private Rigidbody rb;
	public static Vector3 diceVelocity;


	private int[] angles = {0, 90, 180, 270, 360 };

	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
    
	void Update () {
		diceVelocity = rb.linearVelocity; // 움직임이 있는지 확인하기 위한 변수
	}
    
	public void DiceRoll()
    {
    // 회전 랜덤 변수
		float dirX = Random.Range(0, 3000);
		float dirY = Random.Range(0, 3000);
		float dirZ = Random.Range(0, 3000);
        
		// 초기 회전값 (랜덤 부여)
		Quaternion currentRotation = transform.localRotation;
		float randomIndex_x = Random.Range(0, angles.Length);
		float randomIndex_z = Random.Range(0, angles.Length);
        
        // 초기 위치와 회전 초기화
		transform.localPosition = new Vector3(0, 0, 0);
		
        transform.localRotation = Quaternion.Euler(angles[(int)randomIndex_x], currentRotation.eulerAngles.y, angles[(int)randomIndex_z]);
// 윗 방향으로 힘을 가해 공중에 띄우면서, 랜덤한 방향으로 회전
		float ForceRand = Random.Range(200, 300);
		rb.AddForce(Vector3.up * ForceRand);
		rb.AddTorque(new Vector3(dirX, dirY, dirZ), ForceMode.VelocityChange); // 
	}
}