using BomberCrewCommon;
using UnityEngine;

public class ProjectileEffect : ProjectileEffectBase
{
	[SerializeField]
	private Transform m_scaleAndPositionTransform;

	[SerializeField]
	private Transform m_startPos;

	[SerializeField]
	private Renderer m_projectileRenderer;

	[SerializeField]
	private Transform m_lastRaycastEndPosition;

	[SerializeField]
	private Transform m_lastDrawEndPosition;

	[SerializeField]
	private float m_minLineLength = 0.001f;

	[SerializeField]
	private bool m_incendiaryEffect;

	[SerializeField]
	private bool m_useSwitchOffOptimisation;

	[SerializeField]
	private LayerMask m_layerForSwitchOffOptimisation;

	[SerializeField]
	private BigTransform m_bigTransform;

	private BomberState m_bomberState;

	private float m_damage;

	private LayerMask m_targetLayermask;

	private Vector3 m_inheritedVelocity;

	private Vector3 m_direction;

	private float m_projectileStartVelocity = 100f;

	private Transform m_relativeMovementTrackingTransform;

	private ProjectileType m_projectileType;

	private object m_relatedObject;

	private float m_timer;

	private Vector3 m_prevPos;

	private bool m_useSwitchOff;

	private bool m_isSwitchedOff;

	private bool m_hasSetDirection;

	private static RaycastHit[] s_results = new RaycastHit[512];

	private void DrawRay(Vector3 fromPosition, Vector3 toPosition)
	{
		float magnitude = (fromPosition - toPosition).magnitude;
		if (magnitude > m_minLineLength)
		{
			m_startPos.position = fromPosition;
			if (!m_hasSetDirection)
			{
				m_startPos.LookAt(toPosition);
				m_hasSetDirection = true;
			}
			Vector3 localScale = m_scaleAndPositionTransform.transform.localScale;
			localScale.z = magnitude;
			m_scaleAndPositionTransform.localPosition = new Vector3(0f, 0f, localScale.z / 2f);
			m_scaleAndPositionTransform.localScale = localScale;
			m_projectileRenderer.enabled = true;
		}
		else
		{
			m_projectileRenderer.enabled = false;
		}
		m_lastDrawEndPosition.position = toPosition;
	}

	private void DrawRay(Vector3 toPosition)
	{
		DrawRay(m_lastDrawEndPosition.position, toPosition);
	}

	private void Awake()
	{
		m_projectileRenderer.enabled = false;
	}

	public override void TriggerEffect(Vector3 startPos, Vector3 endPos, float muzzleVelocity, float damage, LayerMask targetLayermask, Vector3 inheritedVelocity, object relatedObject, ProjectileType projectileType)
	{
		m_bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		m_hasSetDirection = false;
		m_projectileType = projectileType;
		m_relatedObject = relatedObject;
		m_projectileStartVelocity = muzzleVelocity;
		m_damage = damage;
		m_targetLayermask = targetLayermask;
		m_direction = (endPos - startPos).normalized;
		base.transform.position = startPos;
		base.transform.LookAt(endPos);
		m_bigTransform.SetFromCurrentPage(base.transform.position);
		m_lastDrawEndPosition.position = startPos;
		m_timer = 2500f / muzzleVelocity;
		if (projectileType.GoShort())
		{
			m_timer *= 0.5f;
		}
		m_useSwitchOff = false;
		m_isSwitchedOff = false;
		if (m_useSwitchOffOptimisation && (int)m_targetLayermask == (int)m_layerForSwitchOffOptimisation)
		{
			m_useSwitchOff = true;
		}
		m_projectileRenderer.enabled = true;
		m_inheritedVelocity = Vector3.zero;
		FixedUpdate();
		m_inheritedVelocity = inheritedVelocity;
	}

	public override void SetFake(bool fake)
	{
		if (fake)
		{
			m_isSwitchedOff = true;
			m_useSwitchOff = true;
		}
	}

	private void LateUpdate()
	{
		if (m_timer >= 0f)
		{
			DrawRay(m_lastRaycastEndPosition.position);
		}
	}

	private void FixedUpdate()
	{
		Vector3 position = base.transform.position;
		float num = Time.fixedDeltaTime * m_projectileStartVelocity;
		Vector3 vector = m_direction * num;
		Vector3 vector2 = position + vector;
		if (!m_isSwitchedOff)
		{
			bool flag = true;
			if (m_useSwitchOff)
			{
				Vector3 lhs = m_bomberState.transform.position - base.transform.position;
				if (lhs.sqrMagnitude > 2500f)
				{
					if (Vector3.Dot(lhs, m_direction) < 0f)
					{
						m_isSwitchedOff = true;
					}
					flag = false;
				}
			}
			if (flag)
			{
				int num2 = Physics.SphereCastNonAlloc(position, 0.05f, m_direction, s_results, num, m_targetLayermask);
				if (num2 > 0 && m_timer > 0f)
				{
					Debug.DrawLine(position, position + vector, Color.red);
					RaycastHit hit = default(RaycastHit);
					float num3 = float.MaxValue;
					for (int i = 0; i < num2; i++)
					{
						RaycastHit raycastHit = s_results[i];
						if (raycastHit.collider != null && raycastHit.distance < num3)
						{
							num3 = raycastHit.distance;
							hit = raycastHit;
						}
					}
					vector2 = Hit(hit, position);
					m_timer = 0f;
				}
				else
				{
					m_timer -= Time.fixedDeltaTime;
				}
			}
			else
			{
				m_timer -= Time.fixedDeltaTime;
			}
		}
		else
		{
			m_timer -= Time.fixedDeltaTime * 3f;
		}
		if (m_timer < 0f || base.transform.position.y < 0f || base.transform.position.y > 2500f)
		{
			Singleton<PoolManager>.Instance.ReturnToPool(base.gameObject);
		}
		Vector3 position2 = m_lastDrawEndPosition.position + m_inheritedVelocity * Time.fixedDeltaTime;
		Vector3 f = vector2 + m_inheritedVelocity * Time.fixedDeltaTime;
		m_bigTransform.SoftSetFromCurrentPage(f);
		m_lastDrawEndPosition.position = position2;
		m_lastRaycastEndPosition.position = vector2 + m_inheritedVelocity * Time.fixedDeltaTime;
	}

	private Vector3 Hit(RaycastHit hit, Vector3 startPoint)
	{
		Damageable component = hit.collider.GetComponent<Damageable>();
		Vector3 normalized = m_direction.normalized;
		Ray ray = new Ray(startPoint - normalized * 1f, normalized);
		RaycastHit hitInfo = hit;
		if (hit.collider.Raycast(ray, out hitInfo, 32f))
		{
			hit = hitInfo;
		}
		Vector3 point = hit.point;
		if (component != null)
		{
			DamageUtils.DoExternalRayDamage(m_damage, component, DamageSource.DamageType.Impact, point, hit, m_direction, base.transform.position, m_incendiaryEffect, m_relatedObject, m_projectileType);
		}
		if (m_projectileType.DoesRadialDamage())
		{
			DamageUtils.DoDamageSlowWithBlock(point, 0f, m_projectileType.GetRadialDamageAmount(), m_projectileType.GetRadius(), DamageSource.DamageType.Impact);
		}
		return point;
	}
}
