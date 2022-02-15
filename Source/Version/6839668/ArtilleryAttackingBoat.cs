using System.Collections;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class ArtilleryAttackingBoat : MonoBehaviour
{
	[SerializeField]
	private Transform m_xzRotationTransform;

	[SerializeField]
	private Transform m_yRotationTransform;

	[SerializeField]
	private float m_yMin = 20f;

	[SerializeField]
	private float m_yMax = 90f;

	[SerializeField]
	private float m_yAdd = 20f;

	[SerializeField]
	private Transform m_aimNode;

	[SerializeField]
	private float m_maxFireRange = 1500f;

	[SerializeField]
	private float m_maxFireHeight = 300f;

	[SerializeField]
	private float m_maxTurnRate = 45f;

	[SerializeField]
	private float m_chanceHitStart;

	[SerializeField]
	private float m_chanceHitEnd;

	[SerializeField]
	private float m_timeForChanceHit;

	[SerializeField]
	private float m_fireDelay;

	[SerializeField]
	private int m_shotsPerFire;

	[SerializeField]
	private SmoothDamageable m_damageableToMonitor;

	[SerializeField]
	private float m_damagePerHit;

	[SerializeField]
	private float m_xMin = -360f;

	[SerializeField]
	private float m_xMax = 360f;

	[SerializeField]
	private ShootableType m_targetType;

	[SerializeField]
	private ParticleSystem m_particleSystemOnFire;

	[SerializeField]
	[AudioEventName]
	private string m_onFireAudioEvent;

	[SerializeField]
	private GameObject m_splashPrefab;

	[SerializeField]
	private GameObject m_hitsPrefab;

	private float m_targetReselect = 3f;

	private Transform m_aimTarget;

	private float m_curYAng = 30f;

	private float m_curXAng = 90f;

	private float m_firingTimeCurrent;

	private float m_visibilityCounter;

	private Vector3 m_aimAheadPos;

	private float m_currentInaccuracyRandom;

	private float m_currentInaccuracyLockCountdown;

	private Shootable m_currentTarget;

	private float m_timeActive;

	private int m_splashPrefabInstanceId;

	private int m_hitsPrefabInstanceId;

	private void Start()
	{
		m_splashPrefabInstanceId = m_splashPrefab.GetInstanceID();
		m_hitsPrefabInstanceId = m_hitsPrefab.GetInstanceID();
		m_particleSystemOnFire.Stop();
	}

	private void Update()
	{
		if (m_damageableToMonitor != null && m_damageableToMonitor.GetHealthNormalised() <= 0f)
		{
			return;
		}
		if (m_currentTarget == null || m_visibilityCounter < -10f || m_currentTarget.IsDestroyed())
		{
			m_currentTarget = Singleton<ShootableCoordinator>.Instance.GetNearestShootable(m_targetType, base.transform.position);
			m_visibilityCounter = 0f;
		}
		if (m_currentTarget != null)
		{
			m_targetReselect -= Time.deltaTime;
			if (m_targetReselect < 0f)
			{
				m_targetReselect = 3f;
				m_aimTarget = null;
			}
			if (m_aimTarget == null)
			{
				m_aimTarget = m_currentTarget.GetRandomTargetableArea();
			}
			Vector3 vector = Vector3.one;
			Transform centreTransform = m_currentTarget.GetCentreTransform();
			if (Singleton<VisibilityHelpers>.Instance.IsVisibleAILenient(base.transform.position + Vector3.up * 50f, centreTransform.position + Vector3.up * 50f, isTargetLitUp: true))
			{
				m_visibilityCounter = 3f;
				Vector3 vector2 = (m_aimAheadPos = AimingUtils.GetAimAheadTarget(m_aimNode.position, centreTransform.position, m_currentTarget.GetVelocity(), 500f));
				vector2 += m_currentInaccuracyLockCountdown * m_currentTarget.GetVelocity().normalized * m_currentInaccuracyRandom;
				vector = vector2 - m_aimNode.position;
			}
			m_visibilityCounter -= Time.deltaTime;
			bool flag = false;
			if (m_visibilityCounter > 0f)
			{
				Vector3 v = vector;
				Vector3 vector3 = base.transform.worldToLocalMatrix.MultiplyVector(v);
				float num = Mathf.Atan2(vector3.y, new Vector3(vector3.x, 0f, vector3.z).magnitude) * 57.29578f;
				float num2 = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
				float num3 = num2 - m_curXAng;
				if (num3 > 180f)
				{
					num3 = 360f - num3;
				}
				else if (num3 < -180f)
				{
					num3 = -360f - num3;
				}
				m_curXAng += Mathf.Clamp(num3, (0f - m_maxTurnRate) * Time.deltaTime, m_maxTurnRate * Time.deltaTime);
				num3 = num - m_curYAng;
				if (num3 > 180f)
				{
					num3 = 360f - num3;
				}
				else if (num3 < -180f)
				{
					num3 = -360f - num3;
				}
				m_curYAng += Mathf.Clamp(num3, (0f - m_maxTurnRate) * Time.deltaTime, m_maxTurnRate * Time.deltaTime);
				m_curXAng = num2;
				m_curYAng = num;
				if (vector.magnitude < m_maxFireRange && Mathf.Abs(vector.y) < m_maxFireHeight && Vector3.Dot(m_aimNode.forward, vector.normalized) > 0.9f)
				{
					flag = true;
				}
			}
			if (flag)
			{
				m_timeActive += Time.deltaTime;
				m_firingTimeCurrent -= Time.deltaTime;
				if (m_firingTimeCurrent < 0f)
				{
					m_firingTimeCurrent = m_fireDelay;
					m_particleSystemOnFire.Play(withChildren: true);
					WingroveRoot.Instance.PostEventGO(m_onFireAudioEvent, base.gameObject);
					StartCoroutine(DoShots(m_currentTarget));
				}
			}
			else
			{
				m_timeActive = 0f;
			}
		}
		else
		{
			m_timeActive = 0f;
		}
		m_curYAng = Mathf.Clamp(m_curYAng, m_yMin, m_yMax);
		m_curXAng = Mathf.Clamp(m_curXAng, m_xMin, m_xMax);
		m_xzRotationTransform.localEulerAngles = new Vector3(0f, m_curXAng, 0f);
		m_yRotationTransform.localEulerAngles = new Vector3(0f - m_curYAng + m_yAdd, 0f, 0f);
	}

	private IEnumerator DoShots(Shootable target)
	{
		yield return new WaitForSeconds(0.1f);
		Transform t = target.GetCentreTransform();
		for (int i = 0; i < m_shotsPerFire; i++)
		{
			if (t != null && target != null)
			{
				float num = Random.Range(0f, 1f);
				float num2 = Mathf.Lerp(m_chanceHitStart, m_chanceHitEnd, Mathf.Clamp01(m_timeActive / m_timeForChanceHit));
				if (num < num2)
				{
					GameObject mainObject = target.GetMainObject();
					Damageable component = mainObject.GetComponent<Damageable>();
					DamageSource damageSource = new DamageSource();
					damageSource.m_damageShapeEffect = DamageSource.DamageShape.None;
					damageSource.m_damageType = DamageSource.DamageType.Impact;
					component.DamageGetPassthrough(m_damagePerHit, damageSource);
					GameObject fromPool = Singleton<PoolManager>.Instance.GetFromPool(m_hitsPrefab, m_hitsPrefabInstanceId);
					Vector3 normalized = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f)).normalized;
					float num3 = Random.Range(10, 20);
					Vector3 nearPos = t.position + normalized * num3;
					Vector3 damagePosition = component.GetDamagePosition(nearPos);
					fromPool.btransform().SetFromCurrentPage(damagePosition);
				}
				else
				{
					GameObject fromPool2 = Singleton<PoolManager>.Instance.GetFromPool(m_splashPrefab, m_splashPrefabInstanceId);
					Vector3 normalized2 = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
					float num4 = Random.Range(100, 140);
					Vector3 fromCurrentPage = t.position + normalized2 * num4;
					fromCurrentPage.y = 0f;
					fromPool2.btransform().SetFromCurrentPage(fromCurrentPage);
				}
			}
			yield return null;
		}
	}

	private void OnDrawGizmos()
	{
		if (m_currentTarget != null)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(m_currentTarget.GetCentreTransform().position, m_aimNode.position);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(m_aimAheadPos, m_aimNode.position);
		}
	}
}
