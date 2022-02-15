using UnityEngine;

public class CustomGravity : MonoBehaviour
{
	[SerializeField]
	private float m_gravity = 9.81f;

	[SerializeField]
	private bool m_enabled = true;

	private Vector3 m_forceVector;

	private Rigidbody[] m_rigidbodies;

	private void Awake()
	{
		m_rigidbodies = base.gameObject.GetComponentsInChildren<Rigidbody>();
		m_forceVector = new Vector3(0f, 0f - m_gravity, 0f);
	}

	private void FixedUpdate()
	{
		if (m_enabled)
		{
			Rigidbody[] rigidbodies = m_rigidbodies;
			foreach (Rigidbody rigidbody in rigidbodies)
			{
				rigidbody.AddForce(m_forceVector, ForceMode.Acceleration);
			}
		}
	}

	public void SetEnabled(bool enabled)
	{
		m_enabled = enabled;
	}
}
