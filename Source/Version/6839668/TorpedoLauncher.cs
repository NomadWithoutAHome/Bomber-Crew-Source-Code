using BomberCrewCommon;
using UnityEngine;

public class TorpedoLauncher : MonoBehaviour
{
	[SerializeField]
	private ProjectileType m_projectileType;

	[SerializeField]
	private float m_chargeTime;

	[SerializeField]
	private float m_maxDist;

	[SerializeField]
	private GameObject m_warningPrefab;

	[SerializeField]
	private Submarine m_submarine;

	[SerializeField]
	private float m_prechargeTimeMax;

	[SerializeField]
	private float m_prechargeTimeMin;

	[SerializeField]
	private float m_rotateSpeed;

	[SerializeField]
	private LayerMask m_attackLayer;

	[SerializeField]
	private SmoothDamageable m_damageableToMonitor;

	private float m_currentTimer;

	private bool m_isChargingAttack;

	private GameObject m_warningDisplay;

	private Shootable m_currentTarget;

	private float m_outOfRangeTimer;

	private void Start()
	{
		m_warningDisplay = Object.Instantiate(m_warningPrefab);
		m_warningDisplay.SetActive(value: false);
		m_currentTimer = Random.Range(m_prechargeTimeMin, m_prechargeTimeMax);
		m_warningDisplay.GetComponent<HazardPreviewWarning>().SetTracker(Singleton<TagManager>.Instance.GetUICamera(), base.gameObject, 25f);
	}

	private void OnDisable()
	{
		if (m_warningDisplay != null)
		{
			m_warningDisplay.SetActive(value: false);
		}
	}

	public Shootable GetTarget()
	{
		return m_currentTarget;
	}

	public bool GetIsChargingAttack()
	{
		return m_isChargingAttack;
	}

	private void Update()
	{
		if (m_damageableToMonitor.GetHealthNormalised() == 0f)
		{
			m_currentTarget = null;
		}
		else if (m_currentTarget == null)
		{
			m_currentTarget = Singleton<ShootableCoordinator>.Instance.GetNearestShootable(ShootableType.Defendable, base.transform.position);
			m_outOfRangeTimer = 0f;
		}
		if (m_currentTarget != null)
		{
			Transform centreTransform = m_currentTarget.GetCentreTransform();
			Vector3 aimAheadTarget = AimingUtils.GetAimAheadTarget(base.transform.position, centreTransform.position, m_currentTarget.GetVelocity(), m_projectileType.GetVelocity());
			aimAheadTarget.y = 0f;
			Vector3 vector = aimAheadTarget - base.transform.position;
			vector.y = 0f;
			Quaternion to = Quaternion.LookRotation(vector.normalized, Vector3.up);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, m_rotateSpeed * Time.deltaTime);
			if (!m_isChargingAttack)
			{
				m_warningDisplay.SetActive(value: false);
				if (vector.magnitude < m_maxDist)
				{
					m_outOfRangeTimer = 0f;
					m_currentTimer -= Time.deltaTime;
					if (m_currentTimer < 0f && Vector3.Dot(base.transform.forward, vector.normalized) > 0.95f)
					{
						if (m_submarine != null)
						{
							m_submarine.SetSubmergeBlocked(blocked: true);
						}
						m_isChargingAttack = true;
						m_currentTimer = m_chargeTime;
					}
				}
				else
				{
					m_outOfRangeTimer += Time.deltaTime;
					if (m_outOfRangeTimer > 10f)
					{
						m_currentTarget = null;
					}
				}
				return;
			}
			m_outOfRangeTimer = 0f;
			m_warningDisplay.SetActive(value: true);
			m_warningDisplay.GetComponent<HazardPreviewWarning>().SetCountdown(m_currentTimer / m_chargeTime, m_currentTimer);
			if (m_submarine != null && m_submarine.WantsToLeave())
			{
				m_isChargingAttack = false;
				m_submarine.SetSubmergeBlocked(blocked: false);
				return;
			}
			m_currentTimer -= Time.deltaTime;
			if (m_currentTimer < 0f)
			{
				if (m_submarine != null)
				{
					m_submarine.SetSubmergeBlocked(blocked: false);
				}
				m_isChargingAttack = false;
				Vector3 position = base.transform.position;
				position.y = 0f;
				Singleton<CommonEffectManager>.Instance.ProjectileEffect(m_projectileType, position, aimAheadTarget, 1f, m_attackLayer, Vector3.zero, null, isDouble: false, isQuad: false, isFake: false);
				m_currentTimer = Random.Range(m_prechargeTimeMin, m_prechargeTimeMax);
				m_currentTarget = null;
			}
		}
		else
		{
			if (m_submarine != null)
			{
				m_submarine.SetSubmergeBlocked(blocked: false);
			}
			m_warningDisplay.SetActive(value: false);
			m_isChargingAttack = false;
		}
	}
}
