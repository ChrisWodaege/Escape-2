using UnityEngine;

public class Presentation : MonoBehaviour
{
    private GameObject _testObject;

    private void Start()
    {
        _testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _testObject.transform.position = Vector3.zero;
    }

    private void Update()
    {
        _testObject.transform.Rotate(Vector3.up);
        _testObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
    }

    // This method will destroy the cube when we delet the component 
    private void OnDestroy()
    {
        if (_testObject != null)
        {
            Destroy(_testObject);
        }
    }
}
