using System;
using AudioNames;
using BomberCrewCommon;
using dbox;
using UnityEngine;
using WingroveAudio;

public class RaycastGunTurret : MonoBehaviour
{
	[SerializeField]
	private LayerMask m_targetLayerMask;

	[SerializeField]
	private Transform m_orientationNode;

	[SerializeField]
	private GameObject[] m_muzzleFlashes;

	[SerializeField]
	private ProjectileType m_projectileSetting;

	[SerializeField]
	private Animation m_recoilAnimation;

	[SerializeField]
	private bool m_skipAudioHook;

	[SerializeField]
	private bool m_doAudioFilter;

	[SerializeField]
	private bool m_isDouble;

	[SerializeField]
	private bool m_isQuad;

	[SerializeField]
	private bool m_isDud;

	private bool m_firing;

	private float m_firingDelay;

	private float m_range = 2500f;

	private float m_muzzleVelocity = 500f;

	private bool m_firingInEffect;

	private bool m_fireFake;

	private Vector3 m_inheritedVelocity = Vector3.zero;

	private AmmoFeed m_ammoFeed;

	private float m_damageMultiplier = 1f;

	private int m_cachedGameObjectId;

	private object m_relatedObject;

	private bool m_firingInEffectLastFrame;

	private float m_firePhaseCutoff = 1f;

	private float m_firePhase;

	private bool m_hasRecoilAnimation;

	private bool m_sendDboxEvent;

	private void Awake()
	{
		m_cachedGameObjectId = base.gameObject.GetInstanceID();
		GameObject[] muzzleFlashes = m_muzzleFlashes;
		foreach (GameObject gameObject in muzzleFlashes)
		{
			gameObject.SetActive(value: false);
		}
		m_hasRecoilAnimation = m_recoilAnimation != null;
	}

	public void SetSendsDBoxEvents()
	{
		m_sendDboxEvent = true;
	}

	public void SetFirePhase(float phase)
	{
		m_firePhase = phase;
	}

	public void SetProjectileType(ProjectileType pt)
	{
		m_projectileSetting = pt;
	}

	public void SetFireFake(bool fireFake)
	{
		m_fireFake = fireFake;
	}

	public float GetMuzzleVelocity()
	{
		return m_muzzleVelocity;
	}

	public void SetFiring(bool firing, Vector3 inheritedVelocity)
	{
		m_firing = firing;
		m_inheritedVelocity = inheritedVelocity;
	}

	public void SetGroup(float t)
	{
		m_firingDelay = m_projectileSetting.GetFireInterval() * t;
	}

	public void SetAmmoFeed(AmmoFeed ammoFeed)
	{
		m_ammoFeed = ammoFeed;
	}

	public void SetRelatedObject(object o)
	{
		m_relatedObject = o;
	}

	public void PoolProjectiles(int count)
	{
		if (!Singleton<GameFlow>.Instance.GetGameMode().UseEndlessDifficulty() && !m_isDud)
		{
			if (m_isQuad)
			{
				Singleton<PoolManager>.Instance.PoolPrefabAdditional(m_projectileSetting.GetProjectileQuadPrefab(), 2);
			}
			else if (m_isDouble)
			{
				Singleton<PoolManager>.Instance.PoolPrefabAdditional(m_projectileSetting.GetProjectileDoublePrefab(), 2);
			}
			else
			{
				Singleton<PoolManager>.Instance.PoolPrefabAdditional(m_projectileSetting.GetProjectilePrefab(), 2);
			}
		}
	}

	private void Update()
	{
		m_firePhase += Time.deltaTime;
		if (m_firePhase > (float)Math.PI * 2f)
		{
			m_firePhase -= (float)Math.PI * 2f;
		}
		float num = Mathf.Sin(m_firePhase) * 0.5f + 0.5f;
		m_firingInEffectLastFrame = false;
		bool flag = true;
		if (m_firing)
		{
			m_firingDelay -= Time.deltaTime;
			if (m_ammoFeed != null)
			{
				if (m_ammoFeed.GetAmmo() != 0)
				{
					m_ammoFeed.UseAmmo();
				}
				else
				{
					flag = false;
				}
			}
			if (m_firingDelay < 0f)
			{
				if (num > m_firePhaseCutoff)
				{
					flag = false;
				}
				if (flag)
				{
					m_firingDelay = m_projectileSetting.GetFireInterval();
					if (Singleton<CommonEffectManager>.Instance != null)
					{
						if (!m_isDud)
						{
							float num2 = m_damageMultiplier;
							if (m_isDouble)
							{
								num2 *= 2f;
							}
							else if (m_isQuad)
							{
								num2 *= 4f;
							}
							Singleton<CommonEffectManager>.Instance.ProjectileEffect(m_projectileSetting, m_orientationNode.position, m_orientationNode.position + m_orientationNode.forward * m_range, num2, m_targetLayerMask, m_inheritedVelocity, m_relatedObject, m_isDouble, m_isQuad, m_fireFake);
							if (m_sendDboxEvent)
							{
								DboxInMissionController.DBoxCall(DboxSdkWrapper.PostFireGun);
							}
						}
						if (!m_skipAudioHook)
						{
							WingroveRoot.Instance.PostEventGO(m_projectileSetting.GetFireAudioHook(), base.gameObject);
							if (m_doAudioFilter)
							{
								float setValue = 0f;
								WingroveListener singleListener = WingroveRoot.Instance.GetSingleListener();
								if (singleListener != null)
								{
									float magnitude = (singleListener.transform.position - base.transform.position).magnitude;
									setValue = Mathf.Clamp01((magnitude - 100f) / 1100f);
								}
								WingroveRoot.Instance.SetParameterForObject(GameEvents.Parameters.CacheVal_ObjectDistance(), m_cachedGameObjectId, base.gameObject, setValue);
							}
						}
						if (m_hasRecoilAnimation)
						{
							m_recoilAnimation.Play();
						}
					}
				}
			}
		}
		bool flag2 = m_firing && flag;
		if (flag2 != m_firingInEffect)
		{
			m_firingInEffect = flag2;
			GameObject[] muzzleFlashes = m_muzzleFlashes;
			foreach (GameObject gameObject in muzzleFlashes)
			{
				gameObject.SetActive(m_firingInEffect);
			}
		}
		m_firingInEffectLastFrame = m_firing && flag;
	}

	public bool DidFire()
	{
		return m_firingInEffectLastFrame;
	}

	public void SetDamageMultiplier(float damageMult)
	{
		m_damageMultiplier = damageMult;
	}

	public void SetFirePhaseCutoff(float firePhaseCutoff)
	{
		m_firePhaseCutoff = firePhaseCutoff;
	}

	public AmmoFeed GetAmmoFeed()
	{
		return m_ammoFeed;
	}
}
