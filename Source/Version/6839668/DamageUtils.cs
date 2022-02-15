using System.Collections.Generic;
using UnityEngine;

public static class DamageUtils
{
	private static Collider[] m_colliderBuffer = new Collider[512];

	private static RaycastHit[] s_results = new RaycastHit[512];

	private static Dictionary<int, Vector3> m_blockableDirs = new Dictionary<int, Vector3>(16);

	private static Dictionary<int, float> m_blockableAmounts = new Dictionary<int, float>(16);

	private static Dictionary<int, Vector3> m_otherDamageables = new Dictionary<int, Vector3>(16);

	private static Dictionary<int, Damageable> m_intToDamageable = new Dictionary<int, Damageable>(16);

	private static bool m_isLocked = false;

	public static void DoExternalRayDamage(float amt, Damageable firstHit, DamageSource.DamageType dt, Vector3 positionOfHit, RaycastHit rch, Vector3 trajectory, Vector3 positionFrom, bool incendiary, object relatedObject, ProjectileType projectileType)
	{
		if (m_isLocked)
		{
			DebugLogWrapper.LogError("DAMAGE FROM DAMAGE!");
		}
		m_isLocked = true;
		DamageSource damageSource = new DamageSource();
		GameObject gameObject = firstHit.gameObject;
		damageSource.m_damageShapeEffect = DamageSource.DamageShape.Line;
		damageSource.m_position = positionOfHit;
		damageSource.m_raycastInfo = rch;
		damageSource.m_trajectory = trajectory;
		damageSource.m_damageType = dt;
		damageSource.m_incendiaryEffect = incendiary;
		damageSource.m_relatedObject = relatedObject;
		damageSource.m_fromProjectile = projectileType;
		float num = firstHit.DamageGetPassthrough(amt, damageSource);
		if (num > 0f)
		{
			int num2 = Physics.SphereCastNonAlloc(positionOfHit, 0.35f, trajectory.normalized, s_results, 4f);
			for (int i = 0; i < num2; i++)
			{
				RaycastHit raycastHit = s_results[i];
				if (raycastHit.collider != null && raycastHit.collider.gameObject != gameObject)
				{
					Damageable component = raycastHit.collider.GetComponent<Damageable>();
					if (component != null && !component.DirectDamageOnly())
					{
						component.DamageGetPassthrough(amt * num, damageSource);
					}
				}
			}
		}
		m_isLocked = false;
	}

	public static void DoDamageSphereFastEverything(Vector3 pos, float damageMin, float damageMax, float radius, DamageSource.DamageType dt)
	{
		if (m_isLocked)
		{
			DebugLogWrapper.LogError("DAMAGE FROM DAMAGE!");
		}
		m_isLocked = true;
		DamageSource damageSource = new DamageSource();
		damageSource.m_damageShapeEffect = DamageSource.DamageShape.Sphere;
		damageSource.m_damageType = dt;
		damageSource.m_position = pos;
		damageSource.m_radius = radius;
		Collider[] array = Physics.OverlapSphere(pos, radius);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			Damageable component = collider.GetComponent<Damageable>();
			if (component != null)
			{
				Vector3 vector = collider.ClosestPointOnBounds(pos);
				float magnitude = (pos - vector).magnitude;
				float t = magnitude / radius;
				float amt = Mathf.Lerp(damageMax, damageMin, t);
				component.DamageGetPassthrough(amt, damageSource);
			}
		}
		m_isLocked = false;
	}

	public static void DoDamageSlowWithBlock(Vector3 pos, float damageMin, float damageMax, float radius, DamageSource.DamageType dt)
	{
		if (m_isLocked)
		{
			DebugLogWrapper.LogError("DAMAGE FROM DAMAGE!");
		}
		m_isLocked = true;
		DamageSource damageSource = new DamageSource();
		damageSource.m_damageShapeEffect = DamageSource.DamageShape.Sphere;
		damageSource.m_damageType = dt;
		damageSource.m_position = pos;
		damageSource.m_radius = radius;
		Collider[] array = Physics.OverlapSphere(pos, radius);
		bool flag = false;
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			Damageable component = collider.GetComponent<Damageable>();
			if (component != null)
			{
				int objectIdCached = component.GetObjectIdCached();
				Vector3 vector = collider.ClosestPointOnBounds(pos);
				Vector3 value = vector - pos;
				float magnitude = value.magnitude;
				float t = magnitude / radius;
				if (component.IsDamageBlocker())
				{
					m_blockableDirs[objectIdCached] = value;
					float amt = Mathf.Lerp(damageMax, damageMin, t);
					m_blockableAmounts[objectIdCached] = component.DamageGetPassthrough(amt, damageSource);
				}
				else
				{
					m_otherDamageables[objectIdCached] = value;
				}
				m_intToDamageable[objectIdCached] = component;
				flag = true;
			}
		}
		foreach (KeyValuePair<int, Vector3> otherDamageable in m_otherDamageables)
		{
			float num = 1f;
			foreach (KeyValuePair<int, Vector3> blockableDir in m_blockableDirs)
			{
				if (Vector3.Dot(otherDamageable.Value, blockableDir.Value) > 0.5f && blockableDir.Value.magnitude < otherDamageable.Value.magnitude)
				{
					num *= m_blockableAmounts[blockableDir.Key];
				}
			}
			float magnitude2 = otherDamageable.Value.magnitude;
			float t2 = magnitude2 / radius;
			Damageable damageable = m_intToDamageable[otherDamageable.Key];
			if (num == 1f || !damageable.DirectDamageOnly())
			{
				damageable.DamageGetPassthrough(num * Mathf.Lerp(damageMax, damageMin, t2), damageSource);
			}
		}
		if (flag)
		{
			m_blockableDirs.Clear();
			m_blockableAmounts.Clear();
			m_otherDamageables.Clear();
			m_intToDamageable.Clear();
		}
		m_isLocked = false;
	}
}
