using BomberCrewCommon;
using UnityEngine;

public class ProjectileEffectNoLineEffect : ProjectileEffectBase
{
	private BomberState m_bomberState;

	private float m_damage;

	private LayerMask m_targetLayermask;

	private Vector3 m_inheritedVelocity;

	private Vector3 m_direction;

	private float m_projectileStartVelocity = 100f;

	private Transform m_relativeMovementTrackingTransform;

	private object m_relatedObject;

	private float m_timer;

	private Vector3 m_prevPos;

	private ProjectileType m_projectileType;

	private void Awake()
	{
		m_bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
	}

	public override void TriggerEffect(Vector3 startPos, Vector3 endPos, float muzzleVelocity, float damage, LayerMask targetLayermask, Vector3 inheritedVelocity, object relatedObject, ProjectileType projectileType)
	{
		m_projectileType = projectileType;
		m_relatedObject = relatedObject;
		m_projectileStartVelocity = muzzleVelocity;
		m_damage = damage;
		m_targetLayermask = targetLayermask;
		m_direction = (endPos - startPos).normalized;
		base.transform.position = startPos;
		base.transform.LookAt(endPos);
		base.gameObject.btransform().SetFromCurrentPage(base.transform.position);
		m_timer = 16f;
		m_inheritedVelocity = Vector3.zero;
		FixedUpdate();
		m_inheritedVelocity = inheritedVelocity;
	}

	private void FixedUpdate()
	{
		Vector3 position = base.transform.position;
		Vector3 vector = m_direction * Time.fixedDeltaTime * m_projectileStartVelocity;
		Vector3 vector2 = position + vector;
		RaycastHit[] array = Physics.SphereCastAll(position, 0.05f, vector.normalized, vector.magnitude, m_targetLayermask);
		if (array.Length > 0 && m_timer > 0f)
		{
			Debug.DrawLine(position, position + vector, Color.red);
			RaycastHit hit = default(RaycastHit);
			float num = float.MaxValue;
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (raycastHit.collider != null && raycastHit.distance < num)
				{
					num = raycastHit.distance;
					hit = raycastHit;
				}
			}
			vector2 = Hit(hit, position);
			m_timer = 0f;
		}
		else
		{
			Debug.DrawLine(position, position + vector, Color.white);
			m_timer -= Time.fixedDeltaTime;
		}
		if (m_timer < 0f || base.transform.position.y < 0f || base.transform.position.y > 10000f)
		{
			Singleton<PoolManager>.Instance.ReturnToPool(base.gameObject);
		}
		base.transform.position = vector2 + m_inheritedVelocity * Time.fixedDeltaTime;
	}

	private Vector3 Hit(RaycastHit hit, Vector3 startPoint)
	{
		Damageable component = hit.collider.GetComponent<Damageable>();
		Ray ray = new Ray(startPoint - m_direction * 8f, m_direction);
		RaycastHit hitInfo = hit;
		if (hit.collider.Raycast(ray, out hitInfo, 16f))
		{
			hit = hitInfo;
		}
		Vector3 point = hit.point;
		if (component != null)
		{
			DamageUtils.DoExternalRayDamage(m_damage, component, DamageSource.DamageType.Impact, point, hit, m_direction, base.transform.position, incendiary: false, m_relatedObject, m_projectileType);
		}
		return point;
	}
}
