using UnityEngine;

public class BonusRotator : MonoBehaviour
{
    [SerializeField] private float m_RotationSpeed = 30f;

    private Vector3 rotationVector;

    void Start()
    {
        rotationVector = new Vector3(0, 0, m_RotationSpeed);
    }

    void Update()
    {
        transform.Rotate(rotationVector * Time.deltaTime);
    }
}
