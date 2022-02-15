using UnityEngine;

public class FighterDynamicsNull : FighterDynamics
{
	[SerializeField]
	private float m_maxHeight;

	private float m_t;

	private Vector3 m_lastPosition;

	private Vector3 m_velocity;

	public override Vector3 GetVelocity()
	{
		return m_velocity;
	}

	public override void FixedUpdate()
	{
		Vector3 vector = base.transform.position - m_lastPosition;
		m_velocity = vector / Time.fixedDeltaTime;
		m_lastPosition = base.transform.position;
	}
}
