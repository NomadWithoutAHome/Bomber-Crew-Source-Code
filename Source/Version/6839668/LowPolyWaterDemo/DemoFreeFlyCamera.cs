using UnityEngine;

namespace LowPolyWaterDemo;

public class DemoFreeFlyCamera : MonoBehaviour
{
	public bool lockCursor;

	public float cameraSensitivity = 4f;

	public float normalMoveSpeed = 10f;

	public float smoothTime = 10f;

	private float rotationX;

	private float rotationY;

	private void Start()
	{
		if (lockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	private void Update()
	{
		float num = Mathf.Clamp(Time.deltaTime, 0f, 0.03f);
		rotationX += Input.GetAxis("Mouse X") * cameraSensitivity;
		rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity;
		rotationY = Mathf.Clamp(rotationY, -90f, 90f);
		if (rotationX > 360f)
		{
			rotationX -= 360f;
		}
		Quaternion b = Quaternion.AngleAxis(rotationX, Vector3.up);
		b *= Quaternion.AngleAxis(rotationY, Vector3.left);
		base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, b, smoothTime * num);
		float num2 = normalMoveSpeed * num;
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			num2 *= 4f;
		}
		Vector3 forward = base.transform.forward;
		Vector3 right = base.transform.right;
		if (Input.GetKey(KeyCode.Space))
		{
			base.transform.position += Vector3.up * num2;
		}
		if (Input.GetKey(KeyCode.E))
		{
			base.transform.position -= Vector3.up * num2;
		}
		base.transform.position += forward * num2 * Input.GetAxis("Vertical");
		base.transform.position += right * num2 * Input.GetAxis("Horizontal");
	}
}
