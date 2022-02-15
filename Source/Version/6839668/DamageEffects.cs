using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class DamageEffects : MonoBehaviour
{
	public enum EffectHit
	{
		None,
		MetallicImpactEffect,
		MetallicImpactEffectHeavy
	}

	[SerializeField]
	private int m_maxSphereHolesAtOnce;

	[SerializeField]
	private float m_numSphereHolesPerDamage;

	[SerializeField]
	private EffectHit m_effectLineImpact;

	[SerializeField]
	private EffectHit m_effectSphereImpact;

	[SerializeField]
	private DamageableMeshSharedMask m_damageMask;

	[SerializeField]
	private bool m_rumbleOnHit;

	private Vector3 m_previousPosition;

	private Vector3 m_deltaPos;

	public void DoDamageEffect(Collider col, DamageSource damageSource, float actualDamage)
	{
		if (damageSource.m_damageShapeEffect == DamageSource.DamageShape.Line)
		{
			if (damageSource.m_damageType == DamageSource.DamageType.Impact)
			{
				if (m_effectLineImpact == EffectHit.MetallicImpactEffect)
				{
					Singleton<CommonEffectManager>.Instance.EffectHit(damageSource.m_raycastInfo.point + m_deltaPos, Quaternion.LookRotation(damageSource.m_raycastInfo.normal), base.transform, CommonEffectManager.AudioHitType.Fuselage, damageSource.m_fromProjectile);
				}
				else if (m_effectLineImpact == EffectHit.MetallicImpactEffectHeavy)
				{
					Singleton<CommonEffectManager>.Instance.EffectHit(damageSource.m_raycastInfo.point + m_deltaPos, Quaternion.LookRotation(damageSource.m_raycastInfo.normal), base.transform, CommonEffectManager.AudioHitType.Heavy, damageSource.m_fromProjectile);
				}
				if (m_damageMask != null)
				{
					m_damageMask.DamageFromRaycastHit(damageSource.m_raycastInfo, Singleton<DamageShapes>.Instance.GetShape(), 2f);
				}
				if (m_rumbleOnHit)
				{
					Singleton<ControllerRumble>.Instance.GetRumbleMixerHit().Kick();
				}
			}
		}
		else
		{
			if (damageSource.m_damageShapeEffect != DamageSource.DamageShape.Sphere)
			{
				return;
			}
			if (damageSource.m_damageType == DamageSource.DamageType.Impact)
			{
				if (m_rumbleOnHit)
				{
					Singleton<ControllerRumble>.Instance.GetRumbleMixerShock().Kick();
				}
				Vector3 vector = col.ClosestPointOnBounds(damageSource.m_position);
				float num = m_numSphereHolesPerDamage * actualDamage;
				int num2 = (int)Mathf.Clamp(num, 1f, m_maxSphereHolesAtOnce);
				Ray ray;
				if (damageSource.m_position != vector)
				{
					ray = new Ray(damageSource.m_position, (vector - damageSource.m_position).normalized);
				}
				else
				{
					Vector3 vector2 = col.transform.position - vector;
					ray = new Ray(damageSource.m_position - vector2 * 0.05f, (vector + vector2 * 0.05f - damageSource.m_position).normalized);
				}
				if (!col.Raycast(ray, out var hitInfo, damageSource.m_radius))
				{
					return;
				}
				float num3 = 1f - (vector - damageSource.m_position).magnitude / damageSource.m_radius;
				float num4 = Mathf.Clamp(num / (float)num2, 1f, 4f);
				float areaRadius = num3 * damageSource.m_radius;
				for (int i = 0; i < num2; i++)
				{
					if (m_effectLineImpact == EffectHit.MetallicImpactEffect)
					{
						Vector3 insideUnitSphere = Random.insideUnitSphere;
						insideUnitSphere -= Vector3.Dot(insideUnitSphere, hitInfo.normal) * hitInfo.normal;
						if (m_effectSphereImpact == EffectHit.MetallicImpactEffect)
						{
							Singleton<CommonEffectManager>.Instance.EffectHit(vector + insideUnitSphere + m_deltaPos, Quaternion.LookRotation(hitInfo.normal), base.transform, CommonEffectManager.AudioHitType.Fuselage, null);
						}
						else
						{
							Singleton<CommonEffectManager>.Instance.EffectHit(vector + insideUnitSphere + m_deltaPos, Quaternion.LookRotation(hitInfo.normal), base.transform, CommonEffectManager.AudioHitType.Heavy, null);
						}
					}
				}
				if (m_damageMask != null)
				{
					m_damageMask.DamageFromUVSphere(hitInfo.textureCoord, 2f * num4, areaRadius, num2, Singleton<DamageShapes>.Instance.GetShape());
				}
			}
			else if (damageSource.m_damageType == DamageSource.DamageType.GroundImpact && actualDamage > 5f)
			{
				if (m_rumbleOnHit)
				{
					Singleton<ControllerRumble>.Instance.GetRumbleMixerShock().Kick();
				}
				WingroveRoot.Instance.PostEventGO("IMPACT_GROUND", base.gameObject);
			}
		}
	}

	private void Update()
	{
	}
}
