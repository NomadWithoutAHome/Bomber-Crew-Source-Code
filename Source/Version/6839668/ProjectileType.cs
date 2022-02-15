using UnityEngine;
using WingroveAudio;

[CreateAssetMenu(menuName = "Bomber Crew/Projectile Type")]
public class ProjectileType : ScriptableObject
{
	[SerializeField]
	private GameObject m_projectileDisplayPrefab;

	[SerializeField]
	private GameObject m_projectileDisplayPrefabDouble;

	[SerializeField]
	private GameObject m_projectileDisplayPrefabQuad;

	[SerializeField]
	private float m_damageBase;

	[SerializeField]
	private float m_velocityBase;

	[SerializeField]
	[AudioEventName]
	private string m_fireAudioHook;

	[SerializeField]
	private float m_fireInterval;

	[SerializeField]
	private bool m_isIncendiary;

	[SerializeField]
	private GameObject m_hitEffect;

	[SerializeField]
	private bool m_extraAudioTriggerEnabled;

	[SerializeField]
	[AudioEventName]
	private string m_extraAudioTrigger = "IMPACT_FUSELAGE";

	[SerializeField]
	private bool m_radialDamage;

	[SerializeField]
	private float m_radius;

	[SerializeField]
	private float m_radialDamageAmount;

	[SerializeField]
	private bool m_goShort;

	private int m_cachedInstanceId;

	private int m_cachedInstanceDoubleId;

	private int m_cachedInstanceQuadId;

	private int m_cachedInstanceHitId;

	public bool DoesRadialDamage()
	{
		return m_radialDamage;
	}

	public bool GoShort()
	{
		return m_goShort;
	}

	public float GetRadialDamageAmount()
	{
		return m_radialDamageAmount;
	}

	public float GetRadius()
	{
		return m_radius;
	}

	public float GetFireInterval()
	{
		return m_fireInterval;
	}

	public GameObject GetHitEffect()
	{
		return m_hitEffect;
	}

	public int GetCachedInstanceIdHitEffect()
	{
		if (m_cachedInstanceHitId == 0)
		{
			m_cachedInstanceHitId = m_hitEffect.GetInstanceID();
		}
		return m_cachedInstanceHitId;
	}

	public bool GetAudioHitExtraEnabled()
	{
		return m_extraAudioTriggerEnabled;
	}

	public string GetAudioHitExtra()
	{
		return m_extraAudioTrigger;
	}

	public float GetDamage()
	{
		return m_damageBase;
	}

	public string GetFireAudioHook()
	{
		return m_fireAudioHook;
	}

	public float GetVelocity()
	{
		return m_velocityBase;
	}

	public GameObject GetProjectilePrefab()
	{
		return m_projectileDisplayPrefab;
	}

	public GameObject GetProjectileDoublePrefab()
	{
		return m_projectileDisplayPrefabDouble;
	}

	public GameObject GetProjectileQuadPrefab()
	{
		return m_projectileDisplayPrefabQuad;
	}

	public int GetProjectilePrefabId()
	{
		if (m_cachedInstanceId == 0)
		{
			m_cachedInstanceId = m_projectileDisplayPrefab.GetInstanceID();
		}
		return m_cachedInstanceId;
	}

	public int GetProjectilePrefabDoubleId()
	{
		if (m_cachedInstanceDoubleId == 0)
		{
			m_cachedInstanceDoubleId = m_projectileDisplayPrefabDouble.GetInstanceID();
		}
		return m_cachedInstanceDoubleId;
	}

	public int GetProjectilePrefabQuadId()
	{
		if (m_cachedInstanceQuadId == 0)
		{
			m_cachedInstanceQuadId = m_projectileDisplayPrefabQuad.GetInstanceID();
		}
		return m_cachedInstanceQuadId;
	}
}
