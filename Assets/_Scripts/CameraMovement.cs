using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	private float speed = 5.0f;
	private float zoomSpeed = 2.0f;

	/*public float minX = -1000.0f;
	public float maxX = 1000.0f;*/
	
	public float minY = -100.0f;
	public float maxY = 100.0f;

	public float sensX = 60;
	public float sensY = 60;
	
	float rotationY = 0.0f;
	float rotationX = 0.0f;

	void Update () {

		float scroll = Input.GetAxis("Mouse ScrollWheel");
		transform.Translate(0, scroll * zoomSpeed, scroll * zoomSpeed, Space.World);

		if (Input.GetAxisRaw("Horizontal") > 0)
        {
			transform.position += transform.right * speed * Time.deltaTime;
		}
		if (Input.GetAxisRaw("Horizontal") < 0)
        {
			transform.position += -transform.right * speed * Time.deltaTime;
		}
		if (Input.GetAxisRaw("Vertical") > 0)
        {
			transform.position += transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetAxisRaw("Vertical") < 0)
        {
			transform.position += -transform.forward * speed * Time.deltaTime;
		}

		if (Input.GetMouseButton (1)) {
			rotationX += Input.GetAxis ("Mouse X") * sensX * Time.deltaTime;
			rotationY += Input.GetAxis ("Mouse Y") * sensY * Time.deltaTime;
			rotationY = Mathf.Clamp (rotationY, minY, maxY);
			transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
		}
	}
}
