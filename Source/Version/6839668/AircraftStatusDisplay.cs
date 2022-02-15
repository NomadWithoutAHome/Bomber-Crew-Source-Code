using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class AircraftStatusDisplay : MonoBehaviour
{
	[Serializable]
	public class ColorLerpGroup
	{
		[SerializeField]
		public Gradient m_colorGradient;

		[SerializeField]
		public DamageableSystemInfo[] m_systems;

		[SerializeField]
		public Color m_colorOnDestroyed = Color.black;
	}

	[Serializable]
	public class DamageableSystemInfo
	{
		[SerializeField]
		public tk2dBaseSprite m_spriteToColor;

		[SerializeField]
		public bool m_hasSprite;

		[SerializeField]
		public Animation m_animation;

		[SerializeField]
		public bool m_hasAnimation;

		[SerializeField]
		public string m_bomberSystemUniqueId;
	}

	[SerializeField]
	private ColorLerpGroup[] m_damageSystems;

	[SerializeField]
	private AnimationClip m_okAnim;

	[SerializeField]
	private AnimationClip m_warningAnim;

	[SerializeField]
	private AnimationClip m_alertAnim;

	private string m_okAnimName;

	private string m_warningAnimName;

	private string m_alertAnimName;

	private Dictionary<DamageableSystemInfo, SmoothDamageable> m_systemLookUp = new Dictionary<DamageableSystemInfo, SmoothDamageable>();

	private Dictionary<DamageableSystemInfo, DamageFlash> m_damageFlashes = new Dictionary<DamageableSystemInfo, DamageFlash>();

	private Dictionary<DamageableSystemInfo, SmoothDamageableRepairable> m_repairableSystem = new Dictionary<DamageableSystemInfo, SmoothDamageableRepairable>();

	private void Start()
	{
		m_okAnimName = m_okAnim.name;
		m_warningAnimName = m_warningAnim.name;
		m_alertAnimName = m_alertAnim.name;
		ColorLerpGroup[] damageSystems = m_damageSystems;
		foreach (ColorLerpGroup colorLerpGroup in damageSystems)
		{
			DamageableSystemInfo[] systems = colorLerpGroup.m_systems;
			foreach (DamageableSystemInfo dsi in systems)
			{
				BomberSystemUniqueId systemByName = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetSystemByName(dsi.m_bomberSystemUniqueId);
				if (!(systemByName != null))
				{
					continue;
				}
				SmoothDamageable component = systemByName.GetComponent<SmoothDamageable>();
				m_systemLookUp[dsi] = component;
				if (component != null)
				{
					component.OnDamage = (Action<DamageSource, float>)Delegate.Combine(component.OnDamage, (Action<DamageSource, float>)delegate
					{
						DoDamage(dsi);
					});
					if (component is SmoothDamageableRepairable)
					{
						m_repairableSystem[dsi] = (SmoothDamageableRepairable)component;
					}
				}
				if (dsi.m_spriteToColor != null)
				{
					m_damageFlashes[dsi] = dsi.m_spriteToColor.GetComponent<DamageFlash>();
				}
			}
		}
	}

	private void DoDamage(DamageableSystemInfo dsi)
	{
		DamageFlash damageFlash = m_damageFlashes[dsi];
		damageFlash.DoFlash();
	}

	private void Update()
	{
		ColorLerpGroup[] damageSystems = m_damageSystems;
		foreach (ColorLerpGroup colorLerpGroup in damageSystems)
		{
			DamageableSystemInfo[] systems = colorLerpGroup.m_systems;
			foreach (DamageableSystemInfo damageableSystemInfo in systems)
			{
				SmoothDamageable value = null;
				m_systemLookUp.TryGetValue(damageableSystemInfo, out value);
				if (damageableSystemInfo.m_hasSprite)
				{
					float healthNormalised = value.GetHealthNormalised();
					Color color = colorLerpGroup.m_colorGradient.Evaluate(healthNormalised);
					if (healthNormalised <= 0f)
					{
						color = colorLerpGroup.m_colorOnDestroyed;
					}
					damageableSystemInfo.m_spriteToColor.color = color;
				}
			}
		}
		foreach (KeyValuePair<DamageableSystemInfo, SmoothDamageableRepairable> item in m_repairableSystem)
		{
			if (item.Key.m_hasAnimation)
			{
				string animation = (item.Value.IsBroken() ? m_alertAnimName : ((!item.Value.IsUnreliable()) ? m_okAnimName : m_warningAnimName));
				if (!item.Key.m_animation.IsPlaying(animation))
				{
					item.Key.m_animation.Play(animation);
				}
			}
		}
	}
}
