using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	private float speed = 5.0f;
	private float zoomSpeed = 2.0f;

    public float smoothing = 200.0f;

	public float sensX = 100;
	public float sensY = 100;
	
	float rotationY = 0.0f;
	float rotationX = 0.0f;

	void Update () {

		float scroll = Input.GetAxis("Mouse ScrollWheel");
		transform.Translate(0, scroll * zoomSpeed, scroll * zoomSpeed, Space.World);

		if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + transform.right * speed * Time.deltaTime, smoothing * Time.deltaTime);
            //transform.position += transform.right * speed * Time.deltaTime;
        }
		if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position - transform.right * speed * Time.deltaTime, smoothing * Time.deltaTime);
            //transform.position += -transform.right * speed * Time.deltaTime;
		}
		if (Input.GetAxisRaw("Vertical") > 0)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * speed * Time.deltaTime, smoothing * Time.deltaTime);
            //transform.position += transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetAxisRaw("Vertical") < 0)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position - transform.forward * speed * Time.deltaTime, smoothing * Time.deltaTime);
            //transform.position += -transform.forward * speed * Time.deltaTime;
		}

		if (Input.GetMouseButton (1)) {
            rotationX = Mathf.Lerp(rotationX, rotationX + Input.GetAxis ("Mouse X") * sensX * Time.deltaTime, smoothing * Time.deltaTime);
            rotationY = Mathf.Lerp(rotationY, rotationY + Input.GetAxis ("Mouse Y") * sensY * Time.deltaTime, smoothing * Time.deltaTime);
            
            

            //rotationY = Mathf.Clamp(rotationY, minY, maxY);
            //rotationX = Mathf.Clamp(rotationX, minX, maxX);
            transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
		}
	}
}
