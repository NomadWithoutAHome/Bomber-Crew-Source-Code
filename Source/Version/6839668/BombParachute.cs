using UnityEngine;

public class BombParachute : MonoBehaviour
{
	[SerializeField]
	private float m_airResistanceParachuteGeneral;

	[SerializeField]
	private GameObject m_parachuteHierarchy;

	[SerializeField]
	private float m_timeToActivate = 1.5f;

	[SerializeField]
	private Rigidbody m_fallingRigidbody;

	private float m_timeUntilParachute;

	private bool m_hasParachute;

	private Vector3 m_startingVelocity;

	public void OnEnable()
	{
		m_timeUntilParachute = m_timeToActivate;
		m_parachuteHierarchy.SetActive(value: false);
	}

	private void FixedUpdate()
	{
		m_timeUntilParachute -= Time.deltaTime;
		if (m_timeUntilParachute < 0f)
		{
			m_parachuteHierarchy.SetActive(value: true);
			m_fallingRigidbody.velocity += -m_fallingRigidbody.velocity * Mathf.Clamp01(Time.deltaTime * m_airResistanceParachuteGeneral);
		}
	}
}
