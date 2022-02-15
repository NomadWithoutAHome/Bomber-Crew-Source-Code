using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class FighterDynamics : MonoBehaviour
{
	[SerializeField]
	private float m_maxSpeed;

	[SerializeField]
	private float m_minSpeed;

	[SerializeField]
	private float m_acceleration;

	[SerializeField]
	private float m_gravityAcceleration;

	[SerializeField]
	private float m_maxTurnRate;

	[SerializeField]
	private float m_perpendicularityFactor;

	[SerializeField]
	private FighterPlane m_fighterPlane;

	[SerializeField]
	private Transform m_lookAtRotationNode;

	[SerializeField]
	private float m_maxAltitude = 1000f;

	[SerializeField]
	private Transform m_bankNode;

	[SerializeField]
	private float m_bankFactor;

	[SerializeField]
	private float m_bankRate;

	[SerializeField]
	private bool m_noAvoid;

	[SerializeField]
	private float m_maxCatchUp = 1f;

	[SerializeField]
	private string m_audioEventStart;

	[SerializeField]
	private string m_audioEventEnd;

	private Vector3 m_targetPosition;

	private Vector3 m_targetHeading;

	private Vector3 m_debugPosition;

	private float m_targetVelocity;

	private bool m_hasTargetPosition;

	private float m_currentBank;

	private float m_catchUpHackAmount;

	private bool m_isFrozen;

	protected bool m_isDestroyed;

	private float m_actualVelocity;

	private float m_actualCatchUp;

	private float m_maxDeathSpinSpeed = 250f;

	private float m_deathSpinSpeed;

	private bool m_audioIsPlaying;

	private void Start()
	{
		m_isFrozen = true;
		m_deathSpinSpeed = Random.Range(0f - m_maxDeathSpinSpeed, m_maxDeathSpinSpeed);
	}

	public void SetTargetPositionAndHeading(Vector3 pos, Vector3 heading, float velocity, float catchUpHack)
	{
		m_hasTargetPosition = true;
		m_targetPosition = pos;
		if (m_targetPosition.y < 130f)
		{
			m_targetPosition.y = 130f;
		}
		if (m_targetPosition.y > m_maxAltitude)
		{
			m_targetPosition.y = m_maxAltitude;
		}
		m_targetHeading = heading;
		m_targetVelocity = velocity;
		m_catchUpHackAmount = catchUpHack;
		m_isFrozen = false;
	}

	public void SetHeading(Vector3 heading, float velocity, float catchUpHack)
	{
		m_hasTargetPosition = false;
		m_targetHeading = heading;
		if (base.transform.position.y + m_targetHeading.y * 40f < 100f && m_targetHeading.y < 0f)
		{
			m_targetHeading.y = 0f;
		}
		if (base.transform.position.y + m_targetHeading.y * 40f > m_maxAltitude && m_targetHeading.y > 0f)
		{
			m_targetHeading.y = 0f;
		}
		m_targetVelocity = velocity;
		m_catchUpHackAmount = catchUpHack;
		m_isFrozen = false;
	}

	public void SetFrozen()
	{
		m_isFrozen = true;
	}

	public void SetDestroyed()
	{
		m_isDestroyed = true;
	}

	public Vector3 GetCurrentHeading()
	{
		return m_lookAtRotationNode.forward;
	}

	public virtual Vector3 GetVelocity()
	{
		if (m_isFrozen)
		{
			return Vector3.zero;
		}
		Vector3 velocity = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetPhysicsModel()
			.GetVelocity();
		return GetCurrentHeading() * Mathf.Lerp(m_minSpeed, m_maxSpeed, m_actualVelocity) + m_actualCatchUp * velocity * m_maxCatchUp;
	}

	public void SetExactVelocity(float exV)
	{
		float value = (exV - m_minSpeed) / (m_maxSpeed - m_minSpeed);
		m_targetVelocity = Mathf.Clamp01(value);
	}

	public float GetMaxVelocity()
	{
		return m_maxSpeed;
	}

	public Vector3 GetCurrentAimingDirection()
	{
		return -base.transform.right;
	}

	private void OnDestroy()
	{
		if (m_audioIsPlaying && !string.IsNullOrEmpty(m_audioEventEnd) && WingroveRoot.Instance != null)
		{
			WingroveRoot.Instance.PostEventGO(m_audioEventEnd, base.gameObject);
		}
	}

	public virtual void FixedUpdate()
	{
		bool flag = !m_isFrozen;
		if (flag != m_audioIsPlaying)
		{
			if (flag)
			{
				if (!string.IsNullOrEmpty(m_audioEventStart))
				{
					WingroveRoot.Instance.PostEventGO(m_audioEventStart, base.gameObject);
				}
			}
			else if (!string.IsNullOrEmpty(m_audioEventEnd))
			{
				WingroveRoot.Instance.PostEventGO(m_audioEventEnd, base.gameObject);
			}
			m_audioIsPlaying = flag;
		}
		if (m_isDestroyed)
		{
			m_actualVelocity = Mathf.Lerp(m_actualVelocity, 1f, Mathf.Clamp01(Time.deltaTime * 5f));
			m_actualCatchUp = Mathf.Lerp(m_actualCatchUp, 0f, Mathf.Clamp01(Time.deltaTime * 5f));
			Vector3 currentHeading = GetCurrentHeading();
			Vector3 normalized = new Vector3(currentHeading.x, -2f, currentHeading.z).normalized;
			currentHeading = Vector3.RotateTowards(currentHeading, normalized, m_maxTurnRate * Time.deltaTime, 0f).normalized;
			base.transform.LookAt(base.transform.position + currentHeading, base.transform.up);
			base.gameObject.btransform().position += Time.deltaTime * new Vector3d(GetVelocity());
			base.transform.Rotate(Vector3.forward * (m_deathSpinSpeed * Time.deltaTime));
		}
		else
		{
			if (m_isFrozen)
			{
				return;
			}
			Vector3 vector = GetCurrentHeading();
			Vector3 vector2 = m_targetHeading;
			if (m_hasTargetPosition)
			{
				float num = Vector3.Dot(vector, m_targetHeading) * 0.5f + 0.5f;
				float num2 = 1f - num;
				Vector3 vector3 = Vector3.Cross(m_targetHeading, Vector3.up);
				Vector3 vector4 = m_targetPosition + vector3 * m_perpendicularityFactor * num2;
				Vector3 normalized2 = (vector4 - base.transform.position).normalized;
				vector2 = normalized2;
				m_debugPosition = vector4;
			}
			if (!m_noAvoid)
			{
				Vector3 targPos = base.transform.position + vector.normalized * 40f;
				Vector3 avoidingPosition = Singleton<FighterCoordinator>.Instance.GetAvoidingPosition(m_fighterPlane, base.transform.position, targPos);
				vector = (avoidingPosition - base.transform.position).normalized;
				Vector3 vector5 = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position + Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetPhysicsModel()
					.GetVelocity() * 2f;
				Vector3 vector6 = base.transform.position + vector * GetVelocity().magnitude * 2f;
				if ((vector6 - vector5).magnitude < 20f)
				{
					Vector3 vector7 = avoidingPosition;
					vector7.y = ((!(vector5.y < base.transform.position.y)) ? (vector5.y - 20f) : (vector5.y + 20f));
					vector = (vector7 - base.transform.position).normalized;
				}
			}
			float num3 = Mathf.Atan2(vector.x, 0f - vector.z) * 57.29578f;
			float num4 = Mathf.Atan2(vector2.x, 0f - vector2.z) * 57.29578f;
			float num5 = num4 - num3;
			if (num5 > 180f)
			{
				num5 = num4 - 360f - num3;
			}
			else if (num5 <= -180f)
			{
				num5 = num4 + 360f - num3;
			}
			float num6 = num5 * m_bankFactor;
			float value = num6 - m_currentBank;
			value = Mathf.Clamp(value, (0f - m_bankRate) * Time.deltaTime, m_bankRate * Time.deltaTime);
			m_currentBank += value;
			m_bankNode.localRotation = Quaternion.Euler(new Vector3(0f, 0f, m_currentBank));
			vector = Quaternion.Euler(new Vector3(0f, (0f - m_currentBank) / m_bankFactor * m_maxTurnRate * Time.deltaTime, 0f)) * vector;
			Vector3 normalized3 = new Vector3(vector.x, vector2.y, vector.z).normalized;
			vector = Vector3.RotateTowards(vector, normalized3, m_maxTurnRate * Time.deltaTime, 0f).normalized;
			base.transform.LookAt(base.transform.position + vector, Vector3.up);
			m_actualVelocity = Mathf.Lerp(m_actualVelocity, m_targetVelocity, Mathf.Clamp01(Time.deltaTime * 5f));
			m_actualCatchUp = Mathf.Lerp(m_actualCatchUp, m_catchUpHackAmount, Mathf.Clamp01(Time.deltaTime * 5f));
			base.gameObject.btransform().position += Time.deltaTime * new Vector3d(GetVelocity());
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(base.transform.position, base.transform.position + GetCurrentHeading() * 64f);
		Color color = Gizmos.color;
		if (m_hasTargetPosition)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, m_targetPosition);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position, m_debugPosition);
		}
		else
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(base.transform.position, base.transform.position + m_targetHeading * 128f);
		}
		Gizmos.color = color;
	}
}
