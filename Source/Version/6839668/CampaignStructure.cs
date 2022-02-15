using System;
using System.Collections.Generic;
using System.Globalization;
using BomberCrewCommon;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Campaign Data")]
public class CampaignStructure : ScriptableObject
{
	[Serializable]
	public class CampaignMission
	{
		private class OrderedVector
		{
			public int m_order;

			public Vector2 m_vec;
		}

		[SerializeField]
		public string m_missionReferenceName;

		[SerializeField]
		public bool m_canRepeatIfFailed;

		[SerializeField]
		public bool m_canRepeatIfComplete;

		[SerializeField]
		public int m_failRepeatDelay;

		[SerializeField]
		public int m_completeRepeatDelay;

		[SerializeField]
		public bool m_isKeyMission;

		[SerializeField]
		public bool m_showKeyMissionTitle;

		[SerializeField]
		public string m_unlockOnCompleteTag;

		[SerializeField]
		public string m_unlockOnFinishTag;

		[SerializeField]
		[NamedText]
		public string m_titleNamedText;

		[SerializeField]
		[NamedText]
		public string m_descriptionNamedText;

		[SerializeField]
		public Texture2D m_targetPhotoTexture;

		[SerializeField]
		public int m_minIntelRequired;

		[SerializeField]
		public string m_tagRequired;

		[SerializeField]
		public LevelDescription m_level;

		[SerializeField]
		public BombLoadConfig m_bombLoadConfig;

		[SerializeField]
		public int m_totalIntelReward;

		[SerializeField]
		public int m_totalMoneyReward;

		[SerializeField]
		public bool m_isTrainingMission;

		[SerializeField]
		public bool m_isFinalMission;

		[SerializeField]
		public bool m_isFinalMissionOfDLC;

		[SerializeField]
		public CampaignPerk m_campaignPerk;

		[SerializeField]
		public int m_riskBase;

		[SerializeField]
		public float m_fuelBonus;

		[SerializeField]
		public int m_orderingNumber;

		public int GetRiskBalanced()
		{
			float num = GetAceChance() + Singleton<SaveDataContainer>.Instance.Get().GetAceChanceCounter();
			int num2 = m_riskBase;
			if ((m_isKeyMission && m_showKeyMissionTitle) || num >= 1f)
			{
				num2++;
			}
			int difficultyBump = Singleton<SaveDataContainer>.Instance.Get().GetDifficultyBump();
			if (difficultyBump == 1)
			{
				num2++;
			}
			if (difficultyBump == -2 && num2 > 0)
			{
				num2--;
			}
			return num2;
		}

		public Vector2 GetMissionTargetArea()
		{
			Vector2 result = Vector2.zero;
			bool flag = false;
			foreach (LevelDescription.LevelItemSerialized levelItem in m_level.m_levelItems)
			{
				if (!(levelItem.m_type == "MissionTarget") || flag)
				{
					continue;
				}
				result = new Vector2(levelItem.m_position.x, levelItem.m_position.z);
				foreach (LevelDescription.LevelParameter levelParameter in levelItem.m_levelParameters)
				{
					if (levelParameter.m_key == "MapDisplayPrimary" && levelParameter.m_value == "true")
					{
						flag = true;
					}
				}
			}
			return result;
		}

