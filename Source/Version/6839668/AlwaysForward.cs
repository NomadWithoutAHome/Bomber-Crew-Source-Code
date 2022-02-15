using UnityEngine;

public class AlwaysForward : MonoBehaviour
{
	public float Speed;

	public float yRotation;

	private void Update()
	{
		base.transform.position = base.transform.position + base.transform.forward * Speed;
		base.transform.Rotate(Vector3.up, yRotation);
	}
}
