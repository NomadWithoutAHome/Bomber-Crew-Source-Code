using BomberCrewCommon;
using UnityEngine;

public class ProjectileEffectRocketHomingPlayer : ProjectileEffectBase
{
	[SerializeField]
	private float m_accelerationTime;

	[SerializeField]
	private float m_axisOffset = 1f;

	[SerializeField]
	private Transform m_axisTransform;

	[SerializeField]
	private Transform m_offsetTransform;

	[SerializeField]
	private Rigidbody m_rigidBody;

	[SerializeField]
	private GameObject m_explosionPrefab;

	[SerializeField]
	private ExternalDamageExplicit m_damager;

	[SerializeField]
	private GameObject m_smokeTrailObject;

	[SerializeField]
	private MonoBehaviour m_effectDestroyScript;

	[SerializeField]
	private Collider m_collider;

	[SerializeField]
	private float m_colliderEnableDelay = 0.5f;

	[SerializeField]
	private float m_startSpeedMultiplier = 0.1f;

	[SerializeField]
	private float m_maxRotationPerSecond = 90f;

	private Vector3 m_direction;

	private Vector3 m_inheritedVelocity;

	private float m_currentSpeedMultiplier;

	private float m_muzzleVelocity;

	private bool m_destroyed;

	private bool m_shouldDestroy;

	private float m_timeAlive;

	private Vector3 m_offsetLocalPos = Vector3.zero;

	private FighterPlane m_target;

	public override void TriggerEffect(Vector3 startPos, Vector3 endPos, float muzzleVelocity, float damage, LayerMask targetLayermask, Vector3 inheritedVelocity, object relatedObject, ProjectileType projectileType)
	{
		base.transform.position = startPos;
		base.gameObject.btransform().SetFromCurrentPage(base.transform.position);
		base.transform.LookAt(endPos);
		m_rigidBody.velocity = inheritedVelocity;
		m_direction = (endPos - startPos).normalized;
		m_muzzleVelocity = muzzleVelocity;
		m_inheritedVelocity = inheritedVelocity;
		m_currentSpeedMultiplier = m_startSpeedMultiplier;
		m_timeAlive = 0f;
	}

	public void SetTarget(FighterPlane fp)
	{
		m_target = fp;
	}

	private void Update()
	{
		m_currentSpeedMultiplier += Time.deltaTime / m_accelerationTime;
		if (m_currentSpeedMultiplier > 1f)
		{
			m_currentSpeedMultiplier = 1f;
		}
		Vector3 velocity = m_inheritedVelocity * (1f - m_currentSpeedMultiplier) + m_direction * m_muzzleVelocity * m_currentSpeedMultiplier;
		m_rigidBody.velocity = velocity;
		m_offsetLocalPos.y = m_axisOffset * m_currentSpeedMultiplier;
		m_offsetTransform.localPosition = m_offsetLocalPos;
		if (m_target == null)
		{
			bool foundFighter = false;
			m_target = Singleton<FighterCoordinator>.Instance.GetCurrentTarget(m_target, 50f, base.transform, 120f, out foundFighter);
		}
		if (m_target == null)
		{
			m_target = Singleton<FighterCoordinator>.Instance.GetCurrentTargetIgnoreTagged(m_target, 50f, base.transform, 120f, 3500f);
		}
		if (m_target != null && m_timeAlive > 1f)
		{
			Vector3 forward = m_target.transform.position - base.transform.position;
			Quaternion to = Quaternion.LookRotation(forward, base.transform.up);
			Quaternion rotation = Quaternion.RotateTowards(base.transform.rotation, to, m_maxRotationPerSecond * Time.deltaTime);
			base.transform.rotation = rotation;
			m_rigidBody.rotation = rotation;
			m_direction = base.transform.forward;
		}
		m_timeAlive += Time.deltaTime;
		if (m_timeAlive > 30f)
		{
			DoDestroy();
		}
		if (m_shouldDestroy && !m_destroyed)
		{
			DoDestroy();
		}
	}

	private void DoDestroy()
	{
		if (!m_destroyed)
		{
			m_destroyed = true;
			GameObject fromPoolSlow = Singleton<PoolManager>.Instance.GetFromPoolSlow(m_explosionPrefab);
			fromPoolSlow.transform.position = base.transform.position;
			fromPoolSlow.btransform().SetFromCurrentPage(base.transform.position);
			m_damager.DoDamage();
			m_effectDestroyScript.enabled = true;
			m_smokeTrailObject.transform.parent = null;
			Object.Destroy(base.gameObject);
		}
	}

	public void DestroyEarly()
	{
		m_shouldDestroy = true;
	}

	private void OnCollisionEnter(Collision collision)
	{
		DoDestroy();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<WeatherArea>() == null)
		{
			DoDestroy();
		}
	}
}
