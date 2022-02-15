using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class FighterCoordinator : Singleton<FighterCoordinator>
{
	[Serializable]
	public class FighterType
	{
		[SerializeField]
		public string m_fighterId;

		[SerializeField]
		public GameObject m_prefab;
	}

	public enum AttackPositions
	{
		Tailing = 1,
		Above = 2,
		Below = 4,
		InFront = 8,
		Left = 0x10,
		Right = 0x20,
		AboveRight = 0x40,
		AboveLeft = 0x80,
		BelowLeft = 0x100,
		BelowRight = 0x200,
		InFrontRight = 0x400,
		InFrontLeft = 0x800
	}

	[SerializeField]
	private FighterDifficultySettings m_difficultySettings;

	[SerializeField]
	private FighterDifficultySettings m_difficultySettingsConsole;

	[SerializeField]
	private FighterType[] m_fighterTypePrefabs;

	private List<FighterPlane> m_activeFighters = new List<FighterPlane>(32);

	private List<FighterWing> m_allWings = new List<FighterWing>(16);

	private List<FighterPlane> m_currentlyEngagedGangUpList = new List<FighterPlane>(16);

	private int m_attackPositionTaken;

	private FighterDifficultySettings.DifficultySetting m_difficultySetting;

	private int m_numV1sActive;

	private int m_numV1sHit;

	private int m_totalV1s;

	private int m_maxGangUp;

	private List<FighterType> m_fighterTypePrefabsTotal = new List<FighterType>(16);

	private void Awake()
	{
		m_fighterTypePrefabsTotal.AddRange(m_fighterTypePrefabs);
		foreach (string currentInstalledDLC in Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs())
		{
			DLCExpansionCampaign dLCExpansionCampaign = Singleton<DLCManager>.Instance.LoadAssetFromBundle<DLCExpansionCampaign>(currentInstalledDLC, "CAMPAIGN_" + currentInstalledDLC);
			if (dLCExpansionCampaign != null && dLCExpansionCampaign.GetExtraFighters() != null)
			{
				m_fighterTypePrefabsTotal.AddRange(dLCExpansionCampaign.GetExtraFighters().GetExtraFighters());
			}
		}
	}

	private void Update()
	{
		foreach (FighterWing allWing in m_allWings)
		{
			allWing.UpdateCachedValues();
		}
	}

	public void DeRegisterWing(FighterWing fw)
	{
		m_allWings.Remove(fw);
	}

	public GameObject GetFighterForId(string id)
	{
		foreach (FighterType item in m_fighterTypePrefabsTotal)
		{
			if (item.m_fighterId == id)
			{
				return item.m_prefab;
			}
		}
		return m_fighterTypePrefabs[0].m_prefab;
	}

	public void RegisterForAttackPosition(AttackPositions ap)
	{
		m_attackPositionTaken |= (int)ap;
	}

	public void FreeUpAttackPosition(AttackPositions ap)
	{
		m_attackPositionTaken &= (int)(~ap);
	}

	public bool IsAttackPositionAvailable(AttackPositions ap)
	{
		return ((uint)ap & (uint)m_attackPositionTaken) == 0;
	}

	public string GetDefaultFighterId()
	{
		return m_fighterTypePrefabs[0].m_fighterId;
	}

	public FighterDifficultySettings.DifficultySetting GetCurrentDifficulty()
	{
		if (m_difficultySetting == null)
		{
			if (Singleton<GameFlow>.Instance.GetGameMode().UseEndlessDifficulty())
			{
				m_difficultySetting = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode().GetDifficulty();
			}
			else
			{
				m_difficultySetting = m_difficultySettings.GetCurrentDifficultySetting();
			}
		}
		return m_difficultySetting;
	}

	public void RegisterFigher(FighterPlane fp)
	{
		m_activeFighters.Add(fp);
		if (fp.IsV1())
		{
			m_numV1sActive++;
		}
	}

	public void RegisterV1Only()
	{
		m_totalV1s++;
	}

	public void DeregisterFighter(FighterPlane fp)
	{
		m_activeFighters.Remove(fp);
		if (fp.IsV1())
		{
			m_numV1sActive--;
		}
	}

	public void SetV1HitLondon()
	{
		m_numV1sHit++;
	}

	public int GetV1sHitLondon()
	{
		return m_numV1sHit;
	}

	public int GetV1sActive()
	{
		return m_numV1sActive;
	}

	public int GetV1sTotal()
	{
		return m_totalV1s;
	}

	public List<FighterPlane> GetAllFighters()
	{
		return m_activeFighters;
	}

	public void RegisterWing(FighterWing fw)
	{
		m_allWings.Add(fw);
	}

	public List<FighterWing> GetAllWings()
	{
		return m_allWings;
	}

	public bool AreAnyFightersEngaged()
	{
		foreach (FighterPlane activeFighter in m_activeFighters)
		{
			if (activeFighter.GetAI().IsEngaged())
			{
				return true;
			}
		}
		return false;
	}

	public bool AreAnyFightersTagged()
	{
		foreach (FighterPlane activeFighter in m_activeFighters)
		{
			if (activeFighter.IsTagged())
			{
				return true;
			}
		}
		return false;
	}

	private void Start()
	{
		m_maxGangUp = GetCurrentDifficulty().m_gangUpMaximum;
	}

	public bool GetAllowedAttack(FighterPlane fp)
	{
		while (m_currentlyEngagedGangUpList.Contains(null))
		{
			m_currentlyEngagedGangUpList.Remove(null);
		}
		if (m_currentlyEngagedGangUpList.Contains(fp))
		{
			return true;
		}
		if (m_currentlyEngagedGangUpList.Count < m_maxGangUp)
		{
			m_currentlyEngagedGangUpList.Add(fp);
			return true;
		}
		return false;
	}

	public void SetNoLongerAttacking(FighterPlane fp)
	{
		m_currentlyEngagedGangUpList.Remove(fp);
	}

	public Vector3 GetAvoidingPosition(FighterPlane thisFp, Vector3 startPos, Vector3 targPos)
	{
		bool flag = false;
		float b = 1f;
		foreach (FighterPlane activeFighter in m_activeFighters)
		{
			if (activeFighter != thisFp)
			{
				Vector3 vector = activeFighter.transform.position - startPos;
				Vector3 vector2 = targPos - startPos;
				Vector3 vector3 = activeFighter.transform.position - targPos;
				if (vector.magnitude < vector2.magnitude && (Vector3.Dot(vector.normalized, vector2.normalized) > 0.98f || vector3.magnitude < 50f || vector.magnitude < 20f))
				{
					b = Mathf.Min(vector.magnitude / vector2.magnitude, b);
					flag = true;
				}
			}
		}
		if (flag)
		{
			Vector3 vector4 = Vector3.Cross((targPos - startPos).normalized, Vector3.up);
			vector4.y = 0f;
			return targPos + vector4 * 2f;
		}
		return targPos;
	}

	public bool AreAnyUntaggedFightersEngaged()
	{
		foreach (FighterPlane activeFighter in m_activeFighters)
		{
			if (activeFighter.GetAI().IsEngaged() && !activeFighter.IsTagged())
			{
				return true;
			}
		}
		return false;
	}

	public int GetNumTaggableCurrently()
	{
		int num = 0;
		foreach (FighterPlane activeFighter in m_activeFighters)
		{
			if (activeFighter != null && !activeFighter.IsTagged() && activeFighter.GetComponent<TaggableFighter>().IsTaggable())
			{
				num++;
			}
		}
		return num;
	}

	public int GetNumTaggedCurrently()
	{
		int num = 0;
		foreach (FighterPlane activeFighter in m_activeFighters)
		{
			if (activeFighter != null && activeFighter.IsTagged())
			{
				num++;
			}
		}
		return num;
	}

	public FighterPlane GetCurrentTargetIgnoreTagged(FighterPlane previous, float minDistance, Transform baseOfGun, float fieldOfViewDegrees, float maxDistance)
	{
		FighterPlane fighterPlane = null;
		foreach (FighterPlane activeFighter in m_activeFighters)
		{
			if (!(activeFighter != null) || activeFighter.IsHighPriority())
			{
				continue;
			}
			Vector3 vector = activeFighter.GetAimAtPosition() - baseOfGun.position;
			Vector3 forward = baseOfGun.forward;
			float magnitude = (activeFighter.GetAimAtPosition() - baseOfGun.position).magnitude;
			float num = ((!(fighterPlane == null)) ? (fighterPlane.transform.position - baseOfGun.position).magnitude : float.MaxValue);
			float num2 = ((!(activeFighter == previous)) ? magnitude : (magnitude * 0.65f));
			float num3 = ((!(fighterPlane == null)) ? Vector3.Dot((fighterPlane.transform.position - baseOfGun.position).normalized, forward.normalized) : 0f);
			float num4 = Vector3.Dot(vector.normalized, forward.normalized);
			float num5 = ((!(activeFighter == previous)) ? num4 : (num4 * 1.3f));
			bool flag = false;
			if (magnitude < maxDistance)
			{
				if (fighterPlane == null)
				{
					if (magnitude > minDistance)
					{
						flag = true;
					}
				}
				else if (num2 < num && num5 > num3)
				{
					flag = true;
				}
				else if (num2 < num && num4 > num3)
				{
					flag = true;
				}
				else if (magnitude < num && num5 > num3)
				{
					flag = true;
				}
				else if (num3 < 0f && num4 > 0f)
				{
					flag = true;
				}
			}
			if (flag)
			{
				fighterPlane = activeFighter;
			}
		}
		return fighterPlane;
	}

	public FighterPlane GetCurrentTarget(FighterPlane previous, float minDistance, Transform baseOfGun, float fieldOfViewDegrees, out bool foundFighter)
	{
		FighterPlane fighterPlane = null;
		foundFighter = false;
		Vector3 normalized = baseOfGun.forward.normalized;
		Vector3 position = baseOfGun.position;
		float num = 0f;
		float num2 = 0f;
		foreach (FighterPlane activeFighter in m_activeFighters)
		{
			if (!activeFighter.IsTagged())
			{
				continue;
			}
			Vector3 vector = activeFighter.GetAimAtPosition() - position;
			float magnitude = (activeFighter.GetAimAtPosition() - position).magnitude;
			float num3 = (foundFighter ? num2 : float.MaxValue);
			float num4 = ((!(activeFighter == previous)) ? magnitude : (magnitude * 0.65f));
			float num5 = (foundFighter ? num : 0f);
			float num6 = Vector3.Dot(vector.normalized, normalized);
			float num7 = ((!(activeFighter == previous)) ? num6 : (num6 * 1.3f));
			bool flag = foundFighter && fighterPlane.IsHighPriority();
			bool flag2 = activeFighter.IsHighPriority();
			bool flag3 = false;
			if (!foundFighter)
			{
				if (magnitude > minDistance)
				{
					flag3 = true;
				}
			}
			else if (num4 < num3 && num7 > num5 && !flag)
			{
				flag3 = true;
			}
			else if (num4 < num3 && num6 > num5 && !flag)
			{
				flag3 = true;
			}
			else if (magnitude < num3 && num7 > num5 && !flag)
			{
				flag3 = true;
			}
			else if (num5 < 0f && num6 > 0f)
			{
				flag3 = true;
			}
			else if (num6 > 0f && flag2)
			{
				flag3 = true;
			}
			if (flag3)
			{
				foundFighter = true;
				fighterPlane = activeFighter;
				num2 = vector.magnitude;
				num = Vector3.Dot(vector.normalized, normalized);
			}
		}
		return fighterPlane;
	}
}