		public string GetDebugInfoDesc()
		{
			int num = 0;
			List<string> list = new List<string>();
			int num2 = 0;
			float num3 = 0f;
			bool flag = false;
			List<string> list2 = new List<string>();
			string text = string.Empty;
			string text2 = string.Empty;
			foreach (LevelDescription.LevelItemSerialized levelItem in m_level.m_levelItems)
			{
				if (levelItem.m_type == "ExtraStart_FM")
				{
					text = text + levelItem.m_type + "; ";
				}
				if (levelItem.m_type == "MissionConfig")
				{
					foreach (LevelDescription.LevelParameter levelParameter in levelItem.m_levelParameters)
					{
						if (levelParameter.m_key == "timeOfDay")
						{
							num3 = float.Parse(levelParameter.m_value, CultureInfo.InvariantCulture);
						}
					}
				}
				else if (levelItem.m_type == "AceFighterSpawn")
				{
					flag = true;
				}
				else if (levelItem.m_type == "PhotoTarget")
				{
					foreach (LevelDescription.LevelParameter levelParameter2 in levelItem.m_levelParameters)
					{
						if (levelParameter2.m_key == "optional")
						{
							text2 = ((!(levelParameter2.m_value == "true")) ? (text2 + "\nNON-OPTIONAL PHOTO") : (text2 + "\nOPTIONAL PHOTO"));
						}
					}
				}
				else
				{
					if (!(levelItem.m_type == "FighterWing"))
					{
						continue;
					}
					bool flag2 = false;
					if (string.IsNullOrEmpty(levelItem.m_spawnTrigger) || levelItem.m_spawnTrigger.StartsWith("BOMBTARGETS") || levelItem.m_spawnTrigger.StartsWith("OTHEROBJECTIVE"))
					{
						flag2 = true;
					}
					foreach (LevelDescription.LevelParameter levelParameter3 in levelItem.m_levelParameters)
					{
						if (levelParameter3.m_key == "numFighters")
						{
							if (flag2)
							{
								num += int.Parse(levelParameter3.m_value);
							}
							else
							{
								num2 += int.Parse(levelParameter3.m_value);
							}
						}
						if (!(levelParameter3.m_key == "groupType"))
						{
							continue;
						}
						List<string> list3 = ((!flag2) ? list2 : list);
						if (string.IsNullOrEmpty(levelParameter3.m_value))
						{
							if (!list3.Contains("Me109"))
							{
								list3.Add("Me109");
							}
							continue;
						}
						string[] array = levelParameter3.m_value.Split(",".ToCharArray());
						string[] array2 = array;
						foreach (string item in array2)
						{
							if (!list3.Contains(item))
							{
								list3.Add(item);
							}
						}
					}
				}
			}
			string text3 = "Main Spawn Fighters: " + num + "\n" + string.Join(",", list.ToArray());
			string text4 = text3;
			text3 = text4 + "\nOther Spawn Fighters: " + num2 + "\n" + string.Join(",", list2.ToArray());
			text3 = text3 + "\nTime Of Day: " + $"{num3:0.00}";
			text3 = text3 + "\nObsolete: " + text;
			if (flag)
			{
				text3 += "\nACE CHANCE!";
			}
			return text3 + text2;
		}

		public float GetAceChance()
		{
			float num = 0f;
			foreach (LevelDescription.LevelItemSerialized levelItem in m_level.m_levelItems)
			{
				if (!(levelItem.m_type == "AceFighterSpawn"))
				{
					continue;
				}
				foreach (LevelDescription.LevelParameter levelParameter in levelItem.m_levelParameters)
				{
					if (levelParameter.m_key == "chanceOfAppear")
					{
						num = Mathf.Max(float.Parse(levelParameter.m_value, CultureInfo.InvariantCulture), num);
					}
				}
			}
			return num;
		}

		public bool GetAceIsForced()
		{
			bool result = false;
			foreach (LevelDescription.LevelItemSerialized levelItem in m_level.m_levelItems)
			{
				if (!(levelItem.m_type == "AceFighterSpawn"))
				{
					continue;
				}
				foreach (LevelDescription.LevelParameter levelParameter in levelItem.m_levelParameters)
				{
					if (levelParameter.m_key == "forceId" && levelParameter.m_value != "0")
					{
						result = true;
					}
				}
			}
			return result;
		}

