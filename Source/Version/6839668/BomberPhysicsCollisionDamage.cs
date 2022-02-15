using System.Collections;
using BomberCrewCommon;
using UnityEngine;

public class BomberPhysicsCollisionDamage : MonoBehaviour
{
	[SerializeField]
	private float m_impactVelocityBrace;

	[SerializeField]
	private float m_frictionVelocityBrace;

	[SerializeField]
	private float m_frictionMultiplier;

	[SerializeField]
	private float m_impactMultiplier = 1f;

	[SerializeField]
	private bool m_isWheel;

	[SerializeField]
	private Transform m_wheelForwardDirection;

	[SerializeField]
	private float m_networkDamagePassOnImpact;

	[SerializeField]
	private float m_networkDamagePassOnFriction;

	[SerializeField]
	private BomberPhysicsCollisionDamage[] m_directConnections;

	[SerializeField]
	private float m_radiusMultiplier = 1f;

	[SerializeField]
	private float m_wheelFrictionToImpactTolerance = 0.05f;

	[SerializeField]
	private float m_wheelFrictionToImpactToleranceMultiplier = 0.25f;

	private Collider m_collider;

	private float m_totalDamage;

	private bool m_isEnabled;

	private void Awake()
	{
		m_collider = GetComponent<Collider>();
	}

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(2f);
		m_isEnabled = true;
	}

	public void DoDamage(Vector3 relativeVelocity, Vector3 normal, Vector3 atPoint, float damagePerUnit, GroundCollisionType.GroundCollisionEffectType gct)
	{
		if (!m_isEnabled)
		{
			return;
		}
		Vector3 vector = normal * Vector3.Dot(relativeVelocity, normal);
		Vector3 lhs = relativeVelocity - vector;
		if (m_isWheel)
		{
			Vector3 forward = m_wheelForwardDirection.forward;
			lhs -= forward * Vector3.Dot(lhs, forward);
			float num = vector.magnitude / lhs.magnitude;
			if (num > m_wheelFrictionToImpactTolerance)
			{
				vector += Mathf.InverseLerp(m_wheelFrictionToImpactTolerance, 1f, num) * m_wheelFrictionToImpactToleranceMultiplier * vector.normalized;
			}
		}
		float num2 = Mathf.Max(vector.magnitude - m_impactVelocityBrace, 0f);
		float num3 = Mathf.Max(lhs.magnitude - m_frictionVelocityBrace, 0f);
		float num4 = (num2 * m_impactMultiplier + num3 * m_frictionMultiplier) * damagePerUnit;
		if (num4 > 1f)
		{
			DamageUtils.DoDamageSlowWithBlock(atPoint, 0f, num4 * 0.75f, 6f * m_radiusMultiplier, DamageSource.DamageType.GroundImpact);
			DamageUtils.DoDamageSphereFastEverything(atPoint, 0f, num4 * 0.25f, 6f * m_radiusMultiplier, DamageSource.DamageType.GroundImpact);
			m_totalDamage += num4;
			if (m_totalDamage > 1000f)
			{
				Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetPhysicsModel()
					.SetSimplifiedModel();
			}
			if (m_totalDamage > 100f)
			{
				Singleton<ContextControl>.Instance.SetTargetingAllowed(allowed: false);
				Singleton<BomberCamera>.Instance.SetHasCrashed();
			}
			float amt = (num2 * m_impactMultiplier * m_networkDamagePassOnImpact + num3 * m_networkDamagePassOnFriction * m_frictionMultiplier) * damagePerUnit;
			BomberPhysicsCollisionDamage[] directConnections = m_directConnections;
			foreach (BomberPhysicsCollisionDamage bomberPhysicsCollisionDamage in directConnections)
			{
				if (bomberPhysicsCollisionDamage != null)
				{
					bomberPhysicsCollisionDamage.NetworkDamage(atPoint, amt);
				}
			}
			if (Singleton<GlobalEffects>.Instance != null)
			{
				Singleton<GlobalEffects>.Instance.SpawnImpactEffects(gct, atPoint);
			}
		}
		else if (m_totalDamage > 1000f && !m_isWheel && Singleton<GlobalEffects>.Instance != null)
		{
			Singleton<GlobalEffects>.Instance.SpawnImpactEffects(gct, atPoint);
		}
	}

	public void NetworkDamage(Vector3 fromPoint, float amt)
	{
		if (amt > 1f)
		{
			Vector3 pos = m_collider.ClosestPointOnBounds(fromPoint);
			DamageUtils.DoDamageSlowWithBlock(pos, 0f, amt, 4f, DamageSource.DamageType.GroundImpact);
		}
	}
}
