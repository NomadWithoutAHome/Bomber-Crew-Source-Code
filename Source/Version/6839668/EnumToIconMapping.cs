using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class EnumToIconMapping : Singleton<EnumToIconMapping>
{
	public enum InteractionOrAlertType
	{
		Main,
		Heal,
		Extinguish,
		Repair,
		Message,
		Navigate,
		BailOut,
		Pilot,
		Ammo,
		Fuel,
		Parachute,
		Drop,
		PickUp,
		Electrics,
		Hydraulics,
		Visibility,
		PutBack,
		Landing,
		Temperature,
		Oxygen,
		Photo,
		Repairing,
		PickUpExtinguisher,
		PickUpMedKit,
		PickUpAmmoBox,
		PickUpParachute
	}

	[Serializable]
	public class InteractionIcon
	{
		[SerializeField]
		public InteractionOrAlertType m_interaction;

		[SerializeField]
		public string m_spriteName;
	}

	[Serializable]
	public class SkillIcon
	{
		[SerializeField]
		public Crewman.SpecialisationSkill m_skillType;

		[SerializeField]
		public string m_spriteName;
	}

	[SerializeField]
	private InteractionIcon[] m_interactionIcons;

	[SerializeField]
	private GameObject m_interactionPrefab;

	[SerializeField]
	private SkillIcon[] m_skillIcons;

	private Dictionary<InteractionOrAlertType, string> m_interactionIconsDic = new Dictionary<InteractionOrAlertType, string>();

	private Dictionary<Crewman.SpecialisationSkill, string> m_skillIconsDic = new Dictionary<Crewman.SpecialisationSkill, string>();

	private bool m_isInitialised;

	public GameObject GetInteractionPrefab()
	{
		return m_interactionPrefab;
	}

	private void Init()
	{
		if (!m_isInitialised)
		{
			InteractionIcon[] interactionIcons = m_interactionIcons;
			foreach (InteractionIcon interactionIcon in interactionIcons)
			{
				m_interactionIconsDic[interactionIcon.m_interaction] = interactionIcon.m_spriteName;
			}
			SkillIcon[] skillIcons = m_skillIcons;
			foreach (SkillIcon skillIcon in skillIcons)
			{
				m_skillIconsDic[skillIcon.m_skillType] = skillIcon.m_spriteName;
			}
			m_isInitialised = true;
		}
	}

	private void Start()
	{
		if (!m_isInitialised)
		{
			Init();
		}
	}

	public string GetIconName(InteractionOrAlertType it)
	{
		if (!m_isInitialised)
		{
			Init();
		}
		string value = string.Empty;
		m_interactionIconsDic.TryGetValue(it, out value);
		if (string.IsNullOrEmpty(value))
		{
			value = m_interactionIcons[0].m_spriteName;
		}
		return value;
	}

	public string GetIconName(Crewman.SpecialisationSkill skill)
	{
		if (!m_isInitialised)
		{
			Init();
		}
		string value = string.Empty;
		m_skillIconsDic.TryGetValue(skill, out value);
		if (string.IsNullOrEmpty(value))
		{
			value = m_skillIcons[0].m_spriteName;
		}
		return value;
	}

	public string GetIconNameSmall(Crewman.SpecialisationSkill skill)
	{
		if (!m_isInitialised)
		{
			Init();
		}
		string value = string.Empty;
		m_skillIconsDic.TryGetValue(skill, out value);
		if (string.IsNullOrEmpty(value))
		{
			value = m_skillIcons[0].m_spriteName;
		}
		return value + "_Small";
	}
}
