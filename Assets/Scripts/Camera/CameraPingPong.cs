using UnityEngine;

public class CameraPingPong : MonoBehaviour
{
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;
    [SerializeField] private float _speed = 2f;

    private bool _isGoingToB = true;

    private void Update()
    {
        Transform target = _isGoingToB ? _pointB : _pointA;

        transform.position = Vector3.MoveTowards(transform.position, target.position, _speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            _isGoingToB = !_isGoingToB;
        }
    }
}