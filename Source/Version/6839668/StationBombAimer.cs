using System;
using BomberCrewCommon;
using UnityEngine;

public class StationBombAimer : Station
{
	[SerializeField]
	private SkillRefreshTimer m_skillRefreshTimer;

	[SerializeField]
	private Material m_bombstationCompositorMaterial;

	[SerializeField]
	private float m_invAlphaPerAltitudeMinSkill;

	[SerializeField]
	private float m_invAlphaPerAltitudeMaxSkill;

	[SerializeField]
	private float m_alphaCutMinSkill;

	[SerializeField]
	private float m_alphaCutMaxSkill;

	[SerializeField]
	private CrewmanSkillAbilityFloat m_bombAimingSkill;

	[SerializeField]
	private float m_frequency = 0.2f;

	[SerializeField]
	private float m_alphaMaxMinSkill;

	[SerializeField]
	private float m_alphaMaxMaxSkill;

	private InMissionNotification m_hydraulicsNotification;

	private SkillRefreshTimer.SkillTimer m_takePhotoTimer;

	private float m_t;

	private string m_bombAimingSkillId;

	private new void Start()
	{
		m_takePhotoTimer = m_skillRefreshTimer.CreateNew();
		m_bombAimingSkillId = m_bombAimingSkill.GetCachedName();
	}

	public SkillRefreshTimer.SkillTimer GetPhotoTimer()
	{
		return m_takePhotoTimer;
	}

	private float CalculateAlpha(CrewmanAvatar ca)
	{
		if (ca == null)
		{
			return 0f;
		}
		float proficiency = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetProficiency(m_bombAimingSkillId, ca.GetCrewman());
		if (proficiency == 0f)
		{
			return 0f;
		}
		float num = Mathf.Sin(m_t * (float)Math.PI * 2f) * 0.5f + 0.5f;
		float num2 = 1f - Mathf.Lerp(m_alphaCutMinSkill, m_alphaCutMaxSkill, proficiency);
		float num3 = Mathf.Lerp(m_alphaMaxMinSkill, m_alphaMaxMaxSkill, proficiency);
		float num4 = (num - num2) / ((num2 != 1f) ? (1f - num2) : 1f) * num3;
		float altitudeAboveGround = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetAltitudeAboveGround();
		float num5 = Mathf.Lerp(m_invAlphaPerAltitudeMinSkill, m_invAlphaPerAltitudeMaxSkill, proficiency);
		return Mathf.Clamp01(num4 - num5 * altitudeAboveGround);
	}

	private void Update()
	{
		m_t += Time.deltaTime * m_frequency;
		while (m_t > 1f)
		{
			m_t -= 1f;
		}
		m_bombstationCompositorMaterial.SetFloat("_Multiplier", CalculateAlpha(GetCurrentCrewman()));
		BomberSystems bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		if (bomberSystems.GetBombBayDoors().IsAffectedByHydraulics())
		{
			if (m_hydraulicsNotification == null)
			{
				m_hydraulicsNotification = new InMissionNotification("notification_hydraulics_out", StationNotificationUrgency.Red, EnumToIconMapping.InteractionOrAlertType.Hydraulics);
				GetNotifications().RegisterNotification(m_hydraulicsNotification);
			}
		}
		else if (m_hydraulicsNotification != null)
		{
			GetNotifications().RemoveNotification(m_hydraulicsNotification);
			m_hydraulicsNotification = null;
		}
		if (GetCurrentCrewman() != null)
		{
			m_takePhotoTimer.DoRechargedSpeech(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_bomber_station_button_takephoto"), GetCurrentCrewman(), null);
		}
	}
}
