using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class CommonEffectManager : Singleton<CommonEffectManager>
{
	public enum AudioHitType
	{
		Fuselage,
		Heavy,
		Character
	}

	[SerializeField]
	private GameObject m_hitEffectPrefab;

	private int m_hitEffectPrefabInstanceId;

	private int m_effectHitsThisFrame;

	private void Start()
	{
		m_hitEffectPrefabInstanceId = m_hitEffectPrefab.GetInstanceID();
	}

	private void LateUpdate()
	{
		m_effectHitsThisFrame = 0;
	}

	public GameObject ProjectileEffect(ProjectileType pt, Vector3 origin, Vector3 target, float damageMultiplier, LayerMask targetLayerMask, Vector3 inheritedVelocity, object relatedObject, bool isDouble, bool isQuad, bool isFake)
	{
		GameObject gameObject = null;
		gameObject = (isDouble ? Singleton<PoolManager>.Instance.GetFromPool(pt.GetProjectileDoublePrefab(), pt.GetProjectilePrefabDoubleId()) : ((!isQuad) ? Singleton<PoolManager>.Instance.GetFromPool(pt.GetProjectilePrefab(), pt.GetProjectilePrefabId()) : Singleton<PoolManager>.Instance.GetFromPool(pt.GetProjectileQuadPrefab(), pt.GetProjectilePrefabQuadId())));
		ProjectileEffectBase component = gameObject.GetComponent<ProjectileEffectBase>();
		component.TriggerEffect(origin, target, pt.GetVelocity(), pt.GetDamage() * damageMultiplier, targetLayerMask, inheritedVelocity, relatedObject, pt);
		component.SetFake(isFake);
		return gameObject;
	}

	public void EffectHit(Vector3 hitPoint, Quaternion hitRotation, Transform trackingTransform, AudioHitType hitType, ProjectileType fromProjectile)
	{
		m_effectHitsThisFrame++;
		if (m_effectHitsThisFrame < 40)
		{
			GameObject fromPool;
			if (fromProjectile == null || fromProjectile.GetHitEffect() == null)
			{
				fromPool = Singleton<PoolManager>.Instance.GetFromPool(m_hitEffectPrefab, m_hitEffectPrefabInstanceId);
				fromPool.GetComponent<EffectHit>().TriggerEffect(hitPoint, hitRotation, trackingTransform);
			}
			else
			{
				fromPool = Singleton<PoolManager>.Instance.GetFromPool(fromProjectile.GetHitEffect(), fromProjectile.GetCachedInstanceIdHitEffect());
				fromPool.GetComponent<EffectHit>().TriggerEffect(hitPoint, hitRotation, trackingTransform);
			}
			if (fromProjectile != null && fromProjectile.GetAudioHitExtraEnabled())
			{
				WingroveRoot.Instance.PostEventGO(fromProjectile.GetAudioHitExtra(), fromPool);
			}
			switch (hitType)
			{
			case AudioHitType.Fuselage:
				WingroveRoot.Instance.PostEventGO("IMPACT_FUSELAGE", fromPool);
				break;
			case AudioHitType.Heavy:
				WingroveRoot.Instance.PostEventGO("IMPACT_HEAVY", fromPool);
				break;
			}
		}
	}
}
