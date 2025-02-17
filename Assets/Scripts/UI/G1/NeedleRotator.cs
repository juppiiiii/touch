using UnityEngine;

public class NeedleRotator : MonoBehaviour
{
    public float rotationSpeed = 100f; // 회전 속도
    private bool isMoving = true; // 바늘이 움직이는 상태인지 여부

    public void Rotate()
    {
        if (isMoving)
        {
            transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }
    }

    public void StopRotation()
    {
        isMoving = false;
    }

    public void ResumeRotation()
    {
        isMoving = true;
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
