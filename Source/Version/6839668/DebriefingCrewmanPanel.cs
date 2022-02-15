using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using Common;
using UnityEngine;
using WingroveAudio;

public class DebriefingCrewmanPanel : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_nameText;

	[SerializeField]
	private string m_nameFormat = "{0} {1}";

	[SerializeField]
	private TextSetter m_rankText;

	[SerializeField]
	private CrewmanDualSkillDisplay m_skillsDisplay;

	[SerializeField]
	private GameObject m_medalHierarchy;

	[SerializeField]
	private GameObject m_kiaHierarchy;

	[SerializeField]
	private GameObject m_miaHierarchy;

	[SerializeField]
	private GameObject m_rescuedHierarchy;

	[SerializeField]
	private GameObject m_returnedHierarchy;

	[SerializeField]
	private float m_statusDelay;

	[SerializeField]
	[AudioEventName]
	private string m_kiaAudioEvent;

	[SerializeField]
	[AudioEventName]
	private string m_progressUpStart;

	[SerializeField]
	[AudioEventName]
	private string m_progressUpStop;

	[SerializeField]
	[AudioParameterName]
	private string m_progressNormalised;

	[SerializeField]
	[AudioEventName]
	private string m_progressUpDing;

	[SerializeField]
	[AudioEventName]
	private string m_progressUpEnd;

	[SerializeField]
	private float m_minLevelUpTime = 0.1f;

	[SerializeField]
	private float m_maxLevelUpTime = 4f;

	[SerializeField]
	private int m_xpPerSecond;

	[SerializeField]
	private GameObject m_newSkillPrefab;

	[SerializeField]
	private Transform m_newSkillLocation;

	[SerializeField]
	private Transform m_newSkillLocationSecondary;

	[SerializeField]
	private GameObject m_selectedHierarchy;

	[SerializeField]
	private LayoutGrid m_newSkillLayoutGrid;

	[SerializeField]
	private LayoutGrid m_newSkillLayoutGridSecondary;

	[SerializeField]
	private Renderer m_portraitRenderer;

	private Crewman m_crewman;

	private bool m_isMIA;

	private bool m_isRescued;

	private bool m_levelUpSequenceIsCompleted;

	private int m_progressAudioParamCached;

	public void SetUp(Crewman crewman)
	{
		m_crewman = crewman;
		m_rankText.SetText(m_crewman.GetCrewmanRankTranslated());
		m_nameText.SetText(string.Format(m_nameFormat, m_crewman.GetFirstName(), m_crewman.GetSurname()));
		m_skillsDisplay.SetUp(crewman);
		m_medalHierarchy.SetActive(value: false);
		m_portraitRenderer.material = Object.Instantiate(m_portraitRenderer.sharedMaterial);
		m_portraitRenderer.material.mainTexture = m_crewman.GetPortraitPictureTexture();
		m_progressAudioParamCached = WingroveRoot.Instance.GetParameterId(m_progressNormalised);
	}

	public void SetStatus(bool isMIA, bool isRecovered)
	{
		m_isMIA = isMIA;
		m_isRescued = isRecovered;
	}

	public void SetSelected(bool selected)
	{
		m_selectedHierarchy.CustomActivate(selected);
	}

	public IEnumerator ShowStatus(DebriefingScreen db)
	{
		if (m_crewman.IsDead() && !m_isMIA)
		{
			m_kiaHierarchy.CustomActivate(active: true);
			WingroveRoot.Instance.PostEvent(m_kiaAudioEvent);
		}
		else if (m_isMIA)
		{
			m_miaHierarchy.CustomActivate(active: true);
			WingroveRoot.Instance.PostEvent(m_kiaAudioEvent);
		}
		else if (m_isRescued)
		{
			m_rescuedHierarchy.CustomActivate(active: true);
			WingroveRoot.Instance.PostEvent(m_kiaAudioEvent);
		}
		else
		{
			m_returnedHierarchy.CustomActivate(active: true);
			WingroveRoot.Instance.PostEvent(m_kiaAudioEvent);
		}
		yield return StartCoroutine(db.DoSkipWait(1f));
	}

	public void HideStatusIfAlive()
	{
		if (!m_crewman.IsDead() && !m_isMIA)
		{
			m_rescuedHierarchy.CustomActivate(active: false);
			m_returnedHierarchy.CustomActivate(active: false);
		}
	}

	public void DoShowLevelUp(int xpToAdd, DebriefingScreen db)
	{
		m_levelUpSequenceIsCompleted = false;
		StartCoroutine(ShowLevelUp(xpToAdd, db));
	}

	public bool LevelUpSequenceIsCompleted()
	{
		if (m_crewman.IsDead())
		{
			m_levelUpSequenceIsCompleted = true;
		}
		return m_levelUpSequenceIsCompleted;
	}

	public IEnumerator ShowLevelUp(int xpToAdd, DebriefingScreen db)
	{
		if (m_crewman.IsDead() || m_isMIA)
		{
			yield break;
		}
		float xpTime = Mathf.Clamp((float)xpToAdd / (float)m_xpPerSecond, m_minLevelUpTime, m_maxLevelUpTime);
		float xpPerSecond = (float)xpToAdd / xpTime;
		int xpCtr = 0;
		WingroveRoot.Instance.PostEvent(m_progressUpStart);
		WingroveRoot.Instance.SetParameterGlobal(m_progressAudioParamCached, 0f);
		while (xpCtr < xpToAdd)
		{
			int numToLevelUp = 0;
			bool levelUpPrimary = false;
			bool levelUpSecondary = false;
			if (m_crewman.GetPrimarySkill().GetLevel() != m_crewman.GetPrimarySkill().GetMaxLevel())
			{
				numToLevelUp++;
				levelUpPrimary = true;
			}
			if (m_crewman.GetSecondarySkill() != null && m_crewman.GetSecondarySkill().GetLevel() != m_crewman.GetSecondarySkill().GetMaxLevel())
			{
				numToLevelUp++;
				levelUpSecondary = true;
			}
			int levelPrimary = m_crewman.GetPrimarySkill().GetLevel();
			int levelSecondary = ((m_crewman.GetSecondarySkill() != null) ? m_crewman.GetSecondarySkill().GetLevel() : 0);
			if (numToLevelUp != 0)
			{
				float num = ((!db.ShouldFastForward()) ? Time.deltaTime : (Time.deltaTime * 5f));
				int num2 = Mathf.Min(Mathf.CeilToInt(xpPerSecond * num) * 2, xpToAdd - xpCtr);
				int num3 = num2 / numToLevelUp;
				int xpToAdd2 = num2 - num3;
				if (levelUpPrimary)
				{
					m_crewman.GetPrimarySkill().AddXP(num3);
				}
				if (levelUpSecondary)
				{
					if (!levelUpPrimary)
					{
						m_crewman.GetSecondarySkill().AddXP(num3);
					}
					else
					{
						m_crewman.GetSecondarySkill().AddXP(xpToAdd2);
					}
				}
				xpCtr += num2;
			}
			m_skillsDisplay.Refresh();
			if (levelPrimary != m_crewman.GetPrimarySkill().GetLevel())
			{
				WingroveRoot.Instance.PostEvent(m_progressUpStop);
				WingroveRoot.Instance.PostEvent(m_progressUpDing);
				List<CrewmanSkillAbilityBool> newSkills2 = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetSkillsUnlockedExactlyAt(m_crewman.GetPrimarySkill());
				if (newSkills2.Count > 0)
				{
					HideStatusIfAlive();
				}
				foreach (CrewmanSkillAbilityBool item in newSkills2)
				{
					GameObject gameObject = Object.Instantiate(m_newSkillPrefab);
					gameObject.transform.parent = m_newSkillLocation;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.GetComponent<LevelUpNewAbilityDisplay>().SetFromSkill(item, m_crewman.GetPrimarySkill().GetSkill());
					db.AddNewSkill(m_crewman.GetPrimarySkill().GetSkill(), item);
				}
				m_newSkillLayoutGrid.RepositionChildren();
				yield return new WaitForSeconds(0.4f);
				WingroveRoot.Instance.PostEvent(m_progressUpStart);
			}
			if (m_crewman.GetSecondarySkill() != null && levelSecondary != m_crewman.GetSecondarySkill().GetLevel())
			{
				WingroveRoot.Instance.PostEvent(m_progressUpStop);
				WingroveRoot.Instance.PostEvent(m_progressUpDing);
				List<CrewmanSkillAbilityBool> newSkills = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetSkillsUnlockedExactlyAt(m_crewman.GetSecondarySkill());
				if (newSkills.Count > 0)
				{
					HideStatusIfAlive();
				}
				foreach (CrewmanSkillAbilityBool item2 in newSkills)
				{
					GameObject gameObject2 = Object.Instantiate(m_newSkillPrefab);
					gameObject2.transform.parent = m_newSkillLocationSecondary;
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.GetComponent<LevelUpNewAbilityDisplay>().SetFromSkill(item2, m_crewman.GetSecondarySkill().GetSkill());
					db.AddNewSkill(m_crewman.GetSecondarySkill().GetSkill(), item2);
				}
				m_newSkillLayoutGridSecondary.RepositionChildren();
				yield return StartCoroutine(db.DoSkipWait(0.4f));
				WingroveRoot.Instance.PostEvent(m_progressUpStart);
			}
			if (numToLevelUp == 0)
			{
				break;
			}
			WingroveRoot.Instance.SetParameterGlobal(m_progressAudioParamCached, m_crewman.GetPrimarySkill().GetXPNormalised());
			yield return null;
		}
		WingroveRoot.Instance.PostEvent(m_progressUpStop);
		WingroveRoot.Instance.PostEvent(m_progressUpEnd);
		yield return new WaitForSeconds(0.5f);
		m_levelUpSequenceIsCompleted = true;
	}
}
