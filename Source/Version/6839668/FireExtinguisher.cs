using System.Collections.Generic;
using AudioNames;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class FireExtinguisher : MonoBehaviour
{
	[SerializeField]
	private float m_firePutOutRadius;

	[SerializeField]
	private float m_capacityMax;

	[SerializeField]
	private float m_standardEffiency = 0.5f;

	[SerializeField]
	private float m_maxEfficiency = 1.5f;

	[SerializeField]
	private float m_putOutRange = 1.5f;

	[SerializeField]
	private CrewmanSkillAbilityFloat m_effiencySkill;

	[SerializeField]
	private GameObject m_debugObject;

	private float m_capacity;

	private bool m_isOn;

	private Crewman m_currentCrewman;

	private int m_cachedGameObjectId;

	private CrewmanAvatar m_carriedCrewman;

	private string m_effiencySkillId;

	private void Awake()
	{
		m_cachedGameObjectId = base.gameObject.GetInstanceID();
		m_capacity = m_capacityMax;
		GetComponent<CarryableItem>().OnDropped += OnDropped;
		GetComponent<CarryableItem>().OnPickedUp += OnPickedUp;
		m_effiencySkillId = m_effiencySkill.GetCachedName();
	}

	private void Start()
	{
		if (Singleton<BomberSpawn>.Instance != null)
		{
			Singleton<BomberSpawn>.Instance.GetBomberSystems().RegisterInteractiveSearchable(typeof(FireExtinguisher), GetComponent<InteractiveItem>());
		}
	}

	private void OnDisable()
	{
		if (WingroveRoot.Instance != null)
		{
			WingroveRoot.Instance.PostEventGO("EXTINGUISH_STOP", base.gameObject);
		}
	}

	private void OnDropped()
	{
		DoExtinguish(extinguish: false, null);
	}

	private void OnPickedUp(CrewmanAvatar ca)
	{
		m_carriedCrewman = ca;
	}

	public void DoExtinguish(bool extinguish, Crewman fromCrewman)
	{
		m_currentCrewman = fromCrewman;
		if (m_isOn != extinguish)
		{
			if (!extinguish)
			{
				WingroveRoot.Instance.PostEventGO("EXTINGUISH_STOP", base.gameObject);
			}
			else
			{
				WingroveRoot.Instance.PostEventGO("EXTINGUISH_START", base.gameObject);
			}
		}
		m_isOn = extinguish;
	}

	public float GetCapacity()
	{
		return m_capacity;
	}

	private void Update()
	{
		if (!m_isOn)
		{
			return;
		}
		m_debugObject.transform.position = base.transform.position + m_carriedCrewman.transform.forward * m_putOutRange;
		List<FireOverview.Extinguishable> firesWithin = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetFireOverview().GetFiresWithin(base.transform.position + m_carriedCrewman.transform.forward * m_putOutRange, m_firePutOutRadius);
		float proficiency = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetProficiency(m_effiencySkillId, m_currentCrewman);
		float num = Mathf.Lerp(m_standardEffiency, m_maxEfficiency, proficiency);
		foreach (FireOverview.Extinguishable item in firesWithin)
		{
			if (item.IsOnFire())
			{
				item.PutOutFire(num * Time.deltaTime);
				m_capacity -= Time.deltaTime;
			}
		}
		WingroveRoot.Instance.SetParameterForObject(GameEvents.Parameters.CacheVal_ExtinguisherCapacity(), m_cachedGameObjectId, base.gameObject, Mathf.Clamp01(m_capacity / m_capacityMax));
	}
}
