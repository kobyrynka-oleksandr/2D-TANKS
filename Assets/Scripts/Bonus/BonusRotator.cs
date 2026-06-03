using UnityEngine;

public class BonusRotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 30f;

    private Vector3 rotationVector;

    void Start()
    {
        rotationVector = new Vector3(0, 0, _rotationSpeed);
    }

    void Update()
    {
        transform.Rotate(rotationVector * Time.deltaTime);
    }
}
