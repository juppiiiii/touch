using System;

public struct Status {
	public float bindP;
	public int speed;
	public float gaugeSpeed;

	// 생성자 추가 (선택 사항)
	public Status(float bindP, int speed, float gaugeSpeed)
	{
		this.bindP = bindP;
		this.speed = speed;
		this.gaugeSpeed = gaugeSpeed;
	}
}