		public List<Vector2> GetMissionPath()
		{
			List<OrderedVector> list = new List<OrderedVector>();
			foreach (LevelDescription.LevelItemSerialized levelItem in m_level.m_levelItems)
			{
				if (!(levelItem.m_type == "MissionTarget"))
				{
					continue;
				}
				Vector2 vec = new Vector2(levelItem.m_position.x, levelItem.m_position.z);
				bool flag = true;
				int order = 0;
				foreach (LevelDescription.LevelParameter levelParameter in levelItem.m_levelParameters)
				{
					if (levelParameter.m_key == "MapDisplay" && levelParameter.m_value == "false")
					{
						flag = false;
					}
					if (levelParameter.m_key == "MapDisplayIndex")
					{
						order = int.Parse(levelParameter.m_value);
					}
				}
				if (flag)
				{
					OrderedVector orderedVector = new OrderedVector();
					orderedVector.m_order = order;
					orderedVector.m_vec = vec;
					list.Add(orderedVector);
				}
			}
			List<Vector2> list2 = new List<Vector2>();
			list.Sort((OrderedVector x, OrderedVector y) => Mathf.Clamp(y.m_order - x.m_order, -1, 2));
			foreach (OrderedVector item in list)
			{
				list2.Add(item.m_vec);
			}
			return list2;
		}
	}

	[SerializeField]
	private bool m_useSimplifiedMissionGet;

	[SerializeField]
	private bool m_mainCampaign;

	[SerializeField]
	private CampaignMission[] m_allMissions;

	public static float GetEstimatedTimeSeconds(LevelDescription ld)
	{
		float num = 80f;
		float num2 = 0f;
		Vector3 vector = Vector3.zero;
		int num3 = 0;
		foreach (LevelDescription.LevelItemSerialized levelItem in ld.m_levelItems)
		{
			if (levelItem.m_type.Contains("BombTarget"))
			{
				num3++;
			}
			if (levelItem.m_type == "MissionTarget")
			{
				num2 += (levelItem.m_position - vector).magnitude;
				vector = levelItem.m_position;
			}
		}
		float num4 = num2 / num;
		float num5 = vector.magnitude / num;
		return num4 + num5 + Mathf.Clamp(Mathf.Pow(num3, 2f), 1f, 5f) * 25f;
	}

	public CampaignMission[] GetAllMissions()
	{
		return m_allMissions;
	}

	private bool IsTooClose(CampaignMission cm, List<CampaignMission> others)
	{
		for (int i = 0; i < others.Count; i++)
		{
			if (((Vector3)(others[i].GetMissionTargetArea() - cm.GetMissionTargetArea())).magnitude < 300f)
			{
				return true;
			}
		}
		return false;
	}

