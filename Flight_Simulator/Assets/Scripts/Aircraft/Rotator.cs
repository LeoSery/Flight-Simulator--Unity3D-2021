using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float Speed;

    void Update()
    {
        transform.localRotation *= Quaternion.AngleAxis(Speed * Time.deltaTime, Vector3.up);
    }
}
