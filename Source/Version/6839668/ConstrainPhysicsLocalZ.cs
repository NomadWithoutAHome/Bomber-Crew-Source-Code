using UnityEngine;

public class ConstrainPhysicsLocalZ : MonoBehaviour
{
	private Rigidbody m_rigidBody;

	private void Start()
	{
		m_rigidBody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		m_rigidBody.velocity -= new Vector3(0f, 0f, m_rigidBody.velocity.z);
		base.transform.localPosition -= new Vector3(0f, 0f, base.transform.localPosition.z);
	}
}