	public List<CampaignMission> GetCurrentAvailableMissions(int count, int seed)
	{
		SaveData sd = Singleton<SaveDataContainer>.Instance.Get();
		int numMissionsPlayed = sd.GetNumMissionsPlayed();
		System.Random r = new System.Random(seed);
		if (m_useSimplifiedMissionGet)
		{
			bool flag = true;
			List<CampaignMission> list = new List<CampaignMission>();
			CampaignMission[] allMissions = m_allMissions;
			foreach (CampaignMission campaignMission in allMissions)
			{
				if (flag)
				{
					list.Add(campaignMission);
				}
				flag = sd.HasCompletedMission(campaignMission.m_missionReferenceName);
			}
			return list;
		}
		if ((sd.HasCompletedMission("C08_KEY") || sd.HasCompletedMission("DLCMP02_C05_KEY")) && m_mainCampaign)
		{
			List<CampaignMission> list2 = new List<CampaignMission>();
			CampaignMission[] allMissions2 = m_allMissions;
			foreach (CampaignMission campaignMission2 in allMissions2)
			{
				if (campaignMission2.m_isKeyMission && !campaignMission2.m_isTrainingMission && sd.GetLastPlayedTime(campaignMission2.m_missionReferenceName) < numMissionsPlayed - 1)
				{
					list2.Add(campaignMission2);
				}
			}
			while (list2.Count > 2)
			{
				list2.RemoveAt(r.Next(0, list2.Count));
			}
			while (list2.Count < 6)
			{
				CampaignMission campaignMission3 = m_allMissions[r.Next(0, m_allMissions.Length)];
				if (!list2.Contains(campaignMission3) && !campaignMission3.m_isTrainingMission && sd.GetLastPlayedTime(campaignMission3.m_missionReferenceName) < numMissionsPlayed - 1)
				{
					list2.Add(campaignMission3);
				}
			}
			List<CampaignMission> list3 = new List<CampaignMission>();
			for (int k = 0; k < list2.Count; k++)
			{
				for (int l = k; l < list2.Count; l++)
				{
					if (l != k && ((Vector3)(list2[k].GetMissionTargetArea() - list2[l].GetMissionTargetArea())).magnitude < 300f)
					{
						list3.Add(list2[k]);
					}
				}
			}
			while (list2.Count > 4)
			{
				list2.RemoveAt(r.Next(0, list2.Count));
			}
			return list2;
		}
		List<CampaignMission> list4 = new List<CampaignMission>();
		List<string> keysUnlocked = sd.GetKeysUnlocked(this);
		CampaignMission[] allMissions3 = m_allMissions;
		foreach (CampaignMission campaignMission4 in allMissions3)
		{
			if (!sd.HasPlayedMission(campaignMission4.m_missionReferenceName))
			{
				if (sd.GetIntel() >= campaignMission4.m_minIntelRequired)
				{
					if (string.IsNullOrEmpty(campaignMission4.m_tagRequired))
					{
						list4.Add(campaignMission4);
					}
					else if (keysUnlocked.Contains(campaignMission4.m_tagRequired))
					{
						list4.Add(campaignMission4);
					}
				}
			}
			else if (!sd.HasCompletedMission(campaignMission4.m_missionReferenceName) && campaignMission4.m_isKeyMission && sd.GetIntel() >= campaignMission4.m_minIntelRequired)
			{
				if (string.IsNullOrEmpty(campaignMission4.m_tagRequired))
				{
					list4.Add(campaignMission4);
				}
				else if (keysUnlocked.Contains(campaignMission4.m_tagRequired))
				{
					list4.Add(campaignMission4);
				}
			}
		}
		List<CampaignMission> list5 = new List<CampaignMission>();
		CampaignMission[] allMissions4 = m_allMissions;
		foreach (CampaignMission campaignMission5 in allMissions4)
		{
			if (sd.GetIntel() >= campaignMission5.m_minIntelRequired)
			{
				if (string.IsNullOrEmpty(campaignMission5.m_tagRequired))
				{
					list5.Add(campaignMission5);
				}
				else if (keysUnlocked.Contains(campaignMission5.m_tagRequired))
				{
					list5.Add(campaignMission5);
				}
			}
		}
		List<CampaignMission> list6 = new List<CampaignMission>();
		for (int num = 0; num < list4.Count; num++)
		{
			for (int num2 = num; num2 < list4.Count; num2++)
			{
				if (num2 == num || !(((Vector3)(list4[num].GetMissionTargetArea() - list4[num2].GetMissionTargetArea())).magnitude < 300f))
				{
					continue;
				}
				if (!list4[num].m_isKeyMission)
				{
					if (!list6.Contains(list4[num]))
					{
						list6.Add(list4[num]);
					}
				}
				else if (!list4[num2].m_isKeyMission && !list6.Contains(list4[num2]))
				{
					list6.Add(list4[num2]);
				}
			}
		}
		foreach (CampaignMission item in list6)
		{
			list4.Remove(item);
		}
		while (list4.Count > count)
		{
			bool flag2 = true;
			for (int num3 = 0; num3 < list4.Count; num3++)
			{
				if (!list4[num3].m_isKeyMission)
				{
					flag2 = false;
				}
			}
			if (flag2)
			{
				list4.RemoveAt(0);
				continue;
			}
			for (int num4 = 0; num4 < list4.Count; num4++)
			{
				if (!list4[num4].m_isKeyMission)
				{
					list4.RemoveAt(num4);
					break;
				}
			}
		}
		if (list4.Count < count)
		{
			bool flag3 = false;
			foreach (CampaignMission item2 in list4)
			{
				if (item2.m_isKeyMission)
				{
					flag3 = true;
				}
			}
			int num5 = 1;
			while (list4.Count < count && num5 < ((!flag3) ? int.MaxValue : 2))
			{
				if ((list4.Count > 0 && num5 >= 5) || list5.Count == 0)
				{
					list5.Sort((CampaignMission a, CampaignMission b) => sd.GetNumberOfTimesPlayed(a.m_missionReferenceName) - sd.GetNumberOfTimesPlayed(b.m_missionReferenceName));
					foreach (CampaignMission item3 in list5)
					{
						if (list4.Contains(item3) || !sd.HasPlayedMission(item3.m_missionReferenceName) || IsTooClose(item3, list4))
						{
							continue;
						}
						int numberOfTimesPlayed = sd.GetNumberOfTimesPlayed(item3.m_missionReferenceName);
						if (item3.m_isKeyMission)
						{
							continue;
						}
						bool flag4 = false;
						if (item3.m_canRepeatIfComplete && sd.HasCompletedMission(item3.m_missionReferenceName))
						{
							int num6 = sd.GetNumMissionsPlayed() - sd.GetLastPlayedTime(item3.m_missionReferenceName);
							if (num6 >= item3.m_completeRepeatDelay)
							{
								flag4 = true;
							}
						}
						if (item3.m_canRepeatIfFailed && !flag4 && !sd.HasCompletedMission(item3.m_missionReferenceName))
						{
							int num7 = sd.GetNumMissionsPlayed() - sd.GetLastPlayedTime(item3.m_missionReferenceName);
							if (num7 >= item3.m_failRepeatDelay)
							{
								flag4 = true;
							}
						}
						if (flag4)
						{
							list4.Add(item3);
							break;
						}
					}
					break;
				}
				list5.Sort((CampaignMission a, CampaignMission b) => r.Next() % 3 - 1);
				foreach (CampaignMission item4 in list5)
				{
					if (list4.Contains(item4) || !sd.HasPlayedMission(item4.m_missionReferenceName) || IsTooClose(item4, list4))
					{
						continue;
					}
					int numberOfTimesPlayed2 = sd.GetNumberOfTimesPlayed(item4.m_missionReferenceName);
					if (numberOfTimesPlayed2 > num5 && !item4.m_isKeyMission)
					{
						continue;
					}
					bool flag5 = false;
					if (item4.m_canRepeatIfComplete && sd.HasCompletedMission(item4.m_missionReferenceName))
					{
						int num8 = sd.GetNumMissionsPlayed() - sd.GetLastPlayedTime(item4.m_missionReferenceName);
						if (num8 >= item4.m_completeRepeatDelay)
						{
							flag5 = true;
						}
					}
					if (item4.m_canRepeatIfFailed && !flag5 && !sd.HasCompletedMission(item4.m_missionReferenceName))
					{
						int num9 = sd.GetNumMissionsPlayed() - sd.GetLastPlayedTime(item4.m_missionReferenceName);
						if (num9 >= item4.m_failRepeatDelay)
						{
							flag5 = true;
						}
					}
					if (flag5)
					{
						list4.Add(item4);
						break;
					}
				}
				num5++;
			}
		}
		int num10 = 0;
		foreach (CampaignMission item5 in list4)
		{
			if (item5.m_riskBase == 0)
			{
				num10++;
			}
		}
		while (num10 == 0 || list4.Count < count)
		{
			list5.Sort((CampaignMission a, CampaignMission b) => r.Next() % 3 - 1);
			bool flag6 = false;
			foreach (CampaignMission item6 in list5)
			{
				if (!list4.Contains(item6) && !item6.m_isKeyMission && item6.m_riskBase == 0 && sd.HasPlayedMission(item6.m_missionReferenceName) && !IsTooClose(item6, list4) && item6.m_canRepeatIfComplete && sd.HasCompletedMission(item6.m_missionReferenceName))
				{
					int num11 = sd.GetNumMissionsPlayed() - sd.GetLastPlayedTime(item6.m_missionReferenceName);
					if (num11 >= item6.m_completeRepeatDelay)
					{
						list4.Add(item6);
						num10++;
						flag6 = true;
						break;
					}
				}
			}
			if (!flag6)
			{
				break;
			}
		}
		return list4;
	}
}
