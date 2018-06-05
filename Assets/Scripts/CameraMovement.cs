using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	public float moveSpeed = 1.0f;
	public float turnSpeed = 1.0f;
	public float zoomSpeed = 1.0f;
	public Camera cam;
	private float originalFOV;
	private float x,y,z,v,h,u;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");
		x = Input.GetAxis("Mouse X");
		y = Input.GetAxis("Mouse Y");
		z = Input.GetAxis("Mouse ScrollWheel");
		
		u = 0;
		if (Input.GetKey(KeyCode.Space)) u = 1;
		if (Input.GetKey(KeyCode.LeftControl)) u = -1;
		
		cam.fieldOfView -= z * zoomSpeed;
		
		transform.Translate(new Vector3(h,u,v) * moveSpeed);
		transform.eulerAngles = new Vector3(transform.eulerAngles.x-y*turnSpeed,transform.eulerAngles.y+x*turnSpeed,0);
	}
}
