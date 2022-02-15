using UnityEngine;

public class CrewmanBailedOutController : MonoBehaviour
{
	[SerializeField]
	private float m_airResistanceParachuteUp;

	[SerializeField]
	private float m_airResistanceParachuteGeneral;

	[SerializeField]
	private GameObject m_parachuteHierarchy;

	[SerializeField]
	private GameObject m_hasParachuteHierarchy;

	[SerializeField]
	private float m_timeToActivate = 1.5f;

	private CrewmanAvatar m_crewmanToControl;

	private float m_timeUntilParachute;

	private bool m_hasLanded;

	private bool m_hasParachute;

	private Vector3 m_startingVelocity;

	public void SetUp(CrewmanAvatar ca, Vector3 velocity)
	{
		m_startingVelocity = velocity;
		m_crewmanToControl = ca;
		m_timeUntilParachute = m_timeToActivate;
		GetComponent<Rigidbody>().velocity = velocity;
		m_hasParachute = m_crewmanToControl.GetCarriedItem() != null && m_crewmanToControl.GetCarriedItem().GetComponent<Parachute>() != null;
		m_parachuteHierarchy.SetActive(value: false);
		m_hasParachuteHierarchy.SetActive(m_hasParachute);
	}

	private void Start()
	{
		GetComponent<Rigidbody>().velocity = m_startingVelocity;
	}

	private void FixedUpdate()
	{
		if (!m_hasLanded && m_hasParachute && !m_crewmanToControl.GetCrewman().IsDead() && !m_crewmanToControl.GetHealthState().IsCountingDown())
		{
			m_timeUntilParachute -= Time.deltaTime;
			if (m_timeUntilParachute < 0f)
			{
				m_parachuteHierarchy.SetActive(value: true);
				GetComponent<Rigidbody>().velocity += base.transform.up * m_airResistanceParachuteUp * Mathf.Clamp01(Vector3.Dot(-GetComponent<Rigidbody>().velocity, base.transform.up) * Time.deltaTime);
				GetComponent<Rigidbody>().velocity += -GetComponent<Rigidbody>().velocity * Mathf.Clamp01(Time.deltaTime * m_airResistanceParachuteGeneral);
			}
		}
	}

	public bool HasParachute()
	{
		return m_hasParachute;
	}

	public bool HasLanded()
	{
		return m_hasLanded;
	}

	private void OnCollisionEnter(Collision c)
	{
		m_hasLanded = true;
		if (Mathf.Abs(c.relativeVelocity.y) > 15f)
		{
			DamageSource damageSource = new DamageSource();
			damageSource.m_damageShapeEffect = DamageSource.DamageShape.None;
			damageSource.m_damageType = DamageSource.DamageType.GroundImpact;
			damageSource.m_position = m_crewmanToControl.transform.position;
			m_crewmanToControl.DamageGetPassthrough((Mathf.Abs(c.relativeVelocity.y) - 15f) * 25f, damageSource);
		}
	}
}
