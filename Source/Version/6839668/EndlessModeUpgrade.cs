using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class EndlessModeUpgrade : MonoBehaviour
{
	public enum UpgradeType
	{
		XPUpgrade,
		Repair,
		FuelRefill,
		HealCrew,
		Points
	}

	[SerializeField]
	[NamedText]
	private string m_XPUpgradeNamedText;

	[SerializeField]
	[NamedText]
	private string m_repairNamedText;

	[SerializeField]
	[NamedText]
	private string m_fuelRefillNamedText;

	[SerializeField]
	[NamedText]
	private string m_healCrewNamedText;

	[SerializeField]
	private string m_XPUpgradeSpriteName;

	[SerializeField]
	private string m_repairSpriteName;

	[SerializeField]
	private string m_fuelRefillSpriteName;

	[SerializeField]
	private string m_healCrewSpriteName;

	[SerializeField]
	private TaggableItem m_taggable;

	[SerializeField]
	private float m_hitRadius;

	[SerializeField]
	private UpgradeType m_upgradeType;

	[SerializeField]
	private float m_magnetRadius;

	[SerializeField]
	private float m_magnetSpeedMax;

	[SerializeField]
	private float m_magnetSpeedAccRate;

	[SerializeField]
	private float m_maxTimer = 300f;

	[SerializeField]
	private Renderer m_flashRenderer;

	[SerializeField]
	private Color m_startAlpha;

	[SerializeField]
	private int m_conversionPoints = 250;

	private BomberNavigation m_bomberNavigation;

	private float m_magnetSpeed;

	private float m_timer;

	private float m_targY;

	private bool m_timerSet;

	private bool m_isCollected;

	private float m_postCollectTimer;

	private int m_scoreValue;

	private void Start()
	{
		m_taggable.OnTagComplete += TagUpgrade;
		m_bomberNavigation = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation();
		if (!m_timerSet)
		{
			m_timer = m_maxTimer;
		}
		m_flashRenderer.sharedMaterial = UnityEngine.Object.Instantiate(m_flashRenderer.sharedMaterial);
	}

	public float GetFlash()
	{
		if (m_isCollected)
		{
			return Mathf.Clamp01(1f - m_postCollectTimer * 2f);
		}
		if (m_timer < 10f && m_magnetSpeed <= 0f)
		{
			float num = m_timer - Mathf.Floor(m_timer);
			int num2 = 10 - Mathf.FloorToInt(m_timer);
			float num3 = Mathf.Clamp01(m_timer / 5f);
			return Mathf.Sin(num * (float)num2 * (float)Math.PI * 2f) * 0.5f + 0.5f * num3;
		}
		return 1f;
	}

	public void SetTimer(float timer)
	{
		m_timerSet = true;
		m_timer = timer;
	}

	public void SetUpgrade(UpgradeType ut, int altitudeIndex)
	{
		m_upgradeType = ut;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetAltitudeTargetForIndex(altitudeIndex);
		m_targY = localPosition.y;
		base.transform.localPosition = localPosition;
	}

	public int GetPoints()
	{
		return m_scoreValue;
	}

	public void SetIsPoints(int value)
	{
		m_upgradeType = UpgradeType.Points;
		m_scoreValue = value;
	}

	private void TagUpgrade()
	{
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().SetNavigationPoint(base.gameObject.btransform().position, BomberNavigation.NavigationPointType.Detour, base.gameObject);
	}

	private void Update()
	{
		if (m_upgradeType == UpgradeType.XPUpgrade && Singleton<EndlessModeController>.Instance.CrewAreAtMaxLevel())
		{
			SetIsPoints(m_conversionPoints);
		}
		if (!m_isCollected)
		{
			Vector3d position = base.gameObject.btransform().position;
			Vector3d position2 = Singleton<BomberSpawn>.Instance.GetBomberSystems().gameObject.btransform().position;
			float num = m_targY;
			if (position2.y < (double)(m_targY + 75f) && position2.y > (double)(m_targY - 75f))
			{
				num = (float)position2.y;
			}
			float f = num - (float)position.y;
			if (Mathf.Abs(f) > 0.01f)
			{
				float value = Mathf.Sign(f) * Time.deltaTime * 15f;
				value = Mathf.Clamp(value, 0f - Mathf.Abs(f), Mathf.Abs(f));
				position.y += value;
			}
			base.gameObject.btransform().position = position;
			if (m_bomberNavigation.GetAssociatedObject() != base.gameObject)
			{
				m_taggable.SetTagIncomplete();
			}
			Vector3d vector3d = position - position2;
			if (vector3d.magnitude < (double)m_magnetRadius)
			{
				m_magnetSpeed += Time.deltaTime * m_magnetSpeedAccRate;
				if (m_magnetSpeed > 1f)
				{
					m_magnetSpeed = 1f;
				}
				if (vector3d.magnitude < (double)m_hitRadius)
				{
					WingroveRoot.Instance.PostEvent("COLLECT_ENDLESS_POWERUP");
					DoUpgrade();
				}
			}
			else
			{
				m_magnetSpeed -= Time.deltaTime;
				if (m_magnetSpeed < 0f)
				{
					m_magnetSpeed = 0f;
				}
			}
			Color startAlpha = m_startAlpha;
			startAlpha.a *= GetFlash();
			m_flashRenderer.sharedMaterial.SetColor("_Color", startAlpha);
			if (m_magnetSpeed > 0f)
			{
				base.gameObject.btransform().position -= vector3d.normalized * Mathf.Min(Time.deltaTime * m_magnetSpeed * m_magnetSpeedMax, (float)vector3d.magnitude);
				return;
			}
			m_timer -= Time.deltaTime;
			if (m_timer < 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		else
		{
			base.transform.parent = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform;
			m_postCollectTimer += Time.deltaTime;
			m_flashRenderer.transform.localScale *= 1f - Mathf.Clamp01(5f * Time.deltaTime);
			if (m_postCollectTimer > 0.5f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	public UpgradeType GetUpgradeType()
	{
		return m_upgradeType;
	}

	private void DoUpgrade()
	{
		m_isCollected = true;
		m_taggable.SetTaggable(taggable: false);
		UnityEngine.Object.Destroy(base.gameObject.btransform());
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetCriticalFlash().DoUpgradeFlash();
		switch (m_upgradeType)
		{
		case UpgradeType.Points:
			Singleton<EndlessModeGameFlow>.Instance.AddScore(m_scoreValue);
			Singleton<EndlessModeController>.Instance.SpawnPickUpMessage(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endless_mode_pickup_points"), m_scoreValue);
			break;
		case UpgradeType.XPUpgrade:
		{
			List<CrewSpawner.CrewmanAvatarPairing> allCrew2 = Singleton<CrewSpawner>.Instance.GetAllCrew();
			foreach (CrewSpawner.CrewmanAvatarPairing item in allCrew2)
			{
				if (!item.m_crewman.IsDead())
				{
					int level = item.m_crewman.GetPrimarySkill().GetLevel();
					int xPRequiredForLevel = Singleton<XPRequirements>.Instance.GetXPRequiredForLevel(level);
					item.m_crewman.GetPrimarySkill().AddXP(xPRequiredForLevel + 1);
				}
			}
			Singleton<EndlessModeController>.Instance.SpawnPickUpMessage(m_XPUpgradeSpriteName, Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_XPUpgradeNamedText));
			Singleton<EndlessModeController>.Instance.CollectedLevelUp();
			break;
		}
		case UpgradeType.HealCrew:
		{
			List<CrewSpawner.CrewmanAvatarPairing> allCrew = Singleton<CrewSpawner>.Instance.GetAllCrew();
			foreach (CrewSpawner.CrewmanAvatarPairing item2 in allCrew)
			{
				if (!item2.m_crewman.IsDead())
				{
					if (item2.m_spawnedAvatar.GetHealthState().IsCountingDown())
					{
						item2.m_spawnedAvatar.GetHealthState().Heal(item2.m_crewman);
					}
					else
					{
						item2.m_spawnedAvatar.GetHealthState().HealFractional(item2.m_crewman, 1f);
					}
				}
			}
			Singleton<EndlessModeController>.Instance.SpawnPickUpMessage(m_healCrewSpriteName, Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_healCrewNamedText));
			break;
		}
		case UpgradeType.Repair:
		{
			int engineCount = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngineCount();
			for (int j = 0; j < engineCount; j++)
			{
				Engine engine = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngine(j);
				if (engine != null && !engine.NeedsRepair())
				{
					Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngine(j).UndoDamage(0.25f);
				}
			}
			BomberFuselageSection[] componentsInChildren = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetComponentsInChildren<BomberFuselageSection>();
			BomberFuselageSection[] array2 = componentsInChildren;
			foreach (BomberFuselageSection bomberFuselageSection in array2)
			{
				bomberFuselageSection.UndoDamage(0.125f);
			}
			Singleton<BomberSpawn>.Instance.GetBomberSystems().GetDamageMask().UndoDamage();
			Singleton<EndlessModeController>.Instance.SpawnPickUpMessage(m_repairSpriteName, Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_repairNamedText));
			break;
		}
		case UpgradeType.FuelRefill:
		{
			FuelTank[] fuelTanks = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetFuelTanks();
			FuelTank[] array = fuelTanks;
			foreach (FuelTank fuelTank in array)
			{
				fuelTank.PumpFuelIn(6000f);
			}
			Singleton<EndlessModeController>.Instance.SpawnPickUpMessage(m_fuelRefillSpriteName, Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_fuelRefillNamedText));
			break;
		}
		}
	}
}
