using UnityEngine;

public class Rotate : MonoBehaviour {

    [SerializeField]
    private float _rotationSpeed = 10f;
    private float _currentAngle = 0f;

	void Update () {
        _currentAngle = _rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, _currentAngle, Space.World);
	}
}