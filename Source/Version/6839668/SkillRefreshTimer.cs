using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class SkillRefreshTimer : MonoBehaviour
{
	public class SkillTimer
	{
		private float m_inUseTimer;

		private float m_inUseStart;

		private float m_rechargingTimer;

		private float m_rechargeStart;

		private bool m_isActive;

		private object m_associatedSkillObject;

		private bool m_isDone;

		private bool m_rechargedFlag;

		public void Start(float useTime, float rechargeTime)
		{
			m_inUseTimer = (m_inUseStart = useTime);
			m_rechargingTimer = (m_rechargeStart = rechargeTime);
			m_isActive = true;
			m_isDone = false;
		}

		public bool IsDone()
		{
			return m_isDone;
		}

		public void ResetDoneFlag()
		{
			m_isDone = false;
		}

		public bool IsRecharged()
		{
			return m_rechargedFlag;
		}

		public void ResetRechargedFlag()
		{
			m_rechargedFlag = false;
		}

		public bool CanStart()
		{
			return !m_isActive && m_rechargingTimer <= 0f;
		}

		public void FinishEarly()
		{
			if (m_isActive)
			{
				m_isDone = true;
			}
			m_isActive = false;
			m_inUseTimer = 0f;
		}

		public bool IsActive()
		{
			return m_isActive;
		}

		public void Tick()
		{
			if (m_isActive)
			{
				m_inUseTimer -= Time.deltaTime;
			}
			else if (m_rechargingTimer > 0f)
			{
				m_rechargingTimer -= Time.deltaTime;
				if (m_rechargingTimer <= 0f)
				{
					m_rechargingTimer = 0f;
					m_rechargedFlag = true;
				}
			}
			if (m_inUseTimer < 0f)
			{
				m_isActive = false;
				m_inUseTimer = 0f;
				m_isDone = true;
			}
			if (m_rechargingTimer <= 0f)
			{
				m_rechargingTimer = 0f;
			}
		}

		public float GetInUseNormalised()
		{
			if (m_inUseStart == 0f)
			{
				return 0f;
			}
			return Mathf.Clamp01(m_inUseTimer / m_inUseStart);
		}

		public float GetProgressNormalised()
		{
			if (m_inUseStart == 0f)
			{
				return 0f;
			}
			return 1f - Mathf.Clamp01(m_inUseTimer / m_inUseStart);
		}

		public float GetRechargeNormalised()
		{
			if (m_rechargeStart == 0f)
			{
				return 0f;
			}
			return 1f - Mathf.Clamp01(m_rechargingTimer / m_rechargeStart);
		}

		public object GetAssociatedObject()
		{
			return m_associatedSkillObject;
		}

		public void DoRechargedSpeech(string skillDescription, CrewmanAvatar cm, CrewmanSkillAbilityBool skill)
		{
			if (IsRecharged())
			{
				if (skill == null || Singleton<CrewmanSkillUpgradeInfo>.Instance.IsUnlocked(skill, cm.GetCrewman()))
				{
					Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.SkillRecharged, cm, skillDescription.ToUpper());
				}
				ResetRechargedFlag();
			}
		}
	}

	private List<SkillTimer> m_skillGroups = new List<SkillTimer>();

	public SkillTimer GetFor(object associatedObject)
	{
		SkillTimer skillTimer = null;
		foreach (SkillTimer skillGroup in m_skillGroups)
		{
			if (skillGroup.GetAssociatedObject() == associatedObject)
			{
				skillTimer = skillGroup;
				break;
			}
		}
		if (skillTimer == null)
		{
			SkillTimer item = new SkillTimer();
			m_skillGroups.Add(item);
		}
		return skillTimer;
	}

	public SkillTimer CreateNew()
	{
		SkillTimer skillTimer = new SkillTimer();
		m_skillGroups.Add(skillTimer);
		return skillTimer;
	}

	public bool AreAnySkillsActive()
	{
		foreach (SkillTimer skillGroup in m_skillGroups)
		{
			if (skillGroup.IsActive())
			{
				return true;
			}
		}
		return false;
	}

	public bool AreAnySkillsActive(SkillTimer exclude)
	{
		foreach (SkillTimer skillGroup in m_skillGroups)
		{
			if (exclude != skillGroup && skillGroup.IsActive())
			{
				return true;
			}
		}
		return false;
	}

	private void Update()
	{
		foreach (SkillTimer skillGroup in m_skillGroups)
		{
			skillGroup.Tick();
		}
	}
}
