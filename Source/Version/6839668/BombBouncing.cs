using BomberCrewCommon;
using UnityEngine;

public class BombBouncing : Bomb
{
	[SerializeField]
	private BouncingBombTimingLights m_timingLights;

	[SerializeField]
	private float m_bombForwardsSpeedBoost;

	[SerializeField]
	private float m_bombForwardsSpeedBoostY;

	[SerializeField]
	private int m_maxBounces;

	[SerializeField]
	private GameObject m_bounceSplashEffect;

	private Vector3 m_forward;

	private bool m_releasedWithGoodTiming;

	private int m_numBounces;

	private void Start()
	{
		if (m_rigidBody == null)
		{
			m_rigidBody = base.gameObject.GetComponent<Rigidbody>();
		}
		m_currentHealth = m_health;
		m_groundHitLayerMask = 1 << LayerMask.NameToLayer("Environment");
		if (m_releaseEffect != null)
		{
			m_releaseEffect.SetActive(value: false);
		}
	}

	public override void Release()
	{
		base.Release();
		m_forward = base.transform.forward;
		m_releasedWithGoodTiming = m_timingLights.IsGoodTiming();
		if (!m_releasedWithGoodTiming)
		{
			SetDamageMultiplier(0.25f);
		}
	}

	public new bool IsReleased()
	{
		return m_released;
	}

	private void OnCollisionEnter(Collision c)
	{
		if (!m_released || m_destroyed)
		{
			return;
		}
		BounceWater component = c.collider.GetComponent<BounceWater>();
		if (m_releasedWithGoodTiming && component != null)
		{
			m_numBounces++;
			Vector3 zero = Vector3.zero;
			if (c.contacts.Length > 0)
			{
				GameObject fromPoolSlow = Singleton<PoolManager>.Instance.GetFromPoolSlow(m_bounceSplashEffect);
				fromPoolSlow.transform.position = c.contacts[0].point;
				ContactPoint[] contacts = c.contacts;
				foreach (ContactPoint contactPoint in contacts)
				{
					zero += contactPoint.normal;
				}
				zero /= (float)c.contacts.Length;
				if (Vector3.Dot(zero, Vector3.up) > 0.5f && m_numBounces <= 32)
				{
					Vector3 vector = component.GetCentreTarget().position - base.transform.position;
					vector.y = 0f;
					vector = vector.normalized;
					m_rigidBody.velocity += m_bombForwardsSpeedBoost * vector * 0.25f;
					m_rigidBody.velocity += m_bombForwardsSpeedBoost * m_forward * 0.75f;
					m_rigidBody.angularVelocity = Vector3.zero;
					m_rigidBody.velocity += new Vector3(0f, m_rigidBody.velocity.y * -0.5f + m_bombForwardsSpeedBoostY, 0f);
				}
				else
				{
					SetDamageMultiplier(0.5f);
					CreateCrater();
					Explode();
				}
			}
		}
		else
		{
			if (m_numBounces == 0)
			{
				SetDamageMultiplier(0.25f);
			}
			else
			{
				SetDamageMultiplier(1f);
			}
			CreateCrater();
			Explode();
		}
	}

	private void FixedUpdate()
	{
		if (m_released && base.transform.position.y < 0f && !m_destroyed)
		{
			Explode();
		}
	}

	public override bool IsDamageBlocker()
	{
		return false;
	}

	public override bool DirectDamageOnly()
	{
		return true;
	}
}
